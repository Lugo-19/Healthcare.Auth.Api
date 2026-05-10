-- ============================================================
-- MIGRACIÓN V4 — SPs CRUD de Usuarios, Roles y Permisos
-- Base de datos: healthcare
-- Fecha: 2026-05-09
-- ============================================================

-- ====================================================
-- USUARIOS
-- ====================================================

CREATE OR REPLACE FUNCTION auth.sp_listar_usuarios(
    p_page INT, p_page_size INT,
    p_filtro VARCHAR DEFAULT NULL,
    p_id_estado SMALLINT DEFAULT NULL
) RETURNS TABLE(
    id UUID, nombre VARCHAR, apellido VARCHAR, correo VARCHAR,
    id_rol UUID, nombre_rol VARCHAR,
    ultimo_acceso TIMESTAMP,
    id_estado SMALLINT, nombre_estado VARCHAR,
    fecha_creacion TIMESTAMP, usuario_creacion VARCHAR,
    fecha_modificacion TIMESTAMP, usuario_modificacion VARCHAR,
    total_records BIGINT
) AS $func$
BEGIN
    RETURN QUERY
    SELECT u.id, u.nombre, u.apellido, u.correo,
           u.id_rol, r.nombre::VARCHAR,
           u.ultimo_acceso, u.id_estado, e.nombre::VARCHAR,
           u.fecha_creacion, u.usuario_creacion,
           u.fecha_modificacion, u.usuario_modificacion,
           COUNT(*) OVER() AS total_records
    FROM auth.usuarios u
    LEFT JOIN auth.roles r ON u.id_rol = r.id
    LEFT JOIN auth.estados e ON u.id_estado = e.id
    WHERE (p_filtro IS NULL OR
           u.nombre    ILIKE '%' || p_filtro || '%' OR
           u.apellido  ILIKE '%' || p_filtro || '%' OR
           u.correo    ILIKE '%' || p_filtro || '%')
      AND (p_id_estado IS NULL OR u.id_estado = p_id_estado)
    ORDER BY u.fecha_creacion DESC
    OFFSET (p_page - 1) * p_page_size
    LIMIT p_page_size;
END;
$func$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION auth.sp_obtener_usuario_por_id(p_id UUID)
RETURNS TABLE(
    id UUID, nombre VARCHAR, apellido VARCHAR, correo VARCHAR,
    id_rol UUID, nombre_rol VARCHAR,
    ultimo_acceso TIMESTAMP,
    id_estado SMALLINT, nombre_estado VARCHAR,
    fecha_creacion TIMESTAMP, usuario_creacion VARCHAR,
    fecha_modificacion TIMESTAMP, usuario_modificacion VARCHAR
) AS $func$
BEGIN
    RETURN QUERY
    SELECT u.id, u.nombre, u.apellido, u.correo,
           u.id_rol, r.nombre::VARCHAR,
           u.ultimo_acceso, u.id_estado, e.nombre::VARCHAR,
           u.fecha_creacion, u.usuario_creacion,
           u.fecha_modificacion, u.usuario_modificacion
    FROM auth.usuarios u
    LEFT JOIN auth.roles r ON u.id_rol = r.id
    LEFT JOIN auth.estados e ON u.id_estado = e.id
    WHERE u.id = p_id;
END;
$func$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION auth.sp_crear_usuario(
    p_nombre VARCHAR, p_apellido VARCHAR, p_correo VARCHAR,
    p_contrasena VARCHAR, p_id_rol UUID, p_usuario_creacion VARCHAR
) RETURNS UUID AS $func$
DECLARE v_id UUID;
BEGIN
    INSERT INTO auth.usuarios (nombre, apellido, correo, contrasena, id_rol, id_estado, usuario_creacion)
    VALUES (p_nombre, p_apellido, p_correo, p_contrasena, p_id_rol, 1, p_usuario_creacion)
    RETURNING id INTO v_id;
    RETURN v_id;
END;
$func$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION auth.sp_actualizar_usuario(
    p_id UUID, p_nombre VARCHAR, p_apellido VARCHAR,
    p_correo VARCHAR, p_id_rol UUID, p_usuario_modificacion VARCHAR
) RETURNS VOID AS $func$
BEGIN
    UPDATE auth.usuarios
    SET nombre = p_nombre, apellido = p_apellido, correo = p_correo, id_rol = p_id_rol,
        fecha_modificacion = NOW(), usuario_modificacion = p_usuario_modificacion
    WHERE id = p_id;
END;
$func$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION auth.sp_eliminar_usuario(p_id UUID, p_usuario_modificacion VARCHAR)
RETURNS VOID AS $func$
BEGIN
    UPDATE auth.usuarios
    SET id_estado = 3, fecha_modificacion = NOW(), usuario_modificacion = p_usuario_modificacion
    WHERE id = p_id;
END;
$func$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION auth.sp_cambiar_estado_usuario(
    p_id UUID, p_id_estado SMALLINT, p_usuario_modificacion VARCHAR
) RETURNS VOID AS $func$
BEGIN
    UPDATE auth.usuarios
    SET id_estado = p_id_estado, fecha_modificacion = NOW(), usuario_modificacion = p_usuario_modificacion
    WHERE id = p_id;
END;
$func$ LANGUAGE plpgsql;

-- ====================================================
-- ROLES
-- ====================================================

CREATE OR REPLACE FUNCTION auth.sp_listar_roles(
    p_page INT, p_page_size INT, p_filtro VARCHAR DEFAULT NULL
) RETURNS TABLE(
    id UUID, nombre VARCHAR, descripcion VARCHAR,
    id_estado SMALLINT, nombre_estado VARCHAR,
    fecha_creacion TIMESTAMP, usuario_creacion VARCHAR,
    fecha_modificacion TIMESTAMP, usuario_modificacion VARCHAR,
    total_records BIGINT
) AS $func$
BEGIN
    RETURN QUERY
    SELECT r.id, r.nombre, r.descripcion, r.id_estado, e.nombre::VARCHAR,
           r.fecha_creacion, r.usuario_creacion,
           r.fecha_modificacion, r.usuario_modificacion,
           COUNT(*) OVER()
    FROM auth.roles r
    LEFT JOIN auth.estados e ON r.id_estado = e.id
    WHERE p_filtro IS NULL OR r.nombre ILIKE '%' || p_filtro || '%'
    ORDER BY r.nombre
    OFFSET (p_page - 1) * p_page_size
    LIMIT p_page_size;
END;
$func$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION auth.sp_obtener_rol_por_id(p_id UUID)
RETURNS TABLE(
    id UUID, nombre VARCHAR, descripcion VARCHAR,
    id_estado SMALLINT, nombre_estado VARCHAR,
    fecha_creacion TIMESTAMP, usuario_creacion VARCHAR,
    fecha_modificacion TIMESTAMP, usuario_modificacion VARCHAR
) AS $func$
BEGIN
    RETURN QUERY
    SELECT r.id, r.nombre, r.descripcion, r.id_estado, e.nombre::VARCHAR,
           r.fecha_creacion, r.usuario_creacion,
           r.fecha_modificacion, r.usuario_modificacion
    FROM auth.roles r
    LEFT JOIN auth.estados e ON r.id_estado = e.id
    WHERE r.id = p_id;
END;
$func$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION auth.sp_crear_rol(
    p_nombre VARCHAR, p_descripcion VARCHAR, p_usuario_creacion VARCHAR
) RETURNS UUID AS $func$
DECLARE v_id UUID;
BEGIN
    INSERT INTO auth.roles (nombre, descripcion, id_estado, usuario_creacion)
    VALUES (p_nombre, p_descripcion, 1, p_usuario_creacion)
    RETURNING id INTO v_id;
    RETURN v_id;
END;
$func$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION auth.sp_actualizar_rol(
    p_id UUID, p_nombre VARCHAR, p_descripcion VARCHAR, p_usuario_modificacion VARCHAR
) RETURNS VOID AS $func$
BEGIN
    UPDATE auth.roles
    SET nombre = p_nombre, descripcion = p_descripcion,
        fecha_modificacion = NOW(), usuario_modificacion = p_usuario_modificacion
    WHERE id = p_id;
END;
$func$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION auth.sp_eliminar_rol(p_id UUID, p_usuario_modificacion VARCHAR)
RETURNS VOID AS $func$
BEGIN
    UPDATE auth.roles
    SET id_estado = 3, fecha_modificacion = NOW(), usuario_modificacion = p_usuario_modificacion
    WHERE id = p_id;
END;
$func$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION auth.sp_listar_permisos_de_rol(p_id_rol UUID)
RETURNS TABLE(
    id UUID, nombre VARCHAR, descripcion VARCHAR,
    modulo VARCHAR, accion VARCHAR
) AS $func$
BEGIN
    RETURN QUERY
    SELECT p.id, p.nombre, p.descripcion, p.modulo, p.accion
    FROM auth.permisos p
    INNER JOIN auth.roles_permisos rp ON p.id = rp.id_permiso
    WHERE rp.id_rol = p_id_rol AND p.id_estado = 1;
END;
$func$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION auth.sp_asignar_permisos_a_rol(
    p_id_rol UUID, p_ids_permisos UUID[], p_usuario_creacion VARCHAR
) RETURNS VOID AS $func$
BEGIN
    DELETE FROM auth.roles_permisos WHERE id_rol = p_id_rol;
    INSERT INTO auth.roles_permisos (id_rol, id_permiso, usuario_creacion)
    SELECT p_id_rol, unnest(p_ids_permisos), p_usuario_creacion;
END;
$func$ LANGUAGE plpgsql;

-- ====================================================
-- PERMISOS
-- ====================================================

CREATE OR REPLACE FUNCTION auth.sp_listar_permisos(
    p_page INT, p_page_size INT,
    p_filtro VARCHAR DEFAULT NULL,
    p_modulo VARCHAR DEFAULT NULL
) RETURNS TABLE(
    id UUID, nombre VARCHAR, descripcion VARCHAR,
    modulo VARCHAR, accion VARCHAR,
    id_estado SMALLINT, nombre_estado VARCHAR,
    fecha_creacion TIMESTAMP, usuario_creacion VARCHAR,
    fecha_modificacion TIMESTAMP, usuario_modificacion VARCHAR,
    total_records BIGINT
) AS $func$
BEGIN
    RETURN QUERY
    SELECT p.id, p.nombre, p.descripcion, p.modulo, p.accion,
           p.id_estado, e.nombre::VARCHAR,
           p.fecha_creacion, p.usuario_creacion,
           p.fecha_modificacion, p.usuario_modificacion,
           COUNT(*) OVER()
    FROM auth.permisos p
    LEFT JOIN auth.estados e ON p.id_estado = e.id
    WHERE (p_filtro IS NULL OR p.nombre ILIKE '%' || p_filtro || '%')
      AND (p_modulo IS NULL OR p.modulo = p_modulo)
    ORDER BY p.modulo, p.accion
    OFFSET (p_page - 1) * p_page_size
    LIMIT p_page_size;
END;
$func$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION auth.sp_obtener_permiso_por_id(p_id UUID)
RETURNS TABLE(
    id UUID, nombre VARCHAR, descripcion VARCHAR,
    modulo VARCHAR, accion VARCHAR,
    id_estado SMALLINT, nombre_estado VARCHAR,
    fecha_creacion TIMESTAMP, usuario_creacion VARCHAR,
    fecha_modificacion TIMESTAMP, usuario_modificacion VARCHAR
) AS $func$
BEGIN
    RETURN QUERY
    SELECT p.id, p.nombre, p.descripcion, p.modulo, p.accion,
           p.id_estado, e.nombre::VARCHAR,
           p.fecha_creacion, p.usuario_creacion,
           p.fecha_modificacion, p.usuario_modificacion
    FROM auth.permisos p
    LEFT JOIN auth.estados e ON p.id_estado = e.id
    WHERE p.id = p_id;
END;
$func$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION auth.sp_crear_permiso(
    p_nombre VARCHAR, p_descripcion VARCHAR,
    p_modulo VARCHAR, p_accion VARCHAR, p_usuario_creacion VARCHAR
) RETURNS UUID AS $func$
DECLARE v_id UUID;
BEGIN
    INSERT INTO auth.permisos (nombre, descripcion, modulo, accion, id_estado, usuario_creacion)
    VALUES (p_nombre, p_descripcion, p_modulo, p_accion, 1, p_usuario_creacion)
    RETURNING id INTO v_id;
    RETURN v_id;
END;
$func$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION auth.sp_actualizar_permiso(
    p_id UUID, p_nombre VARCHAR, p_descripcion VARCHAR,
    p_modulo VARCHAR, p_accion VARCHAR, p_usuario_modificacion VARCHAR
) RETURNS VOID AS $func$
BEGIN
    UPDATE auth.permisos
    SET nombre = p_nombre, descripcion = p_descripcion,
        modulo = p_modulo, accion = p_accion,
        fecha_modificacion = NOW(), usuario_modificacion = p_usuario_modificacion
    WHERE id = p_id;
END;
$func$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION auth.sp_eliminar_permiso(p_id UUID, p_usuario_modificacion VARCHAR)
RETURNS VOID AS $func$
BEGIN
    UPDATE auth.permisos
    SET id_estado = 3, fecha_modificacion = NOW(), usuario_modificacion = p_usuario_modificacion
    WHERE id = p_id;
END;
$func$ LANGUAGE plpgsql;

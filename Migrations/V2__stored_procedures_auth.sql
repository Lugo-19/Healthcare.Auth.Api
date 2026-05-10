-- ============================================================
-- MIGRACIÓN V2 — Stored procedures del módulo auth
-- Base de datos: healthcare
-- Fecha: 2026-05-09
--
-- En PostgreSQL los stored procedures que retornan resultsets se
-- implementan como FUNCTIONS (es la convención del motor).
-- Se llaman vía: SELECT * FROM auth.sp_xxx(@p1, @p2, ...)
-- ============================================================

-- 1. Obtener usuario por correo (login)
CREATE OR REPLACE FUNCTION auth.sp_obtener_usuario_por_correo(p_correo VARCHAR)
RETURNS TABLE(
    id UUID,
    nombre VARCHAR,
    apellido VARCHAR,
    correo VARCHAR,
    contrasena VARCHAR,
    id_rol UUID,
    nombre_rol VARCHAR,
    intentos_fallidos SMALLINT,
    bloqueado_hasta TIMESTAMP,
    id_estado SMALLINT
) AS $func$
BEGIN
    RETURN QUERY
    SELECT u.id, u.nombre, u.apellido, u.correo, u.contrasena, u.id_rol,
           r.nombre::VARCHAR AS nombre_rol,
           u.intentos_fallidos, u.bloqueado_hasta, u.id_estado
    FROM auth.usuarios u
    LEFT JOIN auth.roles r ON u.id_rol = r.id
    WHERE u.correo = p_correo;
END;
$func$ LANGUAGE plpgsql;

-- 2. Guardar refresh token
CREATE OR REPLACE FUNCTION auth.sp_guardar_refresh_token(
    p_id_usuario UUID, p_token VARCHAR, p_fecha_expiracion TIMESTAMP,
    p_ip VARCHAR, p_dispositivo VARCHAR
) RETURNS VOID AS $func$
BEGIN
    INSERT INTO auth.refresh_tokens (id_usuario, token, fecha_expiracion, ip_creacion, dispositivo, id_estado, usuario_creacion)
    VALUES (p_id_usuario, p_token, p_fecha_expiracion, p_ip, p_dispositivo, 1, 'SYSTEM');
END;
$func$ LANGUAGE plpgsql;

-- 3. Obtener refresh token con datos del usuario
CREATE OR REPLACE FUNCTION auth.sp_obtener_refresh_token(p_token VARCHAR)
RETURNS TABLE(
    id UUID,
    id_usuario UUID,
    token VARCHAR,
    fecha_expiracion TIMESTAMP,
    fecha_revocacion TIMESTAMP,
    id_estado SMALLINT,
    usuario_nombre VARCHAR,
    usuario_apellido VARCHAR,
    usuario_correo VARCHAR,
    usuario_id_rol UUID,
    usuario_nombre_rol VARCHAR,
    usuario_id_estado SMALLINT
) AS $func$
BEGIN
    RETURN QUERY
    SELECT rt.id, rt.id_usuario, rt.token, rt.fecha_expiracion, rt.fecha_revocacion, rt.id_estado,
           u.nombre::VARCHAR, u.apellido::VARCHAR, u.correo::VARCHAR,
           u.id_rol, r.nombre::VARCHAR, u.id_estado
    FROM auth.refresh_tokens rt
    INNER JOIN auth.usuarios u ON rt.id_usuario = u.id
    LEFT JOIN auth.roles r ON u.id_rol = r.id
    WHERE rt.token = p_token;
END;
$func$ LANGUAGE plpgsql;

-- 4. Revocar refresh token (logout)
CREATE OR REPLACE FUNCTION auth.sp_revocar_refresh_token(p_token VARCHAR, p_usuario VARCHAR)
RETURNS VOID AS $func$
BEGIN
    UPDATE auth.refresh_tokens
    SET fecha_revocacion = NOW(), id_estado = 2,
        fecha_modificacion = NOW(), usuario_modificacion = p_usuario
    WHERE token = p_token;
END;
$func$ LANGUAGE plpgsql;

-- 5. Actualizar último acceso y resetear intentos fallidos
CREATE OR REPLACE FUNCTION auth.sp_actualizar_ultimo_acceso(p_id_usuario UUID)
RETURNS VOID AS $func$
BEGIN
    UPDATE auth.usuarios
    SET ultimo_acceso = NOW(), intentos_fallidos = 0, bloqueado_hasta = NULL,
        fecha_modificacion = NOW(), usuario_modificacion = 'SYSTEM'
    WHERE id = p_id_usuario;
END;
$func$ LANGUAGE plpgsql;

-- 6. Incrementar intentos fallidos (bloqueo automático a los 5 intentos por 15 min)
CREATE OR REPLACE FUNCTION auth.sp_incrementar_intentos_fallidos(p_correo VARCHAR)
RETURNS VOID AS $func$
BEGIN
    UPDATE auth.usuarios
    SET intentos_fallidos = intentos_fallidos + 1,
        bloqueado_hasta = CASE
            WHEN intentos_fallidos + 1 >= 5 THEN NOW() + INTERVAL '15 minutes'
            ELSE bloqueado_hasta
        END,
        fecha_modificacion = NOW(), usuario_modificacion = 'SYSTEM'
    WHERE correo = p_correo;
END;
$func$ LANGUAGE plpgsql;

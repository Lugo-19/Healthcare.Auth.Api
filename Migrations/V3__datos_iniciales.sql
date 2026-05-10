-- ============================================================
-- MIGRACIÓN V3 — Datos iniciales: roles, permisos y usuarios de prueba
-- Base de datos: healthcare
-- Fecha: 2026-05-09
-- Nota: Los hashes de contraseña son BCrypt work factor 11
--       admin@healthcare.com → Admin123!
--       user@healthcare.com  → User123!
-- ============================================================

-- Roles base
INSERT INTO auth.roles (id, nombre, descripcion, id_estado, usuario_creacion)
VALUES
    ('a1b2c3d4-0001-0001-0001-000000000001', 'Administrador', 'Acceso total al sistema',   1, 'SYSTEM'),
    ('a1b2c3d4-0002-0002-0002-000000000002', 'Usuario',       'Acceso básico al sistema',   1, 'SYSTEM')
ON CONFLICT (nombre) DO NOTHING;

-- Permisos base
INSERT INTO auth.permisos (nombre, descripcion, modulo, accion, id_estado, usuario_creacion)
VALUES
    ('auth.login',        'Iniciar sesión',    'auth',     'login',    1, 'SYSTEM'),
    ('auth.logout',       'Cerrar sesión',     'auth',     'logout',   1, 'SYSTEM'),
    ('usuarios.leer',     'Ver usuarios',      'usuarios', 'leer',     1, 'SYSTEM'),
    ('usuarios.crear',    'Crear usuarios',    'usuarios', 'crear',    1, 'SYSTEM'),
    ('usuarios.editar',   'Editar usuarios',   'usuarios', 'editar',   1, 'SYSTEM'),
    ('usuarios.eliminar', 'Eliminar usuarios', 'usuarios', 'eliminar', 1, 'SYSTEM')
ON CONFLICT (nombre) DO NOTHING;

-- Usuarios de prueba
-- IMPORTANTE: Reemplazar los hashes con los generados por la aplicación en producción
INSERT INTO auth.usuarios (nombre, apellido, correo, contrasena, id_rol, id_estado, usuario_creacion)
VALUES
    ('Admin', 'Sistema', 'admin@healthcare.com',
     '$2a$11$UWbmoIEodyPBaXR8Nkp0zeiDz9JfrLhEDDw898P8BOtmgA/oxpSxK',
     'a1b2c3d4-0001-0001-0001-000000000001', 1, 'SYSTEM'),
    ('Usuario', 'Prueba', 'user@healthcare.com',
     '$2a$11$Pd./aLqN1ohxySKCjThTbe4roJNGNt26GNUaBarDjgMg3Lr2Lq.s6',
     'a1b2c3d4-0002-0002-0002-000000000002', 1, 'SYSTEM')
ON CONFLICT (correo) DO NOTHING;

-- ============================================================
-- MIGRACIÓN V1 — Crear esquema auth con tablas base
-- Base de datos: healthcare
-- Fecha: 2026-05-09
-- ============================================================

-- Esquema
CREATE SCHEMA IF NOT EXISTS auth;

-- Catálogo de estados
CREATE TABLE IF NOT EXISTS auth.estados (
    id              SMALLINT PRIMARY KEY,
    nombre          VARCHAR(50) NOT NULL,
    descripcion     VARCHAR(255)
);

INSERT INTO auth.estados (id, nombre, descripcion) VALUES
    (1, 'Activo',    'Registro activo en el sistema'),
    (2, 'Inactivo',  'Registro inactivo en el sistema'),
    (3, 'Eliminado', 'Registro eliminado lógicamente')
ON CONFLICT (id) DO NOTHING;

-- Roles
CREATE TABLE IF NOT EXISTS auth.roles (
    id                      UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    nombre                  VARCHAR(100) NOT NULL UNIQUE,
    descripcion             VARCHAR(255),
    id_estado               SMALLINT NOT NULL DEFAULT 1 REFERENCES auth.estados(id),
    fecha_creacion          TIMESTAMP NOT NULL DEFAULT NOW(),
    usuario_creacion        VARCHAR(100) NOT NULL DEFAULT 'SYSTEM',
    fecha_modificacion      TIMESTAMP,
    usuario_modificacion    VARCHAR(100)
);

-- Permisos
CREATE TABLE IF NOT EXISTS auth.permisos (
    id                      UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    nombre                  VARCHAR(100) NOT NULL UNIQUE,
    descripcion             VARCHAR(255),
    modulo                  VARCHAR(100) NOT NULL,
    accion                  VARCHAR(50) NOT NULL,
    id_estado               SMALLINT NOT NULL DEFAULT 1 REFERENCES auth.estados(id),
    fecha_creacion          TIMESTAMP NOT NULL DEFAULT NOW(),
    usuario_creacion        VARCHAR(100) NOT NULL DEFAULT 'SYSTEM',
    fecha_modificacion      TIMESTAMP,
    usuario_modificacion    VARCHAR(100)
);

-- Relación roles-permisos
CREATE TABLE IF NOT EXISTS auth.roles_permisos (
    id_rol                  UUID NOT NULL REFERENCES auth.roles(id),
    id_permiso              UUID NOT NULL REFERENCES auth.permisos(id),
    fecha_creacion          TIMESTAMP NOT NULL DEFAULT NOW(),
    usuario_creacion        VARCHAR(100) NOT NULL DEFAULT 'SYSTEM',
    PRIMARY KEY (id_rol, id_permiso)
);

-- Usuarios
CREATE TABLE IF NOT EXISTS auth.usuarios (
    id                      UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    nombre                  VARCHAR(100) NOT NULL,
    apellido                VARCHAR(100) NOT NULL,
    correo                  VARCHAR(255) NOT NULL UNIQUE,
    contrasena              VARCHAR(255) NOT NULL,
    id_rol                  UUID REFERENCES auth.roles(id),
    ultimo_acceso           TIMESTAMP,
    intentos_fallidos       SMALLINT NOT NULL DEFAULT 0,
    bloqueado_hasta         TIMESTAMP,
    id_estado               SMALLINT NOT NULL DEFAULT 1 REFERENCES auth.estados(id),
    fecha_creacion          TIMESTAMP NOT NULL DEFAULT NOW(),
    usuario_creacion        VARCHAR(100) NOT NULL DEFAULT 'SYSTEM',
    fecha_modificacion      TIMESTAMP,
    usuario_modificacion    VARCHAR(100)
);

-- Refresh tokens
CREATE TABLE IF NOT EXISTS auth.refresh_tokens (
    id                      UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    id_usuario              UUID NOT NULL REFERENCES auth.usuarios(id),
    token                   VARCHAR(500) NOT NULL UNIQUE,
    fecha_expiracion        TIMESTAMP NOT NULL,
    fecha_revocacion        TIMESTAMP,
    ip_creacion             VARCHAR(50),
    dispositivo             VARCHAR(255),
    id_estado               SMALLINT NOT NULL DEFAULT 1 REFERENCES auth.estados(id),
    fecha_creacion          TIMESTAMP NOT NULL DEFAULT NOW(),
    usuario_creacion        VARCHAR(100) NOT NULL DEFAULT 'SYSTEM',
    fecha_modificacion      TIMESTAMP,
    usuario_modificacion    VARCHAR(100)
);

-- Índices
CREATE INDEX IF NOT EXISTS idx_usuarios_correo         ON auth.usuarios(correo);
CREATE INDEX IF NOT EXISTS idx_usuarios_id_rol         ON auth.usuarios(id_rol);
CREATE INDEX IF NOT EXISTS idx_refresh_tokens_usuario  ON auth.refresh_tokens(id_usuario);
CREATE INDEX IF NOT EXISTS idx_permisos_modulo         ON auth.permisos(modulo);

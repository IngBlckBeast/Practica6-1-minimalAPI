-- 1. Crear la Base de Datos
-- Si ya existe y quieres borrarla para empezar de cero, descomenta la siguiente línea:
-- DROP DATABASE IF EXISTS AuthDb;
CREATE DATABASE AuthDb;
GO

USE AuthDb;
GO

-- 2. Crear Tabla Roles
CREATE TABLE Roles (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL UNIQUE
);
GO

-- 3. Crear Tabla Users (CORREGIDA)
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(50) NOT NULL, -- Nombre
    LastName NVARCHAR(50) NOT NULL,  -- Apellido
    Phone NVARCHAR(20),              -- Telefono
    Age INT,                         -- Edad
    RegisterDate DATETIME DEFAULT GETDATE(), -- Fecha registro
    Email NVARCHAR(100) NOT NULL UNIQUE,
    
    -- --- LOS DOS CAMPOS DE CONTRASEÑA ---
    Password NVARCHAR(MAX) NOT NULL,     -- Texto plano (Para el Ing.)
    PasswordHash NVARCHAR(MAX) NOT NULL, -- Encriptado (Para Producción)
    -- ------------------------------------

    RoleId INT NOT NULL,
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);
GO

-- 4. Insertar Roles (Admin y User)
INSERT INTO Roles (Name) VALUES ('Admin');
INSERT INTO Roles (Name) VALUES ('User');
GO

-- 5. Insertar Usuarios de Ejemplo
-- NOTA: Llenamos tanto Password (legible) como PasswordHash (para el login del sistema)

-- Usuario Admin (Rol 1)
-- Login: admin@test.com / Pass: Admin123!
INSERT INTO Users (FirstName, LastName, Phone, Age, Email, Password, PasswordHash, RoleId)
VALUES ('Daniel', 'Morales', '555-1111', 30, 'admin@test.com', 'Admin123!', 'HASH_DEL_ADMIN', 1);

-- Usuario Normal 1 (Rol 2)
-- Login: maritza@test.com / Pass: User123!
INSERT INTO Users (FirstName, LastName, Phone, Age, Email, Password, PasswordHash, RoleId)
VALUES ('Maritza', 'Cruz', '555-2222', 25, 'maritza@test.com', 'User123!', 'HASH_DE_USER', 2);

-- Usuario Normal 2 (Rol 2)
INSERT INTO Users (FirstName, LastName, Phone, Age, Email, Password, PasswordHash, RoleId)
VALUES ('Jose', 'Valencia', '555-3333', 28, 'jose@test.com', 'User123!', 'HASH_DE_USER', 2);
GO

-- 6. Verificar que todo quedó bien
SELECT FirstName, Email, Password, PasswordHash, r.Name as Rol 
FROM Users u 
JOIN Roles r ON u.RoleId = r.Id;
-- 1. Crear la Base de Datos
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

-- 3. Crear Tabla Users (Con los datos solicitados)
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(50) NOT NULL, -- Nombre
    LastName NVARCHAR(50) NOT NULL,  -- Apellido
    Phone NVARCHAR(20),              -- Telefono
    Age INT,                         -- Edad
    RegisterDate DATETIME DEFAULT GETDATE(), -- Fecha registro
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    RoleId INT NOT NULL,
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);
GO

-- 4. Insertar Roles (Admin y User)
INSERT INTO Roles (Name) VALUES ('Admin');
INSERT INTO Roles (Name) VALUES ('User');
GO

-- 5. Insertar Usuarios de Ejemplo
-- NOTA: En PasswordHash pondremos un hash de ejemplo. 
-- Si tu API usa BCrypt, este hash debería ser válido para la contraseña "User123!"
-- Hash de ejemplo (BCrypt para "User123!"): $2a$11$Z.y/J/..hash.. (Esto es simulado, abajo lo ajustamos en código)

-- Admin (Rol 1)
INSERT INTO Users (FirstName, LastName, Phone, Age, Email, PasswordHash, RoleId)
VALUES ('Daniel', 'Morales', '555-1111', 30, 'admin@test.com', 'HASH_DEL_ADMIN', 1);

-- Usuario 1 (Rol 2)
INSERT INTO Users (FirstName, LastName, Phone, Age, Email, PasswordHash, RoleId)
VALUES ('Maritza', 'Cruz', '555-2222', 21, 'maritza@test.com', 'HASH_DE_USER', 2);

-- Usuario 2 (Rol 2)
INSERT INTO Users (FirstName, LastName, Phone, Age, Email, PasswordHash, RoleId)
VALUES ('Jose', 'Valencia', '555-3333', 22, 'jose@test.com', 'HASH_DE_USER', 2);

-- Usuario 3 (Rol 2)
INSERT INTO Users (FirstName, LastName, Phone, Age, Email, PasswordHash, RoleId)
VALUES ('Jaime', 'Zurita', '555-4444', 23, 'daniel@test.com', 'HASH_DE_USER', 2);
GO

-- Verificar datos
SELECT u.FirstName, u.Email, r.Name as Rol 
FROM Users u 
JOIN Roles r ON u.RoleId = r.Id;
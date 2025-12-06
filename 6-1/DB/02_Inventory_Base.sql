-- 02_Inventory_Base.sql
-- Extensión de la base AuthDb para agregar inventario y movimientos

USE AuthDb;
GO

-- 1. Crear Tabla Productos
CREATE TABLE Productos (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(255),
    Precio DECIMAL(10,2) NOT NULL,
    Stock INT NOT NULL DEFAULT 0,
    FechaCreacion DATETIME DEFAULT GETDATE()
);
GO

-- 2. Crear Tabla Movimientos
CREATE TABLE Movimientos (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ProductoId INT NOT NULL,
    UserId INT NOT NULL,
    TipoMovimiento NVARCHAR(20) NOT NULL CHECK (TipoMovimiento IN ('Ingreso', 'Egreso')),
    Cantidad INT NOT NULL,
    FechaMovimiento DATETIME DEFAULT GETDATE(),
    Observaciones NVARCHAR(255),
    FOREIGN KEY (ProductoId) REFERENCES Productos(Id),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);
GO

-- 3. Insertar Productos de Ejemplo
INSERT INTO Productos (Nombre, Descripcion, Precio, Stock) VALUES
('Laptop Dell XPS 13', 'Laptop ultrabook 13 pulgadas', 1299.99, 15),
('Mouse Logitech MX Master', 'Mouse inalámbrico ergonómico', 89.99, 50),
('Teclado Mecánico Corsair', 'Teclado gaming RGB switches Cherry MX', 149.99, 25),
('Monitor Samsung 27"', 'Monitor 4K UHD 27 pulgadas', 399.99, 8),
('Auriculares Sony WH-1000XM4', 'Auriculares noise cancelling', 279.99, 20),
('SSD Samsung 1TB', 'Disco sólido NVMe M.2', 119.99, 35),
('Webcam Logitech C920', 'Cámara web HD 1080p', 79.99, 12),
('Tablet iPad Air', 'Tablet 10.9 pulgadas 256GB', 749.99, 6);
GO

-- 4. Insertar Movimientos de Ejemplo
-- Ingresos iniciales (stock)
INSERT INTO Movimientos (ProductoId, UserId, TipoMovimiento, Cantidad, Observaciones) VALUES
(1, 1, 'Ingreso', 15, 'Stock inicial laptops'),
(2, 1, 'Ingreso', 50, 'Stock inicial mouse'),
(3, 1, 'Ingreso', 25, 'Stock inicial teclados'),
(4, 1, 'Ingreso', 8, 'Stock inicial monitores'),
(5, 1, 'Ingreso', 20, 'Stock inicial auriculares'),
(6, 1, 'Ingreso', 35, 'Stock inicial SSD'),
(7, 1, 'Ingreso', 12, 'Stock inicial webcams'),
(8, 1, 'Ingreso', 6, 'Stock inicial tablets');

-- Algunos egresos (ventas)
INSERT INTO Movimientos (ProductoId, UserId, TipoMovimiento, Cantidad, Observaciones) VALUES
(1, 2, 'Egreso', 2, 'Venta a cliente corporativo'),
(2, 3, 'Egreso', 5, 'Venta retail'),
(3, 2, 'Egreso', 3, 'Venta online'),
(5, 3, 'Egreso', 1, 'Venta individual');
GO

-- 5. Actualizar stock de productos después de movimientos
UPDATE Productos SET Stock = Stock - 2 WHERE Id = 1; -- Laptops
UPDATE Productos SET Stock = Stock - 5 WHERE Id = 2; -- Mouse
UPDATE Productos SET Stock = Stock - 3 WHERE Id = 3; -- Teclados
UPDATE Productos SET Stock = Stock - 1 WHERE Id = 5; -- Auriculares
GO

-- 6. Verificar datos
SELECT * FROM Productos;
SELECT * FROM Movimientos ORDER BY FechaMovimiento DESC;
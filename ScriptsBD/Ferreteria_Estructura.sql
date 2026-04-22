CREATE DATABASE FerreteriaDB;
GO

USE FerreteriaDB;
GO

-- 1. SEGURIDAD Y ROLES
CREATE TABLE Roles (
    IdRol INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL
);

CREATE TABLE Usuarios (
    IdUsuario INT IDENTITY(1,1) PRIMARY KEY,
    Nombres VARCHAR(100) NOT NULL,
    Apellidos VARCHAR(100) NOT NULL,
    Correo VARCHAR(100) UNIQUE NOT NULL,
    Clave VARCHAR(255) NOT NULL, -- En un proyecto real iría encriptada
    IdRol INT NOT NULL,
    FOREIGN KEY (IdRol) REFERENCES Roles(IdRol)
);

-- 2. CATÁLOGO 
CREATE TABLE Categorias (
    IdCategoria INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL
);

CREATE TABLE Marcas (
    IdMarca INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL
);

CREATE TABLE Productos (
    IdProducto INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(150) NOT NULL,
    Descripcion VARCHAR(255),
    IdMarca INT NOT NULL,
    IdCategoria INT NOT NULL,
    Precio DECIMAL(10,2) NOT NULL,
    Stock INT NOT NULL,
    ImagenUrl VARCHAR(255), 
    Activo BIT DEFAULT 1, 
    FOREIGN KEY (IdMarca) REFERENCES Marcas(IdMarca),
    FOREIGN KEY (IdCategoria) REFERENCES Categorias(IdCategoria)
);

-- 3. PROCESO DE NEGOCIO: VENTAS (Carrito)
CREATE TABLE Ventas (
    IdVenta INT IDENTITY(1,1) PRIMARY KEY,
    IdUsuario INT NOT NULL, -- El cliente que hizo la compra
    FechaVenta DATETIME DEFAULT GETDATE(),
    ImporteTotal DECIMAL(10,2) NOT NULL,
    Estado VARCHAR(50) DEFAULT 'Pendiente', -- Pendiente, Pagado, Despachado
    FOREIGN KEY (IdUsuario) REFERENCES Usuarios(IdUsuario)
);

CREATE TABLE DetalleVentas (
    IdDetalle INT IDENTITY(1,1) PRIMARY KEY,
    IdVenta INT NOT NULL,
    IdProducto INT NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(10,2) NOT NULL,
    Importe DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (IdVenta) REFERENCES Ventas(IdVenta),
    FOREIGN KEY (IdProducto) REFERENCES Productos(IdProducto)
);
GO

-- 4. PROCEDIMIENTOS ALMACENADOS 

-- SP para Listar Productos (con nombres de marca y categoría)
CREATE PROCEDURE sp_ListarProductos
AS
BEGIN
    SELECT 
        p.IdProducto, p.Nombre, p.Descripcion, 
        m.Nombre AS Marca, c.Nombre AS Categoria, 
        p.Precio, p.Stock, p.ImagenUrl, p.Activo
    FROM Productos p
    INNER JOIN Marcas m ON p.IdMarca = m.IdMarca
    INNER JOIN Categorias c ON p.IdCategoria = c.IdCategoria
    WHERE p.Activo = 1;
END;
GO

-- SP para Insertar Producto
CREATE PROCEDURE sp_InsertarProducto
(
    @Nombre VARCHAR(150),
    @Descripcion VARCHAR(255),
    @IdMarca INT,
    @IdCategoria INT,
    @Precio DECIMAL(10,2),
    @Stock INT,
    @ImagenUrl VARCHAR(255)
)
AS
BEGIN
    INSERT INTO Productos (Nombre, Descripcion, IdMarca, IdCategoria, Precio, Stock, ImagenUrl)
    VALUES (@Nombre, @Descripcion, @IdMarca, @IdCategoria, @Precio, @Stock, @ImagenUrl);
END;
GO

-- SP para Actualizar Producto
CREATE PROCEDURE sp_ActualizarProducto
(
    @IdProducto INT,
    @Nombre VARCHAR(150),
    @Descripcion VARCHAR(255),
    @IdMarca INT,
    @IdCategoria INT,
    @Precio DECIMAL(10,2),
    @Stock INT,
    @ImagenUrl VARCHAR(255)
)
AS
BEGIN
    UPDATE Productos SET 
        Nombre = @Nombre, 
        Descripcion = @Descripcion, 
        IdMarca = @IdMarca, 
        IdCategoria = @IdCategoria, 
        Precio = @Precio, 
        Stock = @Stock, 
        ImagenUrl = @ImagenUrl
    WHERE IdProducto = @IdProducto;
END;
GO

-- SP para Eliminar (Borrado Lógico)
CREATE PROCEDURE sp_EliminarProducto
(
    @IdProducto INT
)
AS
BEGIN
    UPDATE Productos SET Activo = 0 WHERE IdProducto = @IdProducto;
END;
GO

-- 5. DATOS DE PRUEBA

INSERT INTO Roles (Nombre) VALUES ('Administrador'), ('Cliente'), ('Almacenero');
INSERT INTO Usuarios (Nombres, Apellidos, Correo, Clave, IdRol) VALUES 
('Juan', 'Admin', 'admin@ferreteria.com', '123456', 1),
('Maria', 'Cliente', 'maria@gmail.com', '123456', 2);

INSERT INTO Categorias (Nombre) VALUES ('Herramientas Manuales'), ('Herramientas Eléctricas'), ('Construcción'), ('Pinturas');
INSERT INTO Marcas (Nombre) VALUES ('Truper'), ('Bosch'), ('Sol'), ('Vencedor'), ('Stanley');

INSERT INTO Productos (Nombre, Descripcion, IdMarca, IdCategoria, Precio, Stock, ImagenUrl) VALUES
('Martillo Galponero', 'Martillo con mango de madera', 1, 1, 25.00, 50, 'martillo.jpg'),
('Taladro Percutor 13mm', 'Taladro profesional 700W', 2, 2, 180.00, 15, 'taladro.jpg'),
('Cemento Tipo 1', 'Bolsa de cemento 42.5kg', 3, 3, 32.50, 100, 'cemento.jpg');
GO
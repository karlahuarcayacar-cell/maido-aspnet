USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'maido_db')
BEGIN
    ALTER DATABASE maido_db SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE maido_db;
END
GO

CREATE DATABASE maido_db;
GO

USE maido_db;
GO

CREATE TABLE Roles (
    IdRol       INT           NOT NULL PRIMARY KEY,
    Nombre      NVARCHAR(50)  NOT NULL,
    Descripcion NVARCHAR(200) NULL
);
GO

CREATE TABLE Usuarios (
    IdUsuario       INT            NOT NULL IDENTITY(1,1) PRIMARY KEY,
    Nombre          NVARCHAR(100)  NOT NULL,
    Apellido        NVARCHAR(100)  NOT NULL,
    Email           NVARCHAR(150)  NOT NULL UNIQUE,
    PasswordHash    NVARCHAR(256)  NOT NULL,
    Telefono        NVARCHAR(20)   NULL,
    Direccion       NVARCHAR(300)  NULL,
    IdRol           INT            NOT NULL DEFAULT 2,
    Activo          BIT            NOT NULL DEFAULT 1,
    FechaRegistro   DATETIME       NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Usuarios_Roles FOREIGN KEY (IdRol) REFERENCES Roles(IdRol)
);
GO

CREATE TABLE Categorias (
    IdCategoria INT           NOT NULL IDENTITY(1,1) PRIMARY KEY,
    Nombre      NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(300) NULL,
    Icono       NVARCHAR(100) NULL,
    Orden       INT           NOT NULL DEFAULT 0,
    Activo      BIT           NOT NULL DEFAULT 1
);
GO

CREATE TABLE Platillos (
    IdPlatillo  INT             NOT NULL IDENTITY(1,1) PRIMARY KEY,
    Nombre      NVARCHAR(150)   NOT NULL,
    Descripcion NVARCHAR(500)   NULL,
    Precio      DECIMAL(10, 2)  NOT NULL,
    ImagenUrl   NVARCHAR(500)   NULL,
    IdCategoria INT             NOT NULL,
    Disponible  BIT             NOT NULL DEFAULT 1,
    Destacado   BIT             NOT NULL DEFAULT 0,
    FechaAlta   DATETIME        NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Platillos_Categorias FOREIGN KEY (IdCategoria) REFERENCES Categorias(IdCategoria)
);
GO

CREATE TABLE Pedidos (
    IdPedido         INT             NOT NULL IDENTITY(1,1) PRIMARY KEY,
    IdUsuario        INT             NOT NULL,
    FechaPedido      DATETIME        NOT NULL DEFAULT GETDATE(),
    TipoPedido       NVARCHAR(20)    NOT NULL CHECK (TipoPedido IN ('Delivery', 'Recojo')),
    DireccionEntrega NVARCHAR(300)   NULL,
    Telefono         NVARCHAR(20)    NULL,
    MetodoPago       NVARCHAR(50)    NOT NULL,
    Subtotal         DECIMAL(10, 2)  NOT NULL,
    IGV              DECIMAL(10, 2)  NOT NULL,
    Total            DECIMAL(10, 2)  NOT NULL,
    Estado           NVARCHAR(30)    NOT NULL DEFAULT 'Pendiente'
                     CHECK (Estado IN ('Pendiente','En Preparacion','En Camino','Entregado','Cancelado')),
    Observaciones    NVARCHAR(500)   NULL,
    CONSTRAINT FK_Pedidos_Usuarios FOREIGN KEY (IdUsuario) REFERENCES Usuarios(IdUsuario)
);
GO

CREATE TABLE DetallePedido (
    IdDetalle      INT             NOT NULL IDENTITY(1,1) PRIMARY KEY,
    IdPedido       INT             NOT NULL,
    IdPlatillo     INT             NOT NULL,
    NombrePlatillo NVARCHAR(150)   NOT NULL,
    PrecioUnitario DECIMAL(10, 2)  NOT NULL,
    Cantidad       INT             NOT NULL,
    Subtotal       DECIMAL(10, 2)  NOT NULL,
    CONSTRAINT FK_DetallePedido_Pedidos   FOREIGN KEY (IdPedido)   REFERENCES Pedidos(IdPedido),
    CONSTRAINT FK_DetallePedido_Platillos FOREIGN KEY (IdPlatillo) REFERENCES Platillos(IdPlatillo)
);
GO

INSERT INTO Roles (IdRol, Nombre, Descripcion) VALUES
(1, 'Administrador', 'Acceso total al sistema administrativo'),
(2, 'Cliente',       'Acceso al catalogo, carrito e historial de pedidos');
GO

INSERT INTO Usuarios (Nombre, Apellido, Email, PasswordHash, Telefono, IdRol)
VALUES ('Admin', 'Maido', 'admin@maido.pe',
        '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918',
        '999888777', 1);
GO

INSERT INTO Categorias (Nombre, Descripcion, Icono, Orden) VALUES
    ('Entradas y Piqueos', 'Pequeños platillos para iniciar la experiencia', 'bi-cup-hot', 1),
    ('Makis y Sushi', 'Rollos y cortes frescos con el toque nikkei',         'bi-record-circle',   2),
    ('Platos de Fondo', 'Platos principales de la cocina nikkei',    'bi-egg-fried',    3),
    ('Postres', 'El final perfecto y dulce para tu comida',            'bi-cake',          4),
    ('Bebidas y Cocteles', 'Bebidas refrescantes y coctelería de autor',     'bi-cup-straw',     5);
GO

INSERT INTO Platillos (Nombre, Descripcion, Precio, ImagenUrl, IdCategoria, Destacado) VALUES
('Tiradito Nikkei',
 'Laminas de corvina en leche de tigre con aji amarillo y ajonjoli tostado.',
 32.00, 'https://images.unsplash.com/photo-1580822184713-fc5400e7fe10?w=600&q=80', 1, 1),
('Ensalada Wakame',
 'Alga wakame fresca con pepino, sesamo y aderezo de limon yuzu.',
 22.00, 'https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=600&q=80', 1, 0),
('Causa Nikkei',
 'Causa limeña rellena de tartar de atun con mayonesa de wasabi.',
 28.00, 'https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=600&q=80', 1, 1),
('Dragon Roll',
 'Roll tempura de langostinos con palta y anguila glaseada en salsa teriyaki.',
 42.00, 'https://images.unsplash.com/photo-1553621042-f6e147245754?w=600&q=80', 2, 1),
('Maido Roll Especial',
 'Roll de atun, queso crema, aji amarillo, tobiko naranja y reduccion de anticucho.',
 48.00, 'https://images.unsplash.com/photo-1562802378-063ec186a863?w=600&q=80', 2, 1),
('Spicy Tuna Roll',
 'Roll de atun spicy, pepino y aguacate con salsa sriracha.',
 35.00, 'https://images.unsplash.com/photo-1611143669185-af224c5e3252?w=600&q=80', 2, 0),
('Arroz Nikkei',
 'Arroz con langostinos salteados, edamame, huevo y salsa sillao.',
 45.00, 'https://images.unsplash.com/photo-1603133872878-684f208fb84b?w=600&q=80', 3, 1),
('Seco de Wagyu',
 'Seco de res wagyu en salsa de cilantro, frejoles negros y yuca frita.',
 68.00, 'https://images.unsplash.com/photo-1544025162-d76694265947?w=600&q=80', 3, 1),
('Ceviche Clasico',
 'Corvina fresca en leche de tigre, aji limo, choclo y cancha serrana.',
 38.00, 'https://images.unsplash.com/photo-1555126634-323283e090fa?w=600&q=80', 3, 0),
('Mochi de Maracuya',
 'Mochi artesanal relleno de crema de maracuya con hoja de oro.',
 18.00, 'https://images.unsplash.com/photo-1599785209707-a456fc1337bb?w=600&q=80', 4, 0),
('Dorayaki de Lucuma',
 'Panqueques esponjosos rellenos de crema de lucuma con miel de abeja.',
 16.00, 'https://images.unsplash.com/photo-1567620905732-2d1ec7ab7445?w=600&q=80', 4, 1),
('Pisco Sour Clasico',
 'Pisco quebranta, limon, azucar, clara de huevo y amargo de Angostura.',
 22.00, 'https://images.unsplash.com/photo-1514362545857-3bc16c4c7d1b?w=600&q=80', 5, 1),
('Matcha Latte',
 'Te matcha ceremonial con leche de avena espumada y sirope de vainilla.',
 16.00, 'https://images.unsplash.com/photo-1536256263959-770b48d82b0a?w=600&q=80', 5, 0),
('Limonada Yuzu',
 'Limonada artesanal con yuzu, hierbabuena y agua con gas.',
 14.00, 'https://images.unsplash.com/photo-1621263764928-df1444c5e859?w=600&q=80', 5, 0);
GO

CREATE OR ALTER PROCEDURE sp_ObtenerUsuarioPorEmail
    @Email NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT u.IdUsuario, u.Nombre, u.Apellido, u.Email, u.PasswordHash,
           u.Telefono, u.Direccion, u.IdRol, r.Nombre AS NombreRol, u.Activo
    FROM Usuarios u
    INNER JOIN Roles r ON u.IdRol = r.IdRol
    WHERE u.Email = @Email AND u.Activo = 1;
END
GO

CREATE OR ALTER PROCEDURE sp_RegistrarUsuario
    @Nombre       NVARCHAR(100),
    @Apellido     NVARCHAR(100),
    @Email        NVARCHAR(150),
    @PasswordHash NVARCHAR(256),
    @Telefono     NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM Usuarios WHERE Email = @Email)
    BEGIN
        SELECT -1 AS IdUsuario;
        RETURN;
    END
    INSERT INTO Usuarios (Nombre, Apellido, Email, PasswordHash, Telefono, IdRol)
    VALUES (@Nombre, @Apellido, @Email, @PasswordHash, @Telefono, 2);
    SELECT SCOPE_IDENTITY() AS IdUsuario;
END
GO

CREATE OR ALTER PROCEDURE sp_ListarUsuarios
AS
BEGIN
    SET NOCOUNT ON;
    SELECT u.IdUsuario, u.Nombre, u.Apellido, u.Email, u.Telefono,
           u.IdRol, r.Nombre AS NombreRol, u.Activo, u.FechaRegistro
    FROM Usuarios u
    INNER JOIN Roles r ON u.IdRol = r.IdRol
    ORDER BY u.FechaRegistro DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_ActualizarEstadoUsuario
    @IdUsuario INT,
    @Activo    BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Usuarios SET Activo = @Activo WHERE IdUsuario = @IdUsuario;
END
GO

CREATE OR ALTER PROCEDURE sp_ListarCategorias
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdCategoria, Nombre, Descripcion, Icono, Orden, Activo
    FROM Categorias
    WHERE Activo = 1
    ORDER BY Orden;
END
GO

CREATE OR ALTER PROCEDURE sp_ListarCategoriasAdmin
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdCategoria, Nombre, Descripcion, Icono, Orden, Activo
    FROM Categorias
    ORDER BY Orden;
END
GO

CREATE OR ALTER PROCEDURE sp_ObtenerCategoriaPorId
    @IdCategoria INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdCategoria, Nombre, Descripcion, Icono, Orden, Activo
    FROM Categorias
    WHERE IdCategoria = @IdCategoria;
END
GO

CREATE OR ALTER PROCEDURE sp_InsertarCategoria
    @Nombre      NVARCHAR(100),
    @Descripcion NVARCHAR(300),
    @Icono       NVARCHAR(100),
    @Orden       INT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Categorias (Nombre, Descripcion, Icono, Orden)
    VALUES (@Nombre, @Descripcion, @Icono, @Orden);
    SELECT SCOPE_IDENTITY() AS IdCategoria;
END
GO

CREATE OR ALTER PROCEDURE sp_ActualizarCategoria
    @IdCategoria INT,
    @Nombre      NVARCHAR(100),
    @Descripcion NVARCHAR(300),
    @Icono       NVARCHAR(100),
    @Orden       INT,
    @Activo      BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Categorias
    SET Nombre      = @Nombre,
        Descripcion = @Descripcion,
        Icono       = @Icono,
        Orden       = @Orden,
        Activo      = @Activo
    WHERE IdCategoria = @IdCategoria;
END
GO

CREATE OR ALTER PROCEDURE sp_EliminarCategoria
    @IdCategoria INT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM Platillos WHERE IdCategoria = @IdCategoria)
        UPDATE Categorias SET Activo = 0 WHERE IdCategoria = @IdCategoria;
    ELSE
        DELETE FROM Categorias WHERE IdCategoria = @IdCategoria;
END
GO

CREATE OR ALTER PROCEDURE sp_ListarPlatillosPublico
    @IdCategoria INT           = NULL,
    @Busqueda    NVARCHAR(150) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT p.IdPlatillo, p.Nombre, p.Descripcion, p.Precio,
           p.ImagenUrl, p.IdCategoria, c.Nombre AS NombreCategoria,
           p.Disponible, p.Destacado
    FROM Platillos p
    INNER JOIN Categorias c ON p.IdCategoria = c.IdCategoria
    WHERE p.Disponible = 1
      AND c.Activo     = 1
      AND (@IdCategoria IS NULL OR p.IdCategoria = @IdCategoria)
      AND (@Busqueda    IS NULL OR p.Nombre LIKE '%' + @Busqueda + '%')
    ORDER BY p.Destacado DESC, p.Nombre;
END
GO

CREATE OR ALTER PROCEDURE sp_ListarPlatillosPaginado
    @Pagina             INT           = 1,
    @RegistrosPorPagina INT           = 10,
    @IdCategoria        INT           = NULL,
    @Busqueda           NVARCHAR(150) = NULL,
    @TotalRegistros     INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT @TotalRegistros = COUNT(*)
    FROM Platillos p
    INNER JOIN Categorias c ON p.IdCategoria = c.IdCategoria
    WHERE (@IdCategoria IS NULL OR p.IdCategoria = @IdCategoria)
      AND (@Busqueda    IS NULL OR p.Nombre LIKE '%' + @Busqueda + '%');

    SELECT p.IdPlatillo, p.Nombre, p.Descripcion, p.Precio,
           p.ImagenUrl, p.IdCategoria, c.Nombre AS NombreCategoria,
           p.Disponible, p.Destacado, p.FechaAlta
    FROM Platillos p
    INNER JOIN Categorias c ON p.IdCategoria = c.IdCategoria
    WHERE (@IdCategoria IS NULL OR p.IdCategoria = @IdCategoria)
      AND (@Busqueda    IS NULL OR p.Nombre LIKE '%' + @Busqueda + '%')
    ORDER BY p.FechaAlta DESC
    OFFSET (@Pagina - 1) * @RegistrosPorPagina ROWS
    FETCH NEXT @RegistrosPorPagina ROWS ONLY;
END
GO

CREATE OR ALTER PROCEDURE sp_ObtenerPlatilloPorId
    @IdPlatillo INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT p.IdPlatillo, p.Nombre, p.Descripcion, p.Precio,
           p.ImagenUrl, p.IdCategoria, c.Nombre AS NombreCategoria,
           p.Disponible, p.Destacado, p.FechaAlta
    FROM Platillos p
    INNER JOIN Categorias c ON p.IdCategoria = c.IdCategoria
    WHERE p.IdPlatillo = @IdPlatillo;
END
GO

CREATE OR ALTER PROCEDURE sp_InsertarPlatillo
    @Nombre      NVARCHAR(150),
    @Descripcion NVARCHAR(500),
    @Precio      DECIMAL(10,2),
    @ImagenUrl   NVARCHAR(500),
    @IdCategoria INT,
    @Disponible  BIT,
    @Destacado   BIT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Platillos (Nombre, Descripcion, Precio, ImagenUrl, IdCategoria, Disponible, Destacado)
    VALUES (@Nombre, @Descripcion, @Precio, @ImagenUrl, @IdCategoria, @Disponible, @Destacado);
    SELECT SCOPE_IDENTITY() AS IdPlatillo;
END
GO

CREATE OR ALTER PROCEDURE sp_ActualizarPlatillo
    @IdPlatillo  INT,
    @Nombre      NVARCHAR(150),
    @Descripcion NVARCHAR(500),
    @Precio      DECIMAL(10,2),
    @ImagenUrl   NVARCHAR(500),
    @IdCategoria INT,
    @Disponible  BIT,
    @Destacado   BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Platillos
    SET Nombre      = @Nombre,
        Descripcion = @Descripcion,
        Precio      = @Precio,
        ImagenUrl   = CASE WHEN @ImagenUrl IS NOT NULL THEN @ImagenUrl ELSE ImagenUrl END,
        IdCategoria = @IdCategoria,
        Disponible  = @Disponible,
        Destacado   = @Destacado
    WHERE IdPlatillo = @IdPlatillo;
END
GO

CREATE OR ALTER PROCEDURE sp_EliminarPlatillo
    @IdPlatillo INT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM DetallePedido WHERE IdPlatillo = @IdPlatillo)
        UPDATE Platillos SET Disponible = 0 WHERE IdPlatillo = @IdPlatillo;
    ELSE
        DELETE FROM Platillos WHERE IdPlatillo = @IdPlatillo;
END
GO

CREATE OR ALTER PROCEDURE sp_RegistrarPedidoTransaccional
    @IdUsuario        INT,
    @TipoPedido       NVARCHAR(20),
    @DireccionEntrega NVARCHAR(300),
    @Telefono         NVARCHAR(20),
    @MetodoPago       NVARCHAR(50),
    @Subtotal         DECIMAL(10,2),
    @IGV              DECIMAL(10,2),
    @Total            DECIMAL(10,2),
    @Observaciones    NVARCHAR(500),
    @DetalleJSON      NVARCHAR(MAX),
    @IdPedido         INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;
    BEGIN TRY
        INSERT INTO Pedidos
            (IdUsuario, TipoPedido, DireccionEntrega, Telefono,
             MetodoPago, Subtotal, IGV, Total, Observaciones)
        VALUES
            (@IdUsuario, @TipoPedido, @DireccionEntrega, @Telefono,
             @MetodoPago, @Subtotal, @IGV, @Total, @Observaciones);

        SET @IdPedido = SCOPE_IDENTITY();

        INSERT INTO DetallePedido
            (IdPedido, IdPlatillo, NombrePlatillo, PrecioUnitario, Cantidad, Subtotal)
        SELECT
            @IdPedido,
            CAST(j.IdPlatillo AS INT),
            j.NombrePlatillo,
            CAST(j.PrecioUnitario AS DECIMAL(10,2)),
            CAST(j.Cantidad AS INT),
            CAST(j.PrecioUnitario AS DECIMAL(10,2)) * CAST(j.Cantidad AS INT)
        FROM OPENJSON(@DetalleJSON)
        WITH (
            IdPlatillo     INT            '$.IdPlatillo',
            NombrePlatillo NVARCHAR(150)  '$.NombrePlatillo',
            PrecioUnitario DECIMAL(10,2)  '$.PrecioUnitario',
            Cantidad       INT            '$.Cantidad'
        ) AS j;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SET @IdPedido = -1;
        THROW;
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE sp_ObtenerPedidoPorId
    @IdPedido INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT p.IdPedido, p.FechaPedido, p.TipoPedido, p.DireccionEntrega,
           p.Telefono, p.MetodoPago, p.Subtotal, p.IGV, p.Total,
           p.Estado, p.Observaciones,
           u.Nombre + ' ' + u.Apellido AS NombreCliente, u.Email
    FROM Pedidos p
    INNER JOIN Usuarios u ON p.IdUsuario = u.IdUsuario
    WHERE p.IdPedido = @IdPedido;
END
GO

CREATE OR ALTER PROCEDURE sp_ObtenerDetallePedido
    @IdPedido INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdDetalle, IdPedido, IdPlatillo, NombrePlatillo,
           PrecioUnitario, Cantidad, Subtotal
    FROM DetallePedido
    WHERE IdPedido = @IdPedido;
END
GO

CREATE OR ALTER PROCEDURE sp_ListarPedidosPorUsuario
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdPedido, FechaPedido, TipoPedido, MetodoPago, Total, Estado
    FROM Pedidos
    WHERE IdUsuario = @IdUsuario
    ORDER BY FechaPedido DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_ListarPedidosPaginado
    @Pagina             INT          = 1,
    @RegistrosPorPagina INT          = 10,
    @Estado             NVARCHAR(30) = NULL,
    @FechaInicio        DATE         = NULL,
    @FechaFin           DATE         = NULL,
    @IdUsuario          INT          = NULL,
    @TotalRegistros     INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT @TotalRegistros = COUNT(*)
    FROM Pedidos p
    WHERE (@Estado      IS NULL OR p.Estado = @Estado)
      AND (@FechaInicio IS NULL OR CAST(p.FechaPedido AS DATE) >= @FechaInicio)
      AND (@FechaFin    IS NULL OR CAST(p.FechaPedido AS DATE) <= @FechaFin)
      AND (@IdUsuario   IS NULL OR p.IdUsuario = @IdUsuario);

    SELECT p.IdPedido, p.FechaPedido, p.TipoPedido, p.MetodoPago,
           p.Subtotal, p.IGV, p.Total, p.Estado,
           u.Nombre + ' ' + u.Apellido AS NombreCliente
    FROM Pedidos p
    INNER JOIN Usuarios u ON p.IdUsuario = u.IdUsuario
    WHERE (@Estado      IS NULL OR p.Estado = @Estado)
      AND (@FechaInicio IS NULL OR CAST(p.FechaPedido AS DATE) >= @FechaInicio)
      AND (@FechaFin    IS NULL OR CAST(p.FechaPedido AS DATE) <= @FechaFin)
      AND (@IdUsuario   IS NULL OR p.IdUsuario = @IdUsuario)
    ORDER BY p.FechaPedido DESC
    OFFSET (@Pagina - 1) * @RegistrosPorPagina ROWS
    FETCH NEXT @RegistrosPorPagina ROWS ONLY;
END
GO

CREATE OR ALTER PROCEDURE sp_ActualizarEstadoPedido
    @IdPedido INT,
    @Estado   NVARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Pedidos SET Estado = @Estado WHERE IdPedido = @IdPedido;
END
GO

CREATE OR ALTER PROCEDURE sp_ReporteVentasPorFecha
    @FechaInicio DATE,
    @FechaFin    DATE
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        CAST(p.FechaPedido AS DATE)  AS Fecha,
        COUNT(DISTINCT p.IdPedido)   AS TotalPedidos,
        SUM(p.Total)                 AS MontoTotal
    FROM Pedidos p
    WHERE p.Estado <> 'Cancelado'
      AND CAST(p.FechaPedido AS DATE) BETWEEN @FechaInicio AND @FechaFin
    GROUP BY CAST(p.FechaPedido AS DATE)
    ORDER BY Fecha;
END
GO

CREATE OR ALTER PROCEDURE sp_ReportePlatillosMasVendidos
    @FechaInicio DATE = NULL,
    @FechaFin    DATE = NULL,
    @Top         INT  = 10
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP (@Top)
        dp.IdPlatillo,
        dp.NombrePlatillo,
        SUM(dp.Cantidad)  AS TotalUnidades,
        SUM(dp.Subtotal)  AS TotalIngresos
    FROM DetallePedido dp
    INNER JOIN Pedidos p ON dp.IdPedido = p.IdPedido
    WHERE p.Estado <> 'Cancelado'
      AND (@FechaInicio IS NULL OR CAST(p.FechaPedido AS DATE) >= @FechaInicio)
      AND (@FechaFin    IS NULL OR CAST(p.FechaPedido AS DATE) <= @FechaFin)
    GROUP BY dp.IdPlatillo, dp.NombrePlatillo
    ORDER BY TotalUnidades DESC;
END
GO

PRINT 'Script maido_db ejecutado correctamente.';
GO

CREATE PROCEDURE sp_ActualizarPerfilUsuario
    @IdUsuario INT,
    @Nombre NVARCHAR(100),
    @Apellido NVARCHAR(100),
    @Telefono NVARCHAR(20),
    @Direccion NVARCHAR(200)
AS
BEGIN
    UPDATE Usuarios
    SET Nombre = @Nombre,
        Apellido = @Apellido,
        Telefono = @Telefono,
        Direccion = @Direccion
    WHERE IdUsuario = @IdUsuario;
END
GO

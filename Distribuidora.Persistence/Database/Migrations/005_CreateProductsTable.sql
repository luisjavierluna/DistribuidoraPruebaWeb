-- =====================================================
-- Script: 005_CreateProductsTable.sql
-- Descripción: Crea la tabla Products (depende de ProductTypes)
-- =====================================================

USE DistribuidoraDb;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    -- Verificar si la tabla ya existe
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Products')
    BEGIN
        CREATE TABLE dbo.Products (
            Id INT PRIMARY KEY IDENTITY(1,1),
            Code NVARCHAR(100) NOT NULL UNIQUE,
            Name NVARCHAR(255) NOT NULL,
            ProductTypeId INT NOT NULL,
            Price DECIMAL(10, 2),
            Active BIT NOT NULL DEFAULT 1,
            CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
            UpdatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
            CONSTRAINT FK_Products_ProductTypes FOREIGN KEY (ProductTypeId) REFERENCES dbo.ProductTypes(Id)
        );
        
        CREATE INDEX IX_Products_ProductTypeId ON dbo.Products(ProductTypeId);
        CREATE INDEX IX_Products_Active ON dbo.Products(Active);
        CREATE INDEX IX_Products_Code ON dbo.Products(Code);
        
        PRINT 'Tabla Products creada exitosamente.';
    END
    ELSE
    BEGIN
        PRINT 'La tabla Products ya existe.';
    END

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
    DECLARE @ErrorState INT = ERROR_STATE();
    
    RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH;

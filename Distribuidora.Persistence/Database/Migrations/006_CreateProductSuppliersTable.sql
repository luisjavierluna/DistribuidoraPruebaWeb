-- =====================================================
-- Script: 006_CreateProductSuppliersTable.sql
-- Descripción: Crea tabla puente ProductSuppliers (depende de Products y Suppliers)
-- =====================================================

USE DistribuidoraDb;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    -- Verificar si la tabla ya existe
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'ProductSuppliers')
    BEGIN
        CREATE TABLE dbo.ProductSuppliers (
            Id INT PRIMARY KEY IDENTITY(1,1),
            ProductId INT NOT NULL,
            SupplierId INT NOT NULL,
            SupplierProductCode NVARCHAR(100),
            Cost DECIMAL(10, 2) NOT NULL,
            Active BIT NOT NULL DEFAULT 1,
            CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
            UpdatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
            CONSTRAINT FK_ProductSuppliers_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id) ON DELETE CASCADE,
            CONSTRAINT FK_ProductSuppliers_Suppliers FOREIGN KEY (SupplierId) REFERENCES dbo.Suppliers(Id),
            CONSTRAINT UQ_ProductSuppliers_ProductSupplier UNIQUE (ProductId, SupplierId)
        );
        
        CREATE INDEX IX_ProductSuppliers_ProductId ON dbo.ProductSuppliers(ProductId);
        CREATE INDEX IX_ProductSuppliers_SupplierId ON dbo.ProductSuppliers(SupplierId);
        CREATE INDEX IX_ProductSuppliers_Active ON dbo.ProductSuppliers(Active);
        
        PRINT 'Tabla ProductSuppliers creada exitosamente.';
    END
    ELSE
    BEGIN
        PRINT 'La tabla ProductSuppliers ya existe.';
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

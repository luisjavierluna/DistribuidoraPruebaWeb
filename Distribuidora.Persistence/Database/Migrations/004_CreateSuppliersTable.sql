-- =====================================================
-- Script: 004_CreateSuppliersTable.sql
-- Descripción: Crea la tabla de catálogo Suppliers
-- =====================================================

USE DistribuidoraDb;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    -- Verificar si la tabla ya existe
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Suppliers')
    BEGIN
        CREATE TABLE dbo.Suppliers (
            Id INT PRIMARY KEY IDENTITY(1,1),
            Name NVARCHAR(255) NOT NULL,
            Description NVARCHAR(MAX),
            Active BIT NOT NULL DEFAULT 1,
            CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
            UpdatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
        );
        
        CREATE INDEX IX_Suppliers_Active ON dbo.Suppliers(Active);
        
        PRINT 'Tabla Suppliers creada exitosamente.';
    END
    ELSE
    BEGIN
        PRINT 'La tabla Suppliers ya existe.';
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

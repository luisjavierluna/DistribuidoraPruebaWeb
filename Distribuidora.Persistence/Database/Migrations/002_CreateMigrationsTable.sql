-- =====================================================
-- Script: 002_CreateMigrationsTable.sql
-- Descripción: Crea la tabla de control de migraciones
-- =====================================================

USE DistribuidoraDb;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    -- Verificar si la tabla ya existe
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Migrations')
    BEGIN
        CREATE TABLE dbo.Migrations (
            Id INT PRIMARY KEY IDENTITY(1,1),
            MigrationName NVARCHAR(255) NOT NULL UNIQUE,
            ExecutedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
        );
        
        PRINT 'Tabla Migrations creada exitosamente.';
    END
    ELSE
    BEGIN
        PRINT 'La tabla Migrations ya existe.';
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

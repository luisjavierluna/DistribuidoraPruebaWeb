-- =====================================================
-- Script: 001_CreateDatabase.sql
-- Descripción: Crea la base de datos DistribuidoraDb
-- =====================================================

BEGIN TRY
    -- Verificar si la base de datos ya existe
    IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = 'DistribuidoraDb')
    BEGIN
        CREATE DATABASE DistribuidoraDb;
        PRINT 'Base de datos DistribuidoraDb creada exitosamente.';
    END
    ELSE
    BEGIN
        PRINT 'La base de datos DistribuidoraDb ya existe.';
    END
END TRY
BEGIN CATCH
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
    DECLARE @ErrorState INT = ERROR_STATE();
    
    RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH;

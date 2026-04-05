-- =====================================================
-- Script: 008_sp_Suppliers_GetAll.sql
-- Descripción: Stored Procedure para obtener todos los proveedores
-- =====================================================

USE DistribuidoraDb;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    -- Verificar si el SP ya existe
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA = 'dbo' AND ROUTINE_NAME = 'sp_Suppliers_GetAll')
    BEGIN
        EXEC('
            CREATE PROCEDURE dbo.sp_Suppliers_GetAll
            AS
            BEGIN
                SELECT 
                    Id,
                    Name,
                    Description,
                    Active,
                    CreatedAt,
                    UpdatedAt
                FROM dbo.Suppliers
                WHERE Active = 1
                ORDER BY Name ASC;
            END
        ');
        PRINT 'Stored Procedure sp_Suppliers_GetAll creado exitosamente.';
    END
    ELSE
    BEGIN
        PRINT 'El Stored Procedure sp_Suppliers_GetAll ya existe.';
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

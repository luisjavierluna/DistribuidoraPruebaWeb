-- =====================================================
-- Script: 016_sp_ProductSuppliers_Delete.sql
-- Descripción: Stored Procedure para eliminar/desactivar un ProductoProveedor (soft delete)
-- =====================================================

USE DistribuidoraDb;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    -- Verificar si el SP ya existe
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA = 'dbo' AND ROUTINE_NAME = 'sp_ProductSuppliers_Delete')
    BEGIN
        EXEC('
            CREATE PROCEDURE dbo.sp_ProductSuppliers_Delete
                @Id INT
            AS
            BEGIN
                BEGIN TRY
                    BEGIN TRANSACTION;
                    
                    UPDATE dbo.ProductSuppliers
                    SET 
                        Active = 0,
                        UpdatedAt = GETUTCDATE()
                    WHERE Id = @Id;
                    
                    COMMIT TRANSACTION;
                END TRY
                BEGIN CATCH
                    ROLLBACK TRANSACTION;
                    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
                    RAISERROR (@ErrorMessage, 16, 1);
                END CATCH;
            END
        ');
        PRINT 'Stored Procedure sp_ProductSuppliers_Delete creado exitosamente.';
    END
    ELSE
    BEGIN
        PRINT 'El Stored Procedure sp_ProductSuppliers_Delete ya existe.';
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

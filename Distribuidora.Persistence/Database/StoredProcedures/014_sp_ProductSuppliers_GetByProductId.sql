-- =====================================================
-- Script: 014_sp_ProductSuppliers_GetByProductId.sql
-- Descripción: Stored Procedure para obtener proveedores de un producto
-- =====================================================

USE DistribuidoraDb;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    -- Verificar si el SP ya existe
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA = 'dbo' AND ROUTINE_NAME = 'sp_ProductSuppliers_GetByProductId')
    BEGIN
        EXEC('
            CREATE PROCEDURE dbo.sp_ProductSuppliers_GetByProductId
                @ProductId INT
            AS
            BEGIN
                SELECT 
                    ps.Id,
                    ps.ProductId,
                    ps.SupplierId,
                    s.Name AS SupplierName,
                    ps.SupplierProductCode,
                    ps.Cost,
                    ps.Active,
                    ps.CreatedAt,
                    ps.UpdatedAt
                FROM dbo.ProductSuppliers ps
                INNER JOIN dbo.Suppliers s ON ps.SupplierId = s.Id
                WHERE ps.ProductId = @ProductId
                  AND ps.Active = 1
                ORDER BY s.Name ASC;
            END
        ');
        PRINT 'Stored Procedure sp_ProductSuppliers_GetByProductId creado exitosamente.';
    END
    ELSE
    BEGIN
        PRINT 'El Stored Procedure sp_ProductSuppliers_GetByProductId ya existe.';
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

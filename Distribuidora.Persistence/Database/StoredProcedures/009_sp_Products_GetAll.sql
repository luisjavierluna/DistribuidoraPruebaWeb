-- =====================================================
-- Script: 009_sp_Products_GetAll.sql
-- Descripción: Stored Procedure para obtener todos los productos con filtros
-- =====================================================

USE DistribuidoraDb;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    -- Verificar si el SP ya existe
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA = 'dbo' AND ROUTINE_NAME = 'sp_Products_GetAll')
    BEGIN
        EXEC('
            CREATE PROCEDURE dbo.sp_Products_GetAll
                @Code NVARCHAR(100) = NULL,
                @ProductTypeId INT = NULL
            AS
            BEGIN
                SELECT 
                    p.Id,
                    p.Code,
                    p.Name,
                    p.ProductTypeId,
                    pt.Name AS ProductTypeName,
                    p.Price,
                    p.Active,
                    p.CreatedAt,
                    p.UpdatedAt
                FROM dbo.Products p
                INNER JOIN dbo.ProductTypes pt ON p.ProductTypeId = pt.Id
                WHERE p.Active = 1
                  AND (@Code IS NULL OR p.Code LIKE ''%'' + @Code + ''%'')
                  AND (@ProductTypeId IS NULL OR p.ProductTypeId = @ProductTypeId)
                ORDER BY p.Name ASC;
            END
        ');
        PRINT 'Stored Procedure sp_Products_GetAll creado exitosamente.';
    END
    ELSE
    BEGIN
        PRINT 'El Stored Procedure sp_Products_GetAll ya existe.';
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

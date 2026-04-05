-- =====================================================
-- Script: 012_sp_Products_Update.sql
-- Descripción: Stored Procedure para actualizar un producto
-- =====================================================

USE DistribuidoraDb;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    -- Verificar si el SP ya existe
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA = 'dbo' AND ROUTINE_NAME = 'sp_Products_Update')
    BEGIN
        EXEC('
            CREATE PROCEDURE dbo.sp_Products_Update
                @Id INT,
                @Code NVARCHAR(100),
                @Name NVARCHAR(255),
                @ProductTypeId INT,
                @Price DECIMAL(10, 2) = NULL,
                @Active BIT = 1
            AS
            BEGIN
                BEGIN TRY
                    BEGIN TRANSACTION;
                    
                    UPDATE dbo.Products
                    SET 
                        Code = @Code,
                        Name = @Name,
                        ProductTypeId = @ProductTypeId,
                        Price = @Price,
                        Active = @Active,
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
        PRINT 'Stored Procedure sp_Products_Update creado exitosamente.';
    END
    ELSE
    BEGIN
        PRINT 'El Stored Procedure sp_Products_Update ya existe.';
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

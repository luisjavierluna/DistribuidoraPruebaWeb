-- =====================================================
-- Script: 011_sp_Products_Insert.sql
-- Descripción: Stored Procedure para insertar un nuevo producto
-- =====================================================

USE DistribuidoraDb;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    -- Verificar si el SP ya existe
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA = 'dbo' AND ROUTINE_NAME = 'sp_Products_Insert')
    BEGIN
        EXEC('
            CREATE PROCEDURE dbo.sp_Products_Insert
                @Code NVARCHAR(100),
                @Name NVARCHAR(255),
                @ProductTypeId INT,
                @Price DECIMAL(10, 2) = NULL,
                @Active BIT = 1,
                @NewId INT OUTPUT
            AS
            BEGIN
                BEGIN TRY
                    BEGIN TRANSACTION;
                    
                    INSERT INTO dbo.Products (Code, Name, ProductTypeId, Price, Active, CreatedAt, UpdatedAt)
                    VALUES (@Code, @Name, @ProductTypeId, @Price, @Active, GETUTCDATE(), GETUTCDATE());
                    
                    SET @NewId = SCOPE_IDENTITY();
                    
                    COMMIT TRANSACTION;
                END TRY
                BEGIN CATCH
                    ROLLBACK TRANSACTION;
                    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
                    RAISERROR (@ErrorMessage, 16, 1);
                END CATCH;
            END
        ');
        PRINT 'Stored Procedure sp_Products_Insert creado exitosamente.';
    END
    ELSE
    BEGIN
        PRINT 'El Stored Procedure sp_Products_Insert ya existe.';
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

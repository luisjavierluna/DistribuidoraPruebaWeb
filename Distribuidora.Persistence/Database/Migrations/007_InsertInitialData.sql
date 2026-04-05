-- Script: 007_InsertInitialData.sql
-- Descripción: Inserta datos iniciales de prueba para tipos de productos, proveedores y productos
-- Fecha: 2026-04-05

BEGIN TRANSACTION

BEGIN TRY
    -- Insertar Tipos de Productos iniciales
    INSERT INTO dbo.ProductTypes (Name, Description, Active, CreatedAt, UpdatedAt)
    VALUES 
        ('Limpieza', 'Productos de limpieza del hogar', 1, GETUTCDATE(), GETUTCDATE()),
        ('Higiene Personal', 'Productos de higiene y cuidado personal', 1, GETUTCDATE(), GETUTCDATE()),
        ('Bebidas', 'Bebidas y refrescos', 1, GETUTCDATE(), GETUTCDATE()),
        ('Alimentos', 'Alimentos no perecederos', 1, GETUTCDATE(), GETUTCDATE()),
        ('Cuidado del Hogar', 'Productos para el cuidado del hogar', 1, GETUTCDATE(), GETUTCDATE());

    -- Insertar Proveedores iniciales
    INSERT INTO dbo.Suppliers (Name, Description, Active, CreatedAt, UpdatedAt)
    VALUES 
        ('Distribuidor Mexico', 'Distribuidor mayorista de productos de consumo', 1, GETUTCDATE(), GETUTCDATE()),
        ('Abarrottes a Granel Ruiz', 'Venta de productos a granel y mayoreo', 1, GETUTCDATE(), GETUTCDATE()),
        ('Surtidora La Morena', 'Distribuidora de artículos de consumo', 1, GETUTCDATE(), GETUTCDATE()),
        ('Casa Comercial del Valle', 'Mayorista de productos diversos', 1, GETUTCDATE(), GETUTCDATE()),
        ('Importadora Global', 'Importación y distribución de productos internacionales', 1, GETUTCDATE(), GETUTCDATE());

    -- Insertar Productos iniciales
    INSERT INTO dbo.Products (Code, Name, ProductTypeId, Price, Active, CreatedAt, UpdatedAt)
    VALUES 
        ('PIN0259', 'Pinol Limpiador 360 ml', 1, 20.50, 1, GETUTCDATE(), GETUTCDATE()),
        ('PIN0152', 'Pinol Limpiador 250 ml', 1, 15.75, 1, GETUTCDATE(), GETUTCDATE()),
        ('JBC001', 'Jabón en barra', 2, 5.50, 1, GETUTCDATE(), GETUTCDATE()),
        ('JAB0500', 'Jabón líquido 500 ml', 2, 8.99, 1, GETUTCDATE(), GETUTCDATE()),
        ('REF0001', 'Refresco Cola 2L', 3, 12.50, 1, GETUTCDATE(), GETUTCDATE()),
        ('REF0002', 'Refresco Naranja 2L', 3, 12.50, 1, GETUTCDATE(), GETUTCDATE()),
        ('ALI0001', 'Arroz 5 kg', 4, 45.00, 1, GETUTCDATE(), GETUTCDATE()),
        ('ALI0002', 'Frijoles 1 kg', 4, 18.50, 1, GETUTCDATE(), GETUTCDATE()),
        ('CAS0001', 'Papel higiénico 12 rollos', 5, 25.00, 1, GETUTCDATE(), GETUTCDATE()),
        ('CAS0002', 'Servilletas pack 40', 5, 14.99, 1, GETUTCDATE(), GETUTCDATE());

    -- Insertar Relaciones Producto-Proveedor
    -- Nota: Los IDs se asignan automáticamente, estos valores son referenciales
    INSERT INTO dbo.ProductSuppliers (ProductId, SupplierId, SupplierProductCode, Cost, Active, CreatedAt, UpdatedAt)
    VALUES 
        -- Pinol 360ml (ProductId=1)
        (1, 1, 'PINOL360', 18.00, 1, GETUTCDATE(), GETUTCDATE()),
        (1, 2, 'P123450', 18.50, 1, GETUTCDATE(), GETUTCDATE()),
        (1, 3, 'LIMP-PINOL01', 17.80, 1, GETUTCDATE(), GETUTCDATE()),
        
        -- Pinol 250ml (ProductId=2)
        (2, 1, 'PINOL250', 14.00, 1, GETUTCDATE(), GETUTCDATE()),
        (2, 4, 'PINOL-SMV250', 14.50, 1, GETUTCDATE(), GETUTCDATE()),
        
        -- Jabón en barra (ProductId=3)
        (3, 2, 'JAB001', 5.00, 1, GETUTCDATE(), GETUTCDATE()),
        (3, 5, 'JAB-IG001', 4.80, 1, GETUTCDATE(), GETUTCDATE()),
        
        -- Jabón líquido (ProductId=4)
        (4, 1, 'JABL500-DM', 7.50, 1, GETUTCDATE(), GETUTCDATE()),
        (4, 3, 'JABL-MORENA', 7.99, 1, GETUTCDATE(), GETUTCDATE()),
        
        -- Refresco Cola (ProductId=5)
        (5, 1, 'REF-COLA-2L', 10.00, 1, GETUTCDATE(), GETUTCDATE()),
        (5, 4, 'COLA-CCV', 10.50, 1, GETUTCDATE(), GETUTCDATE()),
        
        -- Refresco Naranja (ProductId=6)
        (6, 1, 'REF-NARA-2L', 10.00, 1, GETUTCDATE(), GETUTCDATE()),
        (6, 5, 'NARA-IG2L', 10.75, 1, GETUTCDATE(), GETUTCDATE()),
        
        -- Arroz 5kg (ProductId=7)
        (7, 2, 'ARROZ5K-AGR', 40.00, 1, GETUTCDATE(), GETUTCDATE()),
        (7, 4, 'ARROZ-CCV5K', 41.00, 1, GETUTCDATE(), GETUTCDATE()),
        
        -- Frijoles 1kg (ProductId=8)
        (8, 3, 'FRJ1K-MORENA', 17.00, 1, GETUTCDATE(), GETUTCDATE()),
        (8, 5, 'FRIJOL-IG1K', 17.50, 1, GETUTCDATE(), GETUTCDATE()),
        
        -- Papel higiénico (ProductId=9)
        (9, 1, 'PAPEL12-DM', 22.00, 1, GETUTCDATE(), GETUTCDATE()),
        (9, 2, 'PAPEL12-AGR', 23.00, 1, GETUTCDATE(), GETUTCDATE()),
        
        -- Servilletas (ProductId=10)
        (10, 3, 'SERV40-MORENA', 12.50, 1, GETUTCDATE(), GETUTCDATE()),
        (10, 4, 'SERV40-CCV', 13.00, 1, GETUTCDATE(), GETUTCDATE());

    COMMIT TRANSACTION
    
    PRINT '✓ Datos iniciales insertados exitosamente'
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION
    PRINT '✗ Error insertando datos iniciales:'
    PRINT ERROR_MESSAGE()
    THROW
END CATCH


# Distribuidora - Aplicación de Gestión de Productos y Proveedores

Aplicación ASP.NET Core MVC para gestionar productos, tipos de productos, y sus relaciones con proveedores. La aplicación implementa una arquitectura limpia (Clean Architecture) con capas separadas de Domain, Application, Persistence y Web.

---

## 📋 Requisitos Previos

- **Visual Studio 2022** o superior (Community, Professional, Enterprise)
- **.NET 9.0 SDK** instalado
- **SQL Server 2019** o superior (Express, Standard, Enterprise)
- **Acceso de administrador** a SQL Server

---

## ⚙️ Configuración Inicial

### 1. Configurar la Cadena de Conexión (DefaultConnection)

La aplicación obtiene la cadena de conexión del archivo `appsettings.json` ubicado en la carpeta `WebAppMVC/`.

#### Opción A: Autenticación de Windows (Recomendado para desarrollo local)

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.\\SQLEXPRESS;Database=DistribuidoraDb;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;Connection Timeout=15;"
}
```

**Componentes:**
- `Server=.\\SQLEXPRESS` - Instancia local de SQL Server Express
- `Database=DistribuidoraDb` - Nombre de la base de datos (se crea automáticamente)
- `Integrated Security=True` - Usa credenciales de Windows
- `Encrypt=False` - Sin encriptación (solo desarrollo local)
- `TrustServerCertificate=True` - Confía en certificados autofirmados

#### Opción B: Autenticación SQL Server (Usuario y Contraseña)

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=SERVER-NAME;Database=DistribuidoraDb;User Id=sa;Password=TuContraseña;Encrypt=False;TrustServerCertificate=True;Connection Timeout=15;"
}
```

**Componentes:**
- `Server=SERVER-NAME` - Nombre del servidor SQL (ej: `LAPTOP-ABC123` o `192.168.1.100`)
- `User Id=sa` - Usuario SQL Server (ej: `sa`, `admin`, etc.)
- `Password=TuContraseña` - Contraseña del usuario
- Resto igual que la opción A

#### Opción C: SQL Server en Red

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=192.168.1.100,1433;Database=DistribuidoraDb;User Id=sa;Password=TuContraseña;Encrypt=False;TrustServerCertificate=True;Connection Timeout=15;"
}
```

**Cambios principales:**
- `Server=IP,PUERTO` - Especificar IP y puerto del servidor SQL
- Usuario y contraseña (requerido para conexiones remotas)

---

## 🚀 Iniciando la Aplicación

### Pasos en Visual Studio

1. **Abre Visual Studio** y carga la solución:
   ```
   Archivo → Abrir → Proyecto/Solución → DistribuidoraPruebaWeb.sln
   ```

2. **Verifica la cadena de conexión** en:
   ```
   WebAppMVC/appsettings.json
   ```

3. **Establece WebAppMVC como proyecto de inicio:**
   - Haz clic derecho en el proyecto `WebAppMVC` en el Explorador de soluciones
   - Selecciona "Establecer como proyecto de inicio"

4. **Inicia la aplicación:**
   - Presiona `F5` o haz clic en el botón ▶ "IIS Express" (verde)
   - La aplicación se abrirá en tu navegador predeterminado

### Puertos de Desarrollo

- **HTTPS:** https://localhost:7240
- **HTTP:** http://localhost:5184

---

## ✅ Validación de Migraciones

Al iniciar la aplicación, el sistema ejecuta automáticamente los **17 scripts SQL** para:

1. Crear la base de datos (si no existe)
2. Crear todas las tablas
3. Crear todos los procedimientos almacenados
4. Insertar datos iniciales de prueba

### Consola Esperada al Iniciar

Debes ver en la consola de Visual Studio (output):

```
============================================================
RESULTADO DE MIGRACIONES DE BASE DE DATOS:
============================================================
Éxito: True
Scripts ejecutados: 17
Scripts fallidos: 0
Mensaje: V Migraciones completadas. Scripts ejecutados: 17, Errores: 0
============================================================

info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7240
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5184
```

### Si Algo Falla

Si ves `Éxito: False`, verifica:

1. **Cadena de conexión**: Asegúrate de que apunta al servidor SQL correcto
2. **Permisos SQL**: El usuario debe tener permisos para crear bases de datos
3. **SQL Server ejecutándose**: Comprueba que el servicio SQL Server está inicializado
4. **Puertos**: Si usas una instancia nombrada, asegúrate de especificar el puerto correcto

---

## 🎯 Features Disponibles

Una vez que las migraciones se ejecuten correctamente, la aplicación está lista para usar:

### 1. **Gestión de Productos**
- **Crear** nuevos productos con código, nombre, tipo y precio
- **Editar** productos existentes
- **Eliminar** productos (borrado lógico)
- **Ver** lista completa de productos

### 2. **Gestión de Proveedores**
- **Asignar** proveedores a productos
- **Editar** relación producto-proveedor (clave de producto, costo)
- **Eliminar** proveedores de un producto
- **Filtrar** productos por tipo

### 3. **Datos Iniciales**
La base de datos se popula automáticamente con:
- **5 Tipos de Producto**: Limpieza, Higiene Personal, Bebidas, Alimentos, Cuidado del Hogar
- **5 Proveedores**: Distribuidor Mexico, Abarrottes a Granel Ruiz, Surtidora La Morena, Casa Comercial del Valle, Importadora Global
- **10 Productos**: Diversos productos de prueba con precios y asociaciones
- **21 Relaciones Producto-Proveedor**: Múltiples proveedores por producto

---

## 📁 Estructura del Proyecto

```
DistribuidoraPruebaWeb/
├── Distribuidora.Domain/              # Entidades del dominio
│   └── Entities/
├── Distribuidora.Application/         # Servicios de aplicación
│   ├── Services/
│   ├── DTOs/
│   └── Interfaces/
├── Distribuidora.Persistence/         # Acceso a datos
│   ├── Repositories/
│   ├── Data/
│   └── Database/
│       ├── Migrations/                # Scripts SQL de migración
│       └── StoredProcedures/         # Procedimientos almacenados
└── WebAppMVC/                         # Presentación (MVC)
    ├── Controllers/
    ├── Views/
    └── Models/
```

---

## 🔧 Solución de Problemas

### Error: "Cannot open database"
**Solución:** Verifica que la cadena de conexión apunta al servidor SQL correcto.

### Error: "Conversion failed when converting"
**Solución:** Asegúrate de que SQL Server tiene la configuración regional correcta para números decimales.

### Error: "Login failed for user"
**Solución:** Verifica usuario y contraseña en la cadena de conexión, o usa autenticación de Windows (`Integrated Security=True`).

### La aplicación carga pero no muestra productos
**Solución:** Verifica que las migraciones se ejecutaron correctamente (revisa la consola al iniciar).

---

## 📝 Notas Importantes

- ⚠️ **No elimines manualmente** los archivos en `Distribuidora.Persistence/Database/Migrations/`
- ⚠️ **No modificar** los scripts SQL de migración una vez ejecutados
- ℹ️ Los productos se eliminan con lógica soft-delete (no se borran físicamente)
- ℹ️ Todos los campos de timestamp (`CreatedAt`, `UpdatedAt`) se actualizan automáticamente

---

## 🌐 Desplegar en Otra Computadora

### ✅ Sí Funcionará Si:

1. **Visual Studio 2022 o superior** está instalado
2. **.NET 9.0 SDK** está instalado
3. **SQL Server 2019+** está configurado y ejecutándose
4. **La cadena de conexión** en `appsettings.json` es correcta
5. **El usuario SQL** tiene permisos para crear bases de datos y ejecutar scripts

### ⚠️ Pasos Recomendados:

1. Clona el repositorio
2. Abre `WebAppMVC/appsettings.json`
3. **Actualiza la cadena de conexión** con los datos del servidor SQL de la nueva computadora
4. Presiona `F5` para iniciar

**Ejemplo de cambio necesario:**

```json
// Computadora Original
"DefaultConnection": "Server=.\\SQLEXPRESS;Database=DistribuidoraDb;..."

// Nueva Computadora (mismo formato, solo cambiar servidor si es necesario)
"DefaultConnection": "Server=.\\SQLEXPRESS;Database=DistribuidoraDb;..."
// O si es instancia nombrada diferente:
"DefaultConnection": "Server=.\\SQLSERVER2022;Database=DistribuidoraDb;..."
```

**La aplicación debería funcionar sin problemas adicionales** siempre y cuando:
- ✅ Los prerrequisitos estén instalados
- ✅ SQL Server esté ejecutándose
- ✅ La cadena de conexión sea válida

---

## 📞 Soporte

Si tienes problemas:

1. Verifica la consola de Visual Studio: ves el mensaje de éxito de migraciones
2. Abre SQL Server Management Studio y verifica que la BD `DistribuidoraDb` existe
3. Revisa que `appsettings.json` tiene la configuración correcta
4. Limpia la solución: `Build → Clean Solution` y recompila

---

**Versión:** 1.0  
**Última actualización:** 2026-04-05  
**Tecnología:** ASP.NET Core 9.0 MVC + SQL Server + Clean Architecture

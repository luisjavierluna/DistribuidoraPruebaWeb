# Distribuidora - Práctica Web de .Net (C#)

Prueba de aplicación ASP.NET Core MVC para gestionar productos. La aplicación implementa una arquitectura limpia (Clean Architecture) con capas separadas de Domain, Application, Persistence y Web.

---
## 📋 Entregables

Este repositorio incluye los siguientes entregables solicitados:
- Código Fuente
- Script de la BD (Full): se trata de varios scritps dentro de este mismo repo y rama. La ruta es DistribuidoraPruebaWeb/Distribuidora.Persistence/Database/ dentro las carpetas Migrations y StoredProcedures tienen los scripts, estos se ejecutan automaticamente al iniciar la API.
- Manual de usuario sencillo. Se muestra más abajo en este mismo readme
- Carpeta de publicados para IIS7.5 o superior: agrego link de onedrive para descargar la carpeta comprimida en zip: https://1drv.ms/u/c/fcffb5c52c681bf2/IQAZUtnDHmbdQ5d-33tDiEloAXACcUCyPxPb5xFcoth17hQ?e=crvARr

El repositorio no incluye lo siguiente:
- Manual de Instalación sobre IIS 7.5 o superior: la razón es que no se tiene experiencia instalando sobre IIS, sólo se tiene experiencia desplegando en Azure.

---

## 📋 Requisitos Previos

- **Visual Studio 2022** o superior (Community, Professional, Enterprise)
- **.NET 9.0 SDK** instalado
- **SQL Server 2019** o superior (Express, Standard, Enterprise)
- **Acceso de administrador** a SQL Server

---

## ⚙️ Manual de Usuario

### 1. Clonar repositorio
1. **Ubicarse en el repositorio en Github**
<img width="1918" height="737" alt="imagen" src="https://github.com/user-attachments/assets/92529d0e-e609-467e-80a8-00fe784a3863" />

2. **Clonar repositorio localmente** en la carpeta que se considere conveniente usando una terminar como git bash, para clonar usar "git clone https://github.com/luisjavierluna/DistribuidoraPruebaWeb.git" como se muestra en las imágenes siguientes:
<img width="1918" height="718" alt="imagen" src="https://github.com/user-attachments/assets/f7630bb9-eb29-447b-a85e-616d6b21ee13" />
<img width="1924" height="320" alt="imagen" src="https://github.com/user-attachments/assets/94425743-3bc7-4733-9a02-f415b2f7d79b" />

3. **Abrir la solución** usando Visual Studio 2022 o superior y configurar el proyecto de WebAppMVC como
<img width="1602" height="638" alt="imagen" src="https://github.com/user-attachments/assets/f6f94412-22cc-481c-8e88-cc0b16ddc3e1" />
<img width="3011" height="1638" alt="imagen" src="https://github.com/user-attachments/assets/4f9fefcb-1977-4d38-99d9-8d366ad481bc" />
<img width="958" height="662" alt="imagen" src="https://github.com/user-attachments/assets/be7cf449-dec7-42ec-84ff-f47ef2a20af0" />


### 2. Configurar la Cadena de Conexión (DefaultConnection)

Es necesario crear los archivos appsettings.json y appsettings.Development.json, en estos se crea la cadena de conexión con el nombre "DefaultConnection". Los archivos no vienen incluidos ya que al contener usualmente variables de entorno no se deben agregar al respositorio. Dependiendo como tengas configurado SQL server la conexión puede quedar de las siguientes formas:

  **Componentes de la cadena:**
- `Server=.\\SQLEXPRESS` - Instancia local de SQL Server Express
- `Database=DistribuidoraDb` - Nombre de la base de datos (se crea automáticamente)
- `Integrated Security=True` - Usa credenciales de Windows
- `Encrypt=False` - Sin encriptación (solo desarrollo local)
- `TrustServerCertificate=True` - Confía en certificados autofirmados

1. Sin contraseña: "DefaultConnection": "Server=.\\SQLEXPRESS;Database=DistribuidoraDb;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;Connection Timeout=15;"

2. Con contraseña: "DefaultConnection": "Server=SERVER-NAME;Database=DistribuidoraDb;User Id=sa;Password=TuContraseña;Encrypt=False;TrustServerCertificate=True;Connection Timeout=15;"

<img width="572" height="897" alt="imagen" src="https://github.com/user-attachments/assets/cca6eaa9-17e0-40f6-bee9-7464f0f46e19" />
<img width="1918" height="407" alt="imagen" src="https://github.com/user-attachments/assets/889739ea-23d5-4ea2-ab57-74df29cc5e1d" />


### 3. Iniciar aplicación
Dar click al ícnono verde de "Play" en Visual Studio para iniciar la aplicación localmente:
<img width="1918" height="973" alt="imagen" src="https://github.com/user-attachments/assets/7c38fe09-c7bf-4f7c-b048-dd91d0e64325" />

Al iniciar la aplicación sucederá lo siguiente:
1. Se ejecutarán los scripts que crean la base de datos, tablas, procedimientos almacenados, etc. Esto se puede ver en la consola como muestro a continuación:
<img width="1918" height="1020" alt="imagen" src="https://github.com/user-attachments/assets/c2a50f7c-bdc7-480d-bc98-7bc18b556534" />

2. En SQL Server también se puede ver lo que se creó (las tablas ya vienen con datos de prueba):
<img width="581" height="755" alt="imagen" src="https://github.com/user-attachments/assets/be8c6d25-ff21-422f-89d8-11df2fd87599" />
<img width="1331" height="793" alt="imagen" src="https://github.com/user-attachments/assets/27d5ff38-3daf-40ce-b96d-834aebf45bed" />


### 4. Aplicación iniciada
Si todo funciona correctamente, puedes comenzar a probar la aplicación:
#### 🎯 Features Disponibles

1. **Gestión de Productos**
- **Crear** nuevos productos con código, nombre, tipo y precio
- **Editar** productos existentes
- **Eliminar** productos (borrado lógico)
- **Ver** lista completa de productos
- **Buscar/Filtrar** productos por tipo

2. **Gestión de Proveedores**
- **Asignar** proveedores a productos
- **Editar** relación producto-proveedor (clave de producto, costo)
- **Eliminar** proveedores de un producto

### 5. 🔧 Solución de Problemas

Si al iniciar la aplicación ves esto en consola ves `Éxito: False`, verifica lo siguiente:

1. **Cadena de conexión**: Asegúrate de que apunta al servidor SQL correcto
2. **Permisos SQL**: El usuario debe tener permisos para crear bases de datos
3. **SQL Server ejecutándose**: Comprueba que el servicio SQL Server está inicializado
4. **Puertos**: Si usas una instancia nombrada, asegúrate de especificar el puerto correcto

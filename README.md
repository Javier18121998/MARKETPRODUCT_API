# MARKETPRODUCT_API

## Descripción
**MARKETPRODUCT_API** es una API RESTful para la gestión de productos y la creación de órdenes en un mercado en línea. Incluye endpoints para crear nuevos productos y realizar pedidos basados en el nombre y tamaño del producto.

## Dependencias del Proyecto
Aquí se listan las dependencias clave del proyecto y sus versiones:

![Microsoft.AspNetCore.Authentication.JwtBearer](https://img.shields.io/badge/Microsoft.AspNetCore.Authentication.JwtBearer-8.0.10-blue)
![Microsoft.AspNetCore.Mvc](https://img.shields.io/badge/Microsoft.AspNetCore.Mvc-2.2.0-blue)
![Microsoft.AspNetCore.Mvc.Versioning](https://img.shields.io/badge/Microsoft.AspNetCore.Mvc.Versioning-5.1.0-blue)
![Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer](https://img.shields.io/badge/Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer-5.1.0-blue)
![Microsoft.EntityFrameworkCore](https://img.shields.io/badge/Microsoft.EntityFrameworkCore-8.0.10-blue)
![Microsoft.EntityFrameworkCore.SqlServer](https://img.shields.io/badge/Microsoft.EntityFrameworkCore.SqlServer-8.0.10-blue)
![Newtonsoft.Json](https://img.shields.io/badge/Newtonsoft.Json-13.0.3-blue)
![RabbitMQ.Client](https://img.shields.io/badge/RabbitMQ.Client-6.8.1-blue)
![Swashbuckle.AspNetCore](https://img.shields.io/badge/Swashbuckle.AspNetCore-6.4.0-blue)
![Swashbuckle.AspNetCore.Annotations](https://img.shields.io/badge/Swashbuckle.AspNetCore.Annotations-6.9.0-blue)
![System.IdentityModel.Tokens.Jwt](https://img.shields.io/badge/System.IdentityModel.Tokens.Jwt-8.1.2-blue)

---

## Tabla de Contenidos
- [Descripción](#descripción)
- [Dependencias del Proyecto](#dependencias-del-proyecto)
- [Instalación](#instalación)
- [API Endpoints](#api-endpoints)
  - [Crear un Nuevo Producto](#crear-un-nuevo-producto)
  - [Crear Orden por Nombre y Tamaño del Producto](#crear-orden-por-nombre-y-tamaño-del-producto)
- [Contribución](#contribución)
- [Licencia](#licencia)
---
## Instalation
Configuration insider locally, follow the next steps:
1. Clone the repository:
   ```bash
   git clone https://github.com/username/MARKETPRODUCT_API.git
   ```
2. Surface to the dir:
   ```bash
   cd MARKETPRODUCT_API
   ```
3. Restore the packages:
   ```bash
   dotnet restore
   ```
4. Run the project
   ```bash
   dotnet run
   ```
---
## API Endpoints 📌

### Create Product wih Name and Size

- **Ruta:** `api/v1/Products`  
- **Methood:** `POST`  
- **Autorización:** 🔒 Requiere `Bearer Token`

**Description:**  
Permite crear un nuevo producto y almacenarlo en la base de datos.

#### Sended Request
La solicitud espera un objeto JSON con los siguientes campos:

```json
{
  "name": "Nombre del Producto",
  "description": "Descripción del Producto",
  "price": 99.99,
  "size": "Tamaño del Producto"
}
```
### Create Order by Product Name and Size

- **Route:** `api/v1/Orders`
- **Methood:** `POST`
- **Autorización:** 🔒 Requiere `Bearer Token`

**Description:**  
Permite crear un nuevo producto y almacenarlo en la base de datos.

#### Sended Request
La solicitud espera un objeto JSON con los siguientes campos:

```json
{
  "ProductName": "Nombre del Producto",
  "ProductSize": "Tamaño del Producto",
  "Quantity": 35
}
```

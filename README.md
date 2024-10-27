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

## API Endpoints 📌

### Crear un Nuevo Producto

**URL:** `/api/productos`  
**Método:** `POST`  
**Autorización:** 🔒 `Bearer Token` (Requiere autorización)

**Descripción:**  
Este endpoint permite crear un nuevo producto en el sistema y almacenarlo en la base de datos.

---

#### Esquema de Solicitud

```json
{
  "name": "Nombre del Producto",
  "description": "Descripción del Producto",
  "price": 99.99,
  "size": "Tamaño del Producto"
}
```

### Create Order by Product Name and Size

- **Endpoint**: `POST /CreateByNameAndSize`
- **Authorization**: Required

#### Description

Crea una nueva orden utilizando el nombre y el tamaño de un producto. Este endpoint permite a los usuarios especificar un producto y su tamaño para realizar un pedido.

#### Request Body

La solicitud debe incluir un objeto JSON con los siguientes parámetros:

- `ProductName` (string): El nombre del producto que se desea ordenar.
- `ProductSize` (string): El tamaño del producto que se desea ordenar.
- `Quantity` (int): La cantidad del producto que se desea ordenar.

**Ejemplo de cuerpo de solicitud:**

```json
{
  "ProductName": "Camisa",
  "ProductSize": "M",
  "Quantity": 2
}
```

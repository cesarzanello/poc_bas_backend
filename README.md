# POC BAS - Frontend y Backend

## Descripción

POC BAS es una prueba de concepto desarrollada para demostrar una arquitectura multi-tenant utilizando tecnologías modernas de frontend y backend, comunicación en tiempo real mediante SignalR y despliegue basado en contenedores Docker.

La solución permite gestionar productos asociados a distintos tenants, garantizando el aislamiento de datos y notificaciones entre clientes.

---

## Tecnologías Utilizadas

* .NET 10
* Angular 18
* MongoDB
* SQL Server
* SignalR
* Docker
* Docker Compose
* GitHub Actions

---

## Prerrequisitos

* Docker Desktop instalado y funcionando.
* Docker Compose habilitado.

---

## Instalación

Para implementar los proyectos **poc_bas_backend** y **poc_bas_frontend**, seguir los siguientes pasos:

1. Clonar ambos repositorios dentro de la misma carpeta raíz.

Ejemplo:

```text
/repos
├── poc_bas_backend
└── poc_bas_frontend
```

2. Abrir una terminal y posicionarse dentro del repositorio `poc_bas_backend`.

```bash
cd /repos/poc_bas_backend
```

3. Ejecutar el siguiente comando:

```bash
docker compose up --build -d
```

Este proceso realizará las siguientes acciones:

* Creará los contenedores necesarios para la solución.
* Levantará una instancia de MongoDB.
* Levantará una instancia de SQL Server.
* Creará los volúmenes persistentes para ambas bases de datos.
* Construirá y desplegará las imágenes del Backend y Frontend.

---

## Ejecución

Si es la primera vez que se ejecuta la solución, el sistema inicializará automáticamente dos tenants de ejemplo dentro de MongoDB para facilitar las pruebas de la POC.

### Acceso a la aplicación

Frontend:

```text
http://localhost:4200/login
```

Swagger del Backend:

```text
http://localhost:8080/swagger
```

---

## Funcionalidades Implementadas

### Gestión de Productos

* Alta de productos.
* Modificación de productos.
* Eliminación de productos.
* Consulta de productos por tenant.

### Multi-Tenant

* Administración de múltiples tenants.
* Aislamiento lógico de información por tenant.
* Filtrado automático de información utilizando `TenantId`.

### Comunicación en Tiempo Real

* Integración con SignalR.
* Notificaciones automáticas ante operaciones CRUD.
* Grupos de conexiones por tenant.
* Distribución de eventos únicamente a los usuarios del tenant correspondiente.

---

## Arquitectura y Persistencia

### MongoDB

MongoDB se utiliza para almacenar la información de los tenants y simular un proveedor de identidad similar a Azure AD.

### SQL Server

SQL Server se utiliza para almacenar la información transaccional de la aplicación, incluyendo los productos gestionados por cada tenant.

### Multi-Tenant

Cada producto se encuentra asociado a un `TenantId`, permitiendo aislar la información entre distintos clientes dentro de la misma aplicación.

### SignalR

La comunicación en tiempo real mediante SignalR utiliza grupos basados en `TenantId`.

De esta forma:

* Cada cliente se conecta al grupo correspondiente a su tenant.
* Los mensajes son enviados únicamente a los usuarios pertenecientes a dicho tenant.
* Se garantiza el aislamiento de las notificaciones entre clientes.

---

## Datos Iniciales

Durante la primera ejecución se crean automáticamente tenants de ejemplo para facilitar las pruebas de la solución.

Esto permite comenzar a utilizar la aplicación sin necesidad de realizar configuraciones manuales adicionales.

---

## Proceso CI/CD

Las imágenes Docker se publican automáticamente en Docker Hub mediante GitHub Actions.

### Imágenes Docker

```bash
docker pull cesarzanello/mi-back:<tag>
docker pull cesarzanello/mi-front:<tag>
```

### Pipeline Automatizado

Los workflows de CI/CD se ejecutan automáticamente cuando se realiza un merge sobre la rama `main`.

El pipeline realiza las siguientes tareas:

1. Descarga el código fuente del repositorio.
2. Se autentica contra Docker Hub utilizando credenciales almacenadas en GitHub Secrets.
3. Construye la imagen Docker utilizando el Dockerfile del proyecto.
4. Publica la imagen en Docker Hub.
5. Genera dos tags para la imagen:
   - `latest`
   - Hash único del commit (`github.sha`)

Esto permite identificar tanto la última versión disponible como una versión específica asociada a un commit determinado.

---

## Consideraciones

Esta solución fue desarrollada como una prueba de concepto orientada a demostrar:

* Arquitectura multi-tenant.
* Arquitectura backend hexagonal CQRS (Dapper - Entity Framework)
* Arquitectura frontend limpia para mantener una clara separación entre la lógica de presentación, los casos de uso y el acceso a datos. 
* Comunicación en tiempo real mediante SignalR.
* Separación de responsabilidades entre servicios.
* Persistencia híbrida utilizando MongoDB y SQL Server.
* Automatización de despliegues mediante CI/CD.
* Contenerización completa utilizando Docker.

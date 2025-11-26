
---

# 🚀 Instrucciones de Instalación y Ejecución

## Requisitos Previos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022, VS Code o cualquier editor compatible con .NET

## Pasos para Ejecutar

1. **Clonar el repositorio** (si aplica)
   ```bash
   git clone <url-del-repositorio>
   cd EvaluacionDotnet
   ```

2. **Navegar al proyecto API**
   ```bash
   cd API/API
   ```

3. **Restaurar dependencias**
   ```bash
   dotnet restore
   ```

4. **Compilar el proyecto**
   ```bash
   dotnet build
   ```

5. **Ejecutar la aplicación**
   ```bash
   dotnet run
   ```

6. **Acceder a Swagger**
   - La aplicación se ejecutará en `https://localhost:7034` (o puerto configurado)
   - Abrir en el navegador: `https://localhost:7034/swagger`

---

# Guía de Prueba de Endpoints

## Usando Swagger UI

Swagger está configurado y disponible en `/swagger` cuando la aplicación está ejecutándose.

### Flujo de Prueba:

#### 1. **Registrar un Usuario** (`POST /api/users/register`)

1. Busca el endpoint `POST /api/users/register`
2. Haz clic en **"Try it out"**
3. Ingresa el siguiente JSON en el body:
```json
{
  "name": "Raudo Moquete",
  "email": "raudo.moquete@example.com",
  "password": "SecurePass123!"
}
```
> **Nota sobre la contraseña:** Debe contener mayúsculas, minúsculas, números, símbolos y tener más de 8 caracteres.

4. Haz clic en **"Execute"**
5. **Copia el `token`** de la respuesta - lo necesitarás para los endpoints protegidos

#### 2. **Autenticar Usuario** (`POST /api/users/authenticate`)

1. Busca el endpoint `POST /api/users/authenticate`
2. Haz clic en **"Try it out"**
3. Ingresa el siguiente JSON:
```json
{
  "email": "raudo.moquete@example.com",
  "password": "SecurePass123!"
}
```
4. Haz clic en **"Execute"**
5. **Copia el `token`** de la respuesta

#### 3. **Configurar Autenticación JWT en Swagger**

1. En la parte superior de Swagger, haz clic en el botón **"Authorize"**
2. En el campo de texto, **pega SOLO el token** (sin "Bearer", sin comillas)
3. Haz clic en **"Authorize"** y luego **"Close"**
4. Ahora todos los endpoints protegidos tendrán el token configurado automáticamente

#### 4. **Obtener Posts** (`GET /api/posts`)

1. Busca el endpoint `GET /api/posts` (tiene un candado indicando que requiere autenticación)
2. Haz clic en **"Try it out"**
3. Haz clic en **"Execute"**
4. Deberías ver una lista de posts de la API externa

#### 5. **Crear Post** (`POST /api/posts`)

1. Busca el endpoint `POST /api/posts` (tiene un candado)
2. Haz clic en **"Try it out"**
3. Ingresa el siguiente JSON:
```json
{
  "userId": 1,
  "title": "Mi primer post",
  "body": "Este es el contenido de mi post"
}
```
4. Haz clic en **"Execute"**
5. Deberías ver el post creado con un ID generado

---

## Usando Postman

### Configuración Inicial

1. **Importar la colección** (opcional):
   - Puedes crear una nueva colección en Postman llamada "EvaluacionDotnet API"

2. **Configurar Variable de Entorno**:
   - Crea una variable `baseUrl` con valor: `https://localhost:7034`
   - Crea una variable `token` (se llenará automáticamente después de autenticarte)

### Endpoints Públicos (No requieren autenticación)

#### 1. Registrar Usuario
- **Método:** `POST`
- **URL:** `{{baseUrl}}/api/users/register`
- **Headers:**
  - `Content-Type: application/json`
- **Body (raw JSON):**
```json
{
  "name": "Raudo Moquete",
  "email": "raudo.moquete@example.com",
  "password": "SecurePass123!"
}
```
- **Response:** Copia el `token` del response y guárdalo en la variable `token`

#### 2. Autenticar Usuario
- **Método:** `POST`
- **URL:** `{{baseUrl}}/api/users/authenticate`
- **Headers:**
  - `Content-Type: application/json`
- **Body (raw JSON):**
```json
{
  "email": "raudo.moquete@example.com",
  "password": "SecurePass123!"
}
```
- **Response:** Copia el `token` del response

### Endpoints Protegidos (Requieren JWT)

#### 3. Obtener Posts
- **Método:** `GET`
- **URL:** `{{baseUrl}}/api/posts`
- **Headers:**
  - `Authorization: Bearer {{token}}`
  - `Content-Type: application/json`

#### 4. Crear Post
- **Método:** `POST`
- **URL:** `{{baseUrl}}/api/posts`
- **Headers:**
  - `Authorization: Bearer {{token}}`
  - `Content-Type: application/json`
- **Body (raw JSON):**
```json
{
  "userId": 1,
  "title": "Mi primer post",
  "body": "Este es el contenido de mi post"
}
```

### Script para Guardar Token Automáticamente en Postman

En Postman, puedes agregar el siguiente script en la pestaña **"Tests"** del request de autenticación o registro para guardar automáticamente el token:

```javascript
if (pm.response.code === 200) {
    var jsonData = pm.response.json();
    if (jsonData.token) {
        pm.environment.set("token", jsonData.token);
        console.log("Token guardado automáticamente");
    }
}
```

---

## Validaciones y Casos de Error

### Validaciones de Registro

| Caso | Request | Código HTTP Esperado |
|------|---------|---------------------|
| Email inválido | `{"name": "Test", "email": "invalid-email", "password": "SecurePass123!"}` | 400 |
| Contraseña débil | `{"name": "Test", "email": "test@example.com", "password": "123"}` | 400 |
| Nombre vacío | `{"name": "", "email": "test@example.com", "password": "SecurePass123!"}` | 400 |
| Email duplicado | Intentar registrar el mismo email dos veces | 409 |

### Casos de Autenticación

| Caso | Request | Código HTTP Esperado |
|------|---------|---------------------|
| Usuario no encontrado | `{"email": "nonexistent@example.com", "password": "Pass123!"}` | 404 |
| Contraseña incorrecta | Email válido pero contraseña incorrecta | 400 |

### Casos de Endpoints Protegidos

| Caso | Resultado |
|------|-----------|
| Sin token | HTTP 401 Unauthorized |
| Token inválido/expirado | HTTP 401 Unauthorized |
| Token válido | Operación exitosa |

---

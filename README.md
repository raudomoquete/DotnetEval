# Evaluación – Programador .NET

## Objetivo  
Desarrollar una aplicación API Restful.

## Requerimientos  
- Utilizar **.NET 8**.  
- La API debe de utilizar una **base de datos en memoria**. Es decir, toda información solo se conservará mientras la aplicación esté en ejecución. Puede revisar la siguiente documentación:  
	[Proveedor de base de datos InMemory - EF Core | Microsoft Learn](https://learn.microsoft.com/en-us/ef/core/providers/in-memory/)  

### Primera parte:  
- Debe crear un **endpoint** que reciba la siguiente estructura JSON y la guarde en una tabla.  
- Utilizar **BCrypt** para la encriptación de contraseñas. Puede revisar la siguiente librería: [BCrypt.Net-Next](https://www.nuget.org/packages/BCrypt.Net-Next)  
- El **endpoint de creación de usuarios** debe validar los siguientes datos y, en caso de que no se cumpla, retornar un error **HTTP 400** indicando el motivo:  
	- El campo de **nombre** no está vacío.  
	- Es un **correo válido** (utilizar una expresión regular).  
	- La **contraseña** utiliza mayúsculas, minúsculas, símbolos y tiene más de **8 caracteres** (utilizar una expresión regular).  
	- El **correo no se encuentre registrado**.  
- El endpoint debe retornar la siguiente información:  
	- **Nombre registrado**.  
	- **Correo registrado**.  
	- **Identificador único** (puede ser un GUID).  
	- **Token de acceso JWT** persistido por el identificador del usuario.  
		> **Nota:** La clave de encriptación del token debe estar presente en el archivo de configuración **appsettings.json**.  

- Debe crear otro **endpoint** para la **autenticación del usuario**.  
	- Este endpoint debe recibir un **correo y una contraseña**.  
	- En caso de ser un usuario registrado con su contraseña válida, debe retornar el **token JWT**.  

### Segunda parte:  
- El siguiente **endpoint** solo puede ser consumido utilizando el **token JWT** proporcionado al usuario al momento de la creación o inicio de sesión.  
	- En caso de que no se envíe un **token válido**, retornar un **HTTP 401**.  

- Debe crear un **endpoint** que lea información de la siguiente API y la muestre como respuesta:  
	- [`https://jsonplaceholder.typicode.com/posts`](https://jsonplaceholder.typicode.com/posts)  

- Debe crear un **endpoint** que inserte información en la siguiente API y muestre la respuesta:  
	- [`https://jsonplaceholder.typicode.com/posts`](https://jsonplaceholder.typicode.com/posts)  

## Opcional:  
- Utilizar **FluentValidation** para las validaciones de datos de entrada.  
- Utilizar un archivo de configuración (ej. **appsettings.json**) para leer las expresiones regulares a utilizar.  
- Implementar **Swagger**.  
- Crear **pruebas unitarias**.  

# 📌 Puntos a Evaluar – Evaluación Programador .NET  

## 1️⃣ **Calidad y limpieza del código**  
✅ Código bien estructurado, modular y reutilizable.  
✅ Nombres de variables, métodos y clases descriptivos.  
✅ Ausencia de código duplicado y comentarios innecesarios.  
✅ Uso adecuado de patrones de diseño si aplica.  

## 2️⃣ **Uso de buenas prácticas en .NET**  
✅ Uso de **Dependency Injection (DI)**.  
✅ Manejo adecuado de **configuración** en `appsettings.json`.  
✅ Manejo de excepciones a nivel global de la aplicación.
✅ Uso eficiente de **async/await** para operaciones asíncronas.  

## 3️⃣ **Seguridad**  
✅ Uso de **BCrypt** para encriptación de contraseñas.  
✅ Implementación correcta de **JWT** para autenticación.  
✅ Validación de datos con **expresiones regulares** y otras técnicas.  
✅ Protección contra ataques como **SQL Injection** o **XSS** (si aplica).  

## 4️⃣ **Uso de Entity Framework Core**  
✅ Uso de **InMemory Database** correctamente implementado.  
✅ Definición correcta de modelos y relaciones.  

## 5️⃣ **Endpoints y funcionalidad**  
✅ Validaciones adecuadas en la creación de usuarios.  
✅ Manejo correcto de errores con códigos HTTP adecuados.  
✅ Implementación de autenticación y autorización con JWT.  
✅ Consumo e integración correcta de la API externa `jsonplaceholder.typicode.com`.  

## 6️⃣ **Documentación y usabilidad**  
✅ Archivos README con instrucciones claras de instalación y ejecución.  
✅ Explicación de cómo probar los endpoints (Postman, Swagger, etc.).

## 8️⃣ **Extras (Opcionales pero valorados)**  
✅ Uso de **FluentValidation** para validar datos de entrada.  
✅ Implementación de **pruebas unitarias** con xUnit o NUnit.
✅ Uso de **Swagger** para documentar la API.  

📊 **Calificación final** basada en el cumplimiento de estos criterios. ¡Buena suerte! 🚀  
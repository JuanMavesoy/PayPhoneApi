# 📱 PayPhone API

API RESTful desarrollada en ASP.NET Core que permite la gestión de billeteras electrónicas, transferencias de saldo y visualización del historial de movimientos. Esta API también cuenta con autenticación basada en JWT para proteger los endpoints sensibles.

## 🛠️ Tecnologías Utilizadas

- ASP.NET Core 8
- Entity Framework Core (InMemory)
- Autenticación JWT
- Swagger / OpenAPI
- xUnit + Moq (Pruebas Unitarias)
- WebApplicationFactory (Pruebas de Integración)

## 📌 Características

- ✅ CRUD de billeteras electrónicas
- ✅ Transferencia de saldo entre billeteras
- ✅ Registro de movimientos (crédito / débito)
- ✅ Autenticación con JWT
- ✅ Control de acceso a endpoints protegidos
- ✅ Pruebas unitarias e integración
- ✅ Documentación Swagger/OpenAPI generada automáticamente

## 🔐 Autenticación y Autorización

Los endpoints protegidos requieren autenticación mediante JWT. Para obtener el token, se debe hacer un `POST` a `/Auth/login` con:

```json
{
  "username": "admin",
  "password": "secret123"
}
```

Luego, usar el token en el header:

```
Authorization: Bearer {token}
```
tener en cuenta que si se hace desde le swagger, en el campo de value de la autorizacion solo debes pegar el TOKEN, y el internamente le anexa el bearer automaticamente.
---

## 📂 Endpoints

### 🧾 Auth

| Método | Endpoint        | Descripción                         | Protección |
|--------|------------------|-------------------------------------|------------|
| POST   | `/Auth/login`    | Autenticación y generación de token | ❌ No      |

---

### 👛 Wallet

| Método | Endpoint                       | Descripción                             | Protección |
|--------|---------------------------------|-----------------------------------------|------------|
| GET    | `/Wallet`                      | Obtener todas las billeteras            | ❌ No     |
| GET    | `/Wallet/{id}`                 | Obtener billetera por ID                | ❌ No     |
| GET    | `/Wallet/{id}/with-movements`  | Obtener billetera con movimientos       | ❌ No     |
| POST   | `/Wallet`                      | Crear nueva billetera                   | ✅ Sí      |
| PUT    | `/Wallet/{id}`                 | Actualizar billetera                    | ✅ Sí      |
| DELETE | `/Wallet/{id}`                 | Eliminar billetera                      | ✅ Sí      |

---

### 💸 Transfer

| Método | Endpoint     | Descripción                               | Protección |
|--------|--------------|-------------------------------------------|------------|
| POST   | `/Transfer`  | Transferir saldo entre billeteras         | ✅ Sí      |

---

### 📑 MovementHistory

| Método | Endpoint                               | Descripción                                  | Protección |
|--------|-----------------------------------------|----------------------------------------------|------------|
| GET    | `/MovementHistory`                     | Obtener todos los movimientos                | ❌ No      |
| GET    | `/MovementHistory/wallet/{walletId}`   | Movimientos por billetera específica         | ❌ No      |
| POST   | `/MovementHistory`                     | Crear un movimiento manual (crédito/débito) | ✅ Sí      |

---

## 🧪 Pruebas

Este proyecto incluye:

- ✅ Pruebas Unitarias con `xUnit` y `Moq`
- ✅ Pruebas de Integración con `WebApplicationFactory`
- ✅ Cobertura de código verificada con Fine Code Coverage

---

## 🚀 Cómo ejecutar el proyecto

1. Clona el repositorio:
   ```bash
   git clone https://github.com/JuanMavesoy/PayPhoneApi.git
   ```

2. Abre la solución en Visual Studio.

3. Ejecuta el proyecto principal (`PayPhoneApi`) con F5 o desde la consola:
   ```bash
   dotnet run --project PayPhoneApi
   ```

4. Accede a Swagger para probar los endpoints:
   ```
   https://localhost:7152/swagger
   ```

---

## 📌 Usuarios por defecto (para autenticación)

| Usuario | Contraseña   | Rol   |
|---------|--------------|-------|
| admin   | secret123    | Admin |

---

## 🧾 Licencia

Este proyecto fue desarrollado como parte de una prueba técnica. Uso personal o académico permitido.

---

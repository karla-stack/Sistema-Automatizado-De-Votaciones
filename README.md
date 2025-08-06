# Sistema Automatizado de Votaciones (SADVO) 🗳️

[![.NET](https://img.shields.io/badge/.NET-8.0|9.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-MVC-512BD4?style=flat-square)](https://docs.microsoft.com/aspnet/core)
[![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Code%20First-512BD4?style=flat-square)](https://docs.microsoft.com/ef/)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-CSS%20Framework-7952B3?style=flat-square&logo=bootstrap)](https://getbootstrap.com/)

SADVO es una plataforma web completa de votación electrónica diseñada para gestionar de manera integral todo el ciclo electoral. El sistema permite desde el registro y validación segura de electores mediante autenticación con OCR de documentos, hasta la configuración y administración de procesos electorales, el despliegue de boletas con candidatos y partidos, y la emisión de votos de forma confidencial.

## ✨ Características Principales

### 🔐 Sistema de Autenticación y Validación
- **Validación de Identidad con OCR**: Utiliza Tesseract OCR para verificar documentos de identidad
- **Autenticación por roles**: Sistema multi-rol con perfiles de Administrador, Dirigente Político y Elector
- **Validación de ciudadanos**: Control del estado activo/inactivo de los electores para garantizar la participación válida

### 🏛️ Gestión Electoral Completa
- **Mantenimiento de Puestos Electivos**: Creación y administración de cargos políticos
- **Gestión de Partidos Políticos**: Registro y gestión completa de organizaciones políticas
- **Sistema de Candidatos**: Registro y asignación de candidatos a los diferentes puestos electivos
- **Alianzas Políticas**: Gestión de coaliciones y alianzas entre partidos para procesos electorales conjuntos

### 🗳️ Proceso de Votación
- **Votación Segura**: Validación de identidad antes y durante la emisión del voto
- **Boletas Electrónicas**: Presentación intuitiva y amigable de las opciones de voto para el elector
- **Prevención de Doble Voto**: Mecanismo estricto para evitar votaciones duplicadas

### 📊 Reportes y Resultados
- **Resultados en Tiempo Real**: Visualización dinámica de los resultados una vez concluida la votación
- **Reportes Estadísticos**: Análisis detallado de la participación y los resultados electorales
- **Histórico Electoral**: Consulta y archivo de elecciones pasadas para referencia futura

## 🏗️ Arquitectura del Sistema

El proyecto implementa la **Arquitectura Onion** con las siguientes capas:

```
SADVO/
├── SADVO.Core.Domain/             # Capa de Dominio
├── Sadvo.Core.Application/        # Interfaces y DTOs
├── Sadvo.Infrastructure.Persistence/  # Entity Framework, Repositorios
├── Sadvo.WebApp/                  # Capa de Presentación
└── Tests/                         # Pruebas unitarias
```

## 🛠️ Tecnologías Utilizadas

### Backend
- **Framework**: ASP.NET Core MVC (.NET 8/9)
- **ORM**: Entity Framework Core (Code First)
- **Patrón de diseño**: Repository Pattern con Repositorios Genéricos
- **Mapeo**: AutoMapper para transformación entre ViewModels, Entities y DTOs
- **OCR**: Tesseract OCR para procesamiento y validación de documentos de identidad

### Frontend
- **CSS Framework**: Bootstrap para diseño responsivo y adaptable
- **JavaScript**: Para funcionalidades interactivas y validaciones en cliente
- **Motor de plantillas**: Razor Views de ASP.NET Core para renderizado dinámico

### Base de Datos
- **Patrón**: Code First con Entity Framework Core
- **Migraciones**: Controladas automáticamente para gestión de versiones y evolución del esquema

## 📦 Instalación y Configuración

### Prerrequisitos
- .NET 8.0 o 9.0 SDK
- SQL Server o SQL Server LocalDB
- Visual Studio 2022 o VS Code
- Tesseract OCR

### Pasos de Instalación

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/karla-stack/Sistema-Automatizado-De-Votaciones.git
   cd Sistema-Automatizado-De-Votaciones
   ```

2. **Restaurar paquetes NuGet**
   ```bash
   dotnet restore
   ```

3. **Configurar la cadena de conexión**
   ```json
   // appsettings.json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SADVODB;Trusted_Connection=true;"
     }
   }
   ```

4. **Ejecutar migraciones**
   ```bash
   dotnet ef database update
   ```

5. **Ejecutar la aplicación**
   ```bash
   dotnet run --project Sadvo.WebApp
   ```

## 👥 Roles de Usuario

### 🔧 Administrador
- Gestión completa de puestos electivos
- Administración de ciudadanos y usuarios
- Control de partidos políticos
- Asignación de dirigentes políticos
- Creación y finalización de elecciones
- Visualización de reportes y resultados

### 🏛️ Dirigente Político
- Gestión de candidatos de su partido
- Creación y gestión de alianzas políticas
- Asignación de candidatos a puestos
- Dashboard con métricas del partido

### 🗳️ Elector
- Validación de identidad con documento
- Participación en proceso de votación
- Recepción de confirmación por email

## 🔒 Seguridad y Validaciones

### Control de Acceso
- **Autenticación requerida**: Todas las funciones administrativas requieren login
- **Autorización por roles**: Cada rol tiene acceso limitado a sus funciones
- **Validación de URLs**: Prevención de acceso no autorizado a rutas protegidas

### Integridad de Datos
- **Validación OCR**: Verificación automática de documentos de identidad
- **Prevención de duplicados**: Control de cédulas únicas y votos únicos
- **Estados de entidades**: Manejo de activación/desactivación lógica

## 📈 Funcionalidades Destacadas

### 🖼️ Procesamiento OCR
Sistema avanzado de reconocimiento óptico para validar documentos de identidad de forma automática y segura.

### 📧 Sistema de Notificaciones
- Confirmación automática de voto por email
- Resumen detallado de selecciones realizadas
- Integración con servicios SMTP

### 📊 Reportes Dinámicos
- Resultados por puesto electivo
- Porcentajes de participación
- Análisis histórico de elecciones

## 📋 Reglas de Negocio Importantes

1. **Unicidad de Documentos**: Cada ciudadano debe tener una cédula única
2. **Un Voto por Elector**: Sistema anti-fraude que previene votación múltiple
3. **Candidatos por Partido**: Los candidatos pertenecen a un partido específico
4. **Restricciones durante Elecciones**: Bloqueo de modificaciones durante votación activa
5. **Eliminación Lógica**: Todas las entidades manejan estados activo/inactivo
6. **Alianzas Políticas**: Control de solicitudes y aprobaciones entre partidos

## 🤝 Contribuciones

1. Fork del proyecto
2. Crear rama para nueva funcionalidad (`git checkout -b feature/nueva-funcionalidad`)
3. Commit de cambios (`git commit -am 'Agregar nueva funcionalidad'`)
4. Push a la rama (`git push origin feature/nueva-funcionalidad`)
5. Crear Pull Request

---

**Desarrollado con ❤️ para democratizar el proceso electoral**

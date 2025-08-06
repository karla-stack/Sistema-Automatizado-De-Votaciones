# Sistema-Automatizado-De-Votaciones

SADVO es una plataforma web completa de votación electrónica diseñada para gestionar de manera integral todo el ciclo electoral. El sistema permite desde el registro y validación segura de electores mediante autenticación con OCR de documentos, hasta la configuración y administración de procesos electorales, el despliegue de boletas con candidatos y partidos, y la emisión de votos de forma confidencial.

Características Principales

🔐 Sistema de Autenticación y Validación

Validación de Identidad con OCR: Utiliza Tesseract OCR para verificar documentos de identidad
Autenticación por roles: Sistema multi-rol (Administrador, Dirigente Político, Elector)
Validación de ciudadanos: Control de estado activo/inactivo de electores

🏛️ Gestión Electoral Completa

Mantenimiento de Puestos Electivos: Creación y gestión de cargos políticos
Gestión de Partidos Políticos: Administración completa de organizaciones políticas
Sistema de Candidatos: Registro y asignación de candidatos a puestos
Alianzas Políticas: Gestión de coaliciones entre partidos

🗳️ Proceso de Votación

Votación Segura: Sistema de votación con validación de identidad
Boletas Electrónicas: Visualización intuitiva de opciones de voto
Prevención de Doble Voto: Control estricto de participación única

📊 Reportes y Resultados

Resultados en Tiempo Real: Visualización de resultados una vez finalizada la elección
Reportes Estadísticos: Análisis detallado de participación y resultados
Histórico Electoral: Consulta de elecciones pasadas

🛠️ Tecnologías Utilizadas
Backend

Framework: ASP.NET Core MVC (.NET 8/9)
ORM: Entity Framework Core (Code First)
Patrón: Repository Pattern con Repositorios Genéricos
Mapeo: AutoMapper para ViewModels, Entities y DTOs
OCR: Tesseract OCR para procesamiento de documentos

Frontend

CSS Framework: Bootstrap para diseño responsivo
JavaScript: Para interactividad y validaciones del lado cliente
Razor Views: Motor de plantillas de ASP.NET Core

Base de Datos

Patrón: Code First con Entity Framework
Migraciones: Automáticas para control de versiones de BD

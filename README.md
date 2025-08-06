# Sistema-Automatizado-De-Votaciones (SADVO)

SADVO es una plataforma web completa de votación electrónica diseñada para gestionar de manera integral todo el ciclo electoral. El sistema permite desde el registro y validación segura de electores mediante autenticación con OCR de documentos, hasta la configuración y administración de procesos electorales, el despliegue de boletas con candidatos y partidos, y la emisión de votos de forma confidencial.

---

## Características Principales

### 🔐 Sistema de Autenticación y Validación
- **Validación de Identidad con OCR:** Utiliza Tesseract OCR para verificar documentos de identidad.
- **Autenticación por roles:** Sistema multi-rol con perfiles de Administrador, Dirigente Político y Elector.
- **Validación de ciudadanos:** Control del estado activo/inactivo de los electores para garantizar la participación válida.

---

### 🏛️ Gestión Electoral Completa
- **Mantenimiento de Puestos Electivos:** Creación y administración de cargos políticos.
- **Gestión de Partidos Políticos:** Registro y gestión completa de organizaciones políticas.
- **Sistema de Candidatos:** Registro y asignación de candidatos a los diferentes puestos electivos.
- **Alianzas Políticas:** Gestión de coaliciones y alianzas entre partidos para procesos electorales conjuntos.

---

### 🗳️ Proceso de Votación
- **Votación Segura:** Validación de identidad antes y durante la emisión del voto.
- **Boletas Electrónicas:** Presentación intuitiva y amigable de las opciones de voto para el elector.
- **Prevención de Doble Voto:** Mecanismo estricto para evitar votaciones duplicadas.

---

### 📊 Reportes y Resultados
- **Resultados en Tiempo Real:** Visualización dinámica de los resultados una vez concluida la votación.
- **Reportes Estadísticos:** Análisis detallado de la participación y los resultados electorales.
- **Histórico Electoral:** Consulta y archivo de elecciones pasadas para referencia futura.

---

## 🛠️ Tecnologías Utilizadas

### Backend
- **Framework:** ASP.NET Core MVC (.NET 8/9)
- **ORM:** Entity Framework Core (Code First)
- **Patrón de diseño:** Repository Pattern con Repositorios Genéricos
- **Mapeo:** AutoMapper para transformación entre ViewModels, Entities y DTOs
- **OCR:** Tesseract OCR para procesamiento y validación de documentos de identidad

### Frontend
- **CSS Framework:** Bootstrap para diseño responsivo y adaptable
- **JavaScript:** Para funcionalidades interactivas y validaciones en cliente
- **Motor de plantillas:** Razor Views de ASP.NET Core para renderizado dinámico

### Base de Datos
- **Patrón:** Code First con Entity Framework Core
- **Migraciones:** Controladas automáticamente para gestión de versiones y evolución del esquema

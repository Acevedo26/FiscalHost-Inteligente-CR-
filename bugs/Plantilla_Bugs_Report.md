# Plantilla de reporte de bugs – FiscalHost Inteligente Costa Rica

**Uso:** Completar un reporte por cada defecto encontrado durante la ejecución de los casos de prueba.  
Asignar ID secuencial (`BUG-001`, `BUG-002`, …) y referenciarlo en la matriz de casos (columna **Bug ID**).

---

## Identificación

| Campo | Valor |
|-------|-------|
| **ID del bug** | *[A RELLENAR – ej. BUG-001]* |
| **Título del error** | *[A RELLENAR – descripción corta y clara]* |
| **Caso de prueba relacionado** | *[A RELLENAR – ej. CP-004-03]* |
| **Fecha de reporte** | *[A RELLENAR – DD/MM/AAAA]* |
| **Reportado por** | *[A RELLENAR]* |
| **Asignado a** | *[A RELLENAR]* |

---

## Clasificación

| Campo | Valor |
|-------|-------|
| **Prioridad** | *[A RELLENAR – Crítica / Alta / Media / Baja]* |
| **Severidad** | *[A RELLENAR – Bloqueante / Mayor / Menor / Cosmético]* |
| **Estado** | Abierto |
| **Tipo** | *[A RELLENAR – Funcional / UI / Seguridad / Rendimiento / Datos / Integración / Otro]* |

> **Guía rápida de prioridad**  
> - **Crítica**: impide el uso principal del sistema o genera pérdida de datos.  
> - **Alta**: funcionalidad importante afectada, no hay workaround fácil.  
> - **Media**: funcionalidad secundaria o existe workaround.  
> - **Baja**: cosmético o impacto mínimo.

---

## Entorno

| Campo | Valor |
|-------|-------|
| **Medio ambiente** | *[A RELLENAR – Local / Staging / Producción de pruebas]* |
| **Sistema operativo** | *[A RELLENAR – ej. Windows 11, macOS 15, Ubuntu 24.04]* |
| **Navegador (y versión)** | *[A RELLENAR – ej. Chrome 129, Edge 129, Safari 18]* |
| **Versión de compilación / commit** | *[A RELLENAR]* |
| **Dispositivo / resolución** (si aplica) | *[A RELLENAR]* |

---

## Pasos para reproducir

1. *[A RELLENAR – Vaya a…]*  
2. *[A RELLENAR – Introduzca X…]*  
3. *[A RELLENAR – Haga clic en el botón X…]*  
4. *[A RELLENAR – paso adicional si es necesario]*  
5. *[A RELLENAR – paso adicional si es necesario]*  

> Incluir datos exactos de prueba (usuario, archivo, valores) para que el error sea reproducible al 100 %.

---

## Comportamiento actual

*[A RELLENAR]*  

> Describir de forma objetiva lo que ocurre: mensajes de error, pantallas incorrectas, valores calculados mal, excepciones, etc.

---

## Comportamiento esperado

*[A RELLENAR]*  

> Describir lo que debería ocurrir según el caso de prueba o la especificación (resultado esperado del CP correspondiente).

---

## Evidencia

| Tipo | Nombre de archivo / enlace |
|------|----------------------------|
| Captura de pantalla | *[A RELLENAR – ej. BUG-001-01.png]* |
| Video / grabación | *[A RELLENAR]* |
| Log / traza de consola | *[A RELLENAR]* |
| Archivo de entrada usado | *[A RELLENAR]* |

> Adjuntar siempre al menos una captura o video. Preferible incluir la consola del navegador o el log del servidor cuando sea un error técnico.

---

## Información adicional (opcional)

| Campo | Valor |
|-------|-------|
| **Frecuencia** | *[A RELLENAR – Siempre / Intermitente / Solo con datos específicos]* |
| **Workaround conocido** | *[A RELLENAR – o “Ninguno”]* |
| **Impacto en otros módulos** | *[A RELLENAR]* |
| **Notas del tester** | *[A RELLENAR]* |

---

## Historial de cambios del bug

| Fecha | Autor | Acción | Comentario |
|-------|-------|--------|------------|
| *[DD/MM/AAAA]* | *[Nombre]* | Creado | — |
| | | | |
| | | | |

---

*Plantilla convertida a Markdown a partir de “Plantilla Bugs report.docx”.*  
*Campos marcados con **[A RELLENAR]** están listos para ser completados por el agente de pruebas o el tester.*

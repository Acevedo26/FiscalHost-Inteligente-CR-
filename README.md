# FiscalHost Inteligente Costa Rica

## 📌 Descripción
FiscalHost Inteligente Costa Rica es un proyecto académico de ingeniería de software desarrollado en la Universidad Fidélitas.  
Su propósito es implementar un sistema inteligente de gestión fiscal adaptado al contexto costarricense, siguiendo buenas prácticas de desarrollo profesional.

---

## 🚀 Control de versiones

El equipo utiliza **Git** con una estrategia simplificada de **Git Flow**, adaptada al tamaño reducido del equipo (dos desarrolladores) y al calendario académico (febrero–diciembre 2026).

### Estructura de ramas

| Rama        | Propósito                                                                 | Protección                                                                 | Ciclo de vida |
|-------------|---------------------------------------------------------------------------|----------------------------------------------------------------------------|---------------|
| **main**    | Código estable desplegado en producción.                                  | Pull request con al menos una aprobación; checks de CI/CD; sin push directo | Permanente    |
| **develop** | Rama de integración para nuevas funcionalidades.                          | Pull request con al menos una aprobación; sin push directo                  | Permanente    |
| **feature/*** | Desarrollo de nuevas funcionalidades específicas. Ej: `feature/importacion-csv`. | Sin protección especial (trabajo local).                                    | Temporal      |
| **hotfix/***  | Corrección de errores críticos en producción. Ej: `hotfix/error-calculo-renta`. | Pull request hacia `main` y `develop`.                                     | Temporal      |

---

## 🔧 GitHub

- **Repositorio:** [FiscalHost-Inteligente-CR]
- **Visibilidad:** Privado  
- **Colaboradores:** José Andrés Acevedo, Hugo Alberto Villarreal  
- **Rama por defecto:** `main`  

### Configuración
- **Protecciones de ramas**
  - `main`: PR con aprobación, checks de CI/CD, sin push directo.
  - `develop`: PR con aprobación, sin push directo.
- **GitHub Projects:** Tablero Kanban con columnas *To Do*, *In Progress*, *Review*, *Done*.  
- **GitHub Issues:** Activado, con etiquetas por módulo (`M1`–`M5`) y tipo (`feature`, `bug`, `documentation`, `testing`).  
- **GitHub Actions:** Pipeline de CI/CD para compilación, pruebas y despliegue.

---

## 📂 Organización del trabajo

1. Cada requerimiento funcional se documenta como **Issue**.  
2. Se crea una rama `feature/*` asociada al Issue.  
3. Se realiza el desarrollo y se abre un **Pull Request** hacia `develop`.  
4. Una vez probado e integrado, se libera hacia `main` mediante PR aprobado.  
5. Los **hotfixes** se gestionan desde `main` y se sincronizan con `develop`.

---

## 👥 Equipo
- José Andrés Acevedo  
- Hugo Alberto Villarreal
- Enzo Josef Morales  

---

## 📅 Calendario
- Inicio: Febrero 2026  
- Finalización: Diciembre 2026  

# proyecto-atlas-api

## Contexto de trabajo

Si existe un workspace con AGENTS.md en los directorios padre, leerlo explícitamente antes de editar y seguir sus referencias. Una raíz Git anidada puede impedir la herencia automática. Fuera de ese entorno, este archivo y el README deben ser suficientes para trabajar.

## Código y arquitectura

- .NET/ASP.NET Core, EF Core, PostgreSQL y xUnit; solución ProyectoAtlas.slnx.
- Mantener Domain, Application, Infrastructure y Api con responsabilidades explícitas.
- Application usa slices con Command, Query, Handler, Response y Common.
- Api permanece fina; preferir Controllers/ plano salvo necesidad concreta.
- Tipos explícitos, convenciones modernas y GlobalUsings.cs para imports estables.
- Evitar namespace Shared si Common expresa la misma intención.
- Conservar unicidad de slugs, kinds inmutables, relaciones y contratos de documentación.
- Errores uniformes con HTTP status, code estable y message; fallas desconocidas sin detalles internos.

## Desarrollo y verificación

- Usar la versión del SDK fijada en global.json; preservar cambios locales de ese archivo.
- dotnet restore restaura; dotnet build compila; dotnet test ejecuta pruebas.
- Revisar README y configuración local para PostgreSQL y ejecución de la API.
- No aplicar migraciones ni modificar bases remotas durante comprobaciones documentales.
- No agregar abstracciones CRUD/base hasta tener usos reales que las justifiquen.

## Colaboración específica

- Modo mentor: explicar el siguiente paso pequeño y su motivo antes de introducir estructura.
- El usuario escribe el código salvo pedido explícito de implementación directa.
- Cerrar un contexto acotado antes de abrir varios agregados incompletos.
- Preferir intención/contrato, pruebas y una vertical slice completa.
- Decisiones estables de colaboración van aquí; coordinación de la sesión sigue el contexto disponible.

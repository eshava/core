# Eshava.Core — Repository Notes

Collection of methods, classes and helpers for recurring standard tasks. This is an umbrella
repository: eleven independently versioned NuGet packages live in one solution.

**Conventions:** documentation, code and commit messages are written in English. Line endings are
pinned through `.gitattributes` — anything that may run on Linux must be checked out with LF.

## Layout

| Project | Package | Content |
|---|---|---|
| `Eshava.Core` | `Eshava.Core` | Base helpers used by the other projects. |
| `Eshava.Core.Communication` | `Eshava.Core.Communication` | FTP (`System.Net.WebRequest`), SFTP (SSH.NET), HTTP, email (`SmtpClient`). |
| `Eshava.Core.Dynamic.Fields` | `Eshava.Core.Dynamic.Fields` | Fields defined at runtime instead of hard-coded in a C# class. |
| `Eshava.Core.Dynamic.Fields.Validation` | `…Dynamic.Fields.Validation` | Validation of those dynamic fields. |
| `Eshava.Core.IO` | `Eshava.Core.IO` | File system helpers. |
| `Eshava.Core.Linq` | `Eshava.Core.Linq` | Expression and query building. |
| `Eshava.Core.Logging` | `Eshava.Core.Logging` | Logging abstractions. |
| `Eshava.Core.Security` | `Eshava.Core.Security` | Security helpers. |
| `Eshava.Core.Storage` | `Eshava.Core.Storage` | Storage abstractions. |
| `Eshava.Core.Storage.Sql` | `Eshava.Core.Storage.Sql` | SQL implementation of those abstractions. |
| `Eshava.Core.Validation` | `Eshava.Core.Validation` | Object validation. |

Test projects are named `Eshava.Test.<Project>`. `Eshava.Test` holds shared test infrastructure.
Tests use **MSTest** with **FluentAssertions**.

`Eshava.Core.Dependencies.drawio` in the repository root documents how the projects relate.

## Rules

* **Each project is released on its own.** Do not assume that all eleven packages sit on the
  same version — consumers pin them individually.
* Adding a dependency from one project here to another tightens the release coupling. Weigh it
  before doing so.
* Every change to a project needs a change in its `Eshava.Test.<Project>` counterpart.

## Dependants

This repository is the root of the dependency graph. `Eshava.Storm`, `Eshava.Report.Pdf`,
`Eshava.DomainDrivenDesign` and `Eshava.DomainDrivenDesign.CodeAnalysis` all consume packages
from here. A breaking change reaches them only when the package version is raised there, so
they can lag arbitrarily far behind.

## Dependency Updates

Dependabot pull requests for third-party dependencies are active on this repository.

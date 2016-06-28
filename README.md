# uMigrate

![Build Status](https://ci.appveyor.com/api/projects/status/github/affinityid/umigrate?svg=true&branch=MASTER&passingText=MASTER%20BUILD%20OK&failingText=MASTER%20BUILD%20FAILED&pendingText=MASTER%20BUILD%20QUEUED) [![NuGet: uMigrate](https://img.shields.io/nuget/v/uMigrate.svg?style=flat&label=NuGet:%20uMigrate)](https://www.nuget.org/packages/uMigrate/) [![NuGet: uMigrate.Core](https://img.shields.io/nuget/v/uMigrate.Core.svg?style=flat&label=NuGet:%20uMigrate.Core)](https://www.nuget.org/packages/uMigrate.Core/)

A code-first migration framework for Umbraco (7.1.4+) started at [Affinity ID](http://www.affinityid.co.nz).
You can use it to add/change/remove Document Types, Data Types, etc.

## Quick start

1. Install uMigrate from [NuGet Gallery](http://nuget.org/packages/uMigrate).
2. Create your first migration by extending `UmbracoMigrationBase`:

    ```csharp
    public class Migration_201409261039_First : UmbracoMigrationBase {
        public override void Run() {
            var dataType = DataTypes.Add("New Data Type", Constants.PropertyEditors.Textbox, null);
            ContentTypes.Add("newDocumentType", "New Document Type")
                        .AddPropertyGroup("New Tab")
                        .AddProperty("newProperty", "NewProperty", dataType.Object);
        }
    }
    ```
3. Rebuild and open your Umbraco site.

4. Go to Settings > Document Types.
It should have New Document Type, with New Property of type New Data Type.

5. Go to Developer > Migrations.
It should list Migration_201409261039_First -- open it to see the migration log.

## Notes

### Migration process

* There is no proper rollback as it is almost impossible to run Umbraco changes in transaction.
Please backup your database before running migrations.

* There is no UI for migration-in-progress yet.

### API

* Even though uMigrate is code-first, it tries to support direct editing as well.
For example, if you do `ContentTypes.Add("ABC").AddPropertyGroup(…).AddProperty(…)` and there is already a content type named "ABC", uMigrate would add property to it instead of failing.

* uMigrate tries to provide a useful fluent interface -- however practically it is only provided where it as needed in Affinity ID projects.
Please feel free to provide pull requests with any additions/improvements.
You can use Umbraco interfaces directly through `IMigrationContext`.

* Fluent interface is designed to work with lists (jQuery-style).
So `ContentTypes.AddPropertyGroup("Test")` would add property group "Test" to all content types in the system.

### Logging

* uMigrate tries to produce a good log, but it is not able to detect changes made by `Change*` methods.
If you need a proper log for that, use `Logger` provided by `UmbracoMigrationBase`.

### Testing

* It is potentially possible to mock migration dependencies to unit-test migrations, but it is not a final design.
Currently the best choice is to do integration tests.

### Other

More detailed documentation might be available at the [Wiki](../../wiki).

## Contributing

Contributions are welcome, though for feature requests please create an issue first
to see if your changes would align with general design of uMigrate.

Each contribution should include integration tests. Unit tests are optional but welcome.
# skeleton-netcore-ef-code-first

- [description](#description)
- [prepare database context](#prepare-database-context)
- [managing migrations](#managing-migrations)
  - [first migrations](#first-migrations)
  - [update database](#update-database)
- [run the app](#run-the-app)
- [migration tool summary](#migration-tool-summary)
- [clean architecture](#clean-architecture)
- [how this project was built](#how-this-project-was-built)

:point_right: one-to-one, one-to-many, many-to-many code first relationship examples [here](https://github.com/devel0/skeleton-netcore-ef-code-first/tree/one-to-many#skeleton-netcore-ef-code-first)

## description

Shows how to create a code-first local db using sqlite but the same approach can applied using other backends.

## prepare database context

- [add nuget pkg][1] of EntityFrameworkCore.Design and for specific backend used
- [create table as code first][2]
- [create db context][3]
  - [override OnConfiguring][4] where setup specific backend used
  - [override OnModelCreating][5] where table fields can be configured ( primary keys, indexes, ... ) or seeding of initial data can be added
  - [create a dataset][6] foreach of the table wants to be materialized on the db

[1]: https://github.com/devel0/skeleton-netcore-ef-code-first/blob/ed27b430cdb5166ac7801ee3b5b493cca64e4bf3/skeleton-netcore-ef-code-first.csproj#L13
[2]: https://github.com/devel0/skeleton-netcore-ef-code-first/blob/ed27b430cdb5166ac7801ee3b5b493cca64e4bf3/Types/SampleData.cs#L3
[3]: https://github.com/devel0/skeleton-netcore-ef-code-first/blob/ed27b430cdb5166ac7801ee3b5b493cca64e4bf3/Data/DbContext.cs#L6
[4]: https://github.com/devel0/skeleton-netcore-ef-code-first/blob/ed27b430cdb5166ac7801ee3b5b493cca64e4bf3/Data/DbContext.cs#L27
[5]: https://github.com/devel0/skeleton-netcore-ef-code-first/blob/ed27b430cdb5166ac7801ee3b5b493cca64e4bf3/Data/DbContext.cs#L51
[6]: https://github.com/devel0/skeleton-netcore-ef-code-first/blob/ed27b430cdb5166ac7801ee3b5b493cca64e4bf3/Data/DbContext.cs#L69

## managing migrations

Migrations in code-first db allow team of developers to work on the same db project each with their on local db and versioning their changes through commits that includes `Migrations` folder.

The first-est migration is done by a developer and acts as an entry point for the work on that database so other developers can add their code-first changes after that commit by adding further migrations including code and `Migrations` folder changes.

Because each migration has a timestamp in the filename there aren't conflict on these while the `Migrations/LocalDbContextModelSnapshots.cs` can be subjected to normal git conflicts in some circumnstances that can be resolved in the usual way. To reduce conflict situation, regular pulls could help.

### first migrations

Install dotnet ef migrations tools:

```sh
dotnet tool install --global dotnet-ef
```

Create initial migration:

```sh
cd skeleton-netcore-ef-core-first
dotnet ef migrations add initial
```

Commit the initial migration ( I didn't added the initial migration for didactic purpose ).

### update database

Create a migration doesn't imply the code-first materialize on database, in order to do that, issue:

```sh
dotnet ef database update
```

This command can be used also to revert a migration applied by specifying a name of a migration already applied and all migrations after that one specified will be reverted returning to a state of "pending".

Migrations can be removed, when are in pending state, one by one from tail using `dotnet ef migrations remove`.

## run the app

```sh
dn run
Started with 0 records
add new one [newRecord1]
changes: SampleData {Id: -9223372036854774807} Added
```

## migration tool summary

| cmd                                       | description                                                |
| ----------------------------------------- | ---------------------------------------------------------- |
| `dotnet ef migrations list`               | list migrations showing which aren't yet applied (pending) |
| `dotnet ef database update`               | apply pending migrations                                   |
| `dotnet ef migrations add MIGRATION_NAME` | add migrations                                             |
| `dotnet ef migrations remove`             | remove latest not yet committed migration                  |
| `dotnet ef databse update MIGRATION_TO`   | revert applied migration                                   |

## clean architecture

To improve maintainability, modularity and separation of concerns in enterprise applications the [Clean architecture](https://yoan-thirion.gitbook.io/knowledge-base/software-craftsmanship/code-katas/clean-architecture) should evaluated; there are many boiler plate templates available:

```sh
dotnet new search clean
```

## how this project was built

```sh
dotnet new console -n skeleton-netcore-ef-code-first -f net7.0 --langVersion 11

cd skeleton-netcore-ef-code-first
dotnet add package Microsoft.EntityFrameworkCore.Design --version 7.0.5
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 7.0.5
```

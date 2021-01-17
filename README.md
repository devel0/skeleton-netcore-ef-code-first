# skeleton-netcore-ef-code-first

<!-- TOC -->
* [description](#description)
* [how this project was built](#how-this-project-was-built)
<!-- TOCEND -->

## description

example of netcore console app using efcore + psql with support for migrations backup.

## quickstart

- create a db on target

- create first migration

```sh
git clone https://github.com/devel0/skeleton-netcore-ef-code-first
cd skeleton-netcore-ef-code-first
dotnet run
```

- tune created `~/.config/skeleton-netcore-ef-core-first/config.json` file

- execute first migration ( this will create or update tables )

```sh
./add-migr.sh
```

- execute example

```sh
$ dotnet run -c Release
=== ENT NAME [skeleton_netcore_ef_code_first.MigrationsBackup]
=== ENT NAME [skeleton_netcore_ef_code_first.SampleTable]
warn: Microsoft.EntityFrameworkCore.Model.Validation[10400]
      Sensitive data logging is enabled. Log entries and exception messages may include sensitive application data; this mode should only be enabled during development.
info: Microsoft.EntityFrameworkCore.Infrastructure[10403]
      Entity Framework Core 5.0.2 initialized 'MyDbContext' using provider 'Npgsql.EntityFrameworkCore.PostgreSQL' with options: SensitiveDataLoggingEnabled 
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (3ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT s.id, s.create_timestamp, s.some_data, s.update_timestamp
      FROM sample_table AS s
      ORDER BY s.create_timestamp DESC
      LIMIT 1
last created record @[17/01/2021 20:15:04] some_data=1
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (8ms) [Parameters=[@p0='2021-01-17T19:19:56.3664328Z' (DbType = DateTime), @p1='2', @p2=NULL (DbType = DateTime)], CommandType='Text', CommandTimeout='30']
      INSERT INTO sample_table (create_timestamp, some_data, update_timestamp)
      VALUES (@p0, @p1, @p2)
      RETURNING id;
new record added : {
  "create_timestamp": "2021-01-17T19:19:56.3664328Z",
  "some_data": 2,
  "id": 10,
  "update_timestamp": null
}
```

notes:
- create timestamp recorded from code using [utc]() so that db doesn't need to store timezone

## manage migrations




## how this project was built

```sh
dotnet new console -n skeleton-netcore-ef-code-first
cd skeleton-netcore-ef-code-first
dotnet add package Microsoft.EntityFrameworkCore.Design --version 5.0.2
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 5.0.1
dotnet add package Microsoft.Extensions.Logging.Console --version 5.0.0
dotnet add package Microsoft.Extensions.Options --version 5.0.0
dotnet add package Newtonsoft.Json --version 12.0.3
dotnet add package netcore-util --version 1.9.1
dotnet add package Mono.Posix.NETStandard --version 5.20.1-preview
```
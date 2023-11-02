# skeleton-netcore-ef-code-first

- [description](#description)
- [quickstart](#quickstart)
- [expected result](#expected-result)
- [how this project was built](#how-this-project-was-built)

## description

Shows how to create a code-first local db using sqlite but the same approach can applied using other backends.

## quickstart

```sh
git clone https://github.com/devel0/skeleton-netcore-ef-code-first
cd skeleton-netcore-ef-code-first
git checkout one-to-many
dotnet tool restore
dotnet ef database update
dotnet run
```

## expected result

```sh
(local) DB> warn: 11/2/2023 22:39:19.751 CoreEventId.SensitiveDataLoggingEnabledWarning[10400] (Microsoft.EntityFrameworkCore.Infrastructure) 
      Sensitive data logging is enabled. Log entries and exception messages may include sensitive application data; this mode should only be enabled during development.
(local) DB> info: 11/2/2023 22:39:20.033 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command) 
      Executed DbCommand (5ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT COUNT(*)
      FROM "Posts" AS "p"
(local) DB> info: 11/2/2023 22:39:20.177 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command) 
      Executed DbCommand (2ms) [Parameters=[@p0='post1' (Nullable = false) (Size = 5)], CommandType='Text', CommandTimeout='30']
      INSERT INTO "Posts" ("Title")
      VALUES (@p0)
      RETURNING "Id";
(local) DB> info: 11/2/2023 22:39:20.183 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command) 
      Executed DbCommand (0ms) [Parameters=[@p0='post2' (Nullable = false) (Size = 5)], CommandType='Text', CommandTimeout='30']
      INSERT INTO "Posts" ("Title")
      VALUES (@p0)
      RETURNING "Id";
(local) DB> info: 11/2/2023 22:39:20.184 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command) 
      Executed DbCommand (0ms) [Parameters=[@p0='asp-net-core' (Nullable = false) (Size = 12)], CommandType='Text', CommandTimeout='30']
      INSERT INTO "Tags" ("Value")
      VALUES (@p0)
      RETURNING "Id";
(local) DB> info: 11/2/2023 22:39:20.184 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command) 
      Executed DbCommand (0ms) [Parameters=[@p0='ef-core' (Nullable = false) (Size = 7)], CommandType='Text', CommandTimeout='30']
      INSERT INTO "Tags" ("Value")
      VALUES (@p0)
      RETURNING "Id";
(local) DB> info: 11/2/2023 22:39:20.185 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command) 
      Executed DbCommand (0ms) [Parameters=[@p0='code-first' (Nullable = false) (Size = 10)], CommandType='Text', CommandTimeout='30']
      INSERT INTO "Tags" ("Value")
      VALUES (@p0)
      RETURNING "Id";
(local) DB> info: 11/2/2023 22:39:20.186 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command) 
      Executed DbCommand (0ms) [Parameters=[@p1='1', @p2='1'], CommandType='Text', CommandTimeout='30']
      INSERT INTO "PostTags" ("PostId", "TagId")
      VALUES (@p1, @p2)
      RETURNING "Id";
(local) DB> info: 11/2/2023 22:39:20.186 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command) 
      Executed DbCommand (0ms) [Parameters=[@p0='2', @p1='1'], CommandType='Text', CommandTimeout='30']
      INSERT INTO "PostTags" ("PostId", "TagId")
      VALUES (@p0, @p1)
      RETURNING "Id";
(local) DB> info: 11/2/2023 22:39:20.186 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command) 
      Executed DbCommand (0ms) [Parameters=[@p0='2', @p1='2'], CommandType='Text', CommandTimeout='30']
      INSERT INTO "PostTags" ("PostId", "TagId")
      VALUES (@p0, @p1)
      RETURNING "Id";
(local) DB> info: 11/2/2023 22:39:20.186 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command) 
      Executed DbCommand (0ms) [Parameters=[@p0='2', @p1='3'], CommandType='Text', CommandTimeout='30']
      INSERT INTO "PostTags" ("PostId", "TagId")
      VALUES (@p0, @p1)
      RETURNING "Id";
(local) DB> info: 11/2/2023 22:39:20.301 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command) 
      Executed DbCommand (0ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT "p"."Id", "p"."Title", "t0"."Id", "t0"."PostId", "t0"."TagId", "t0"."Id0", "t0"."Value"
      FROM "Posts" AS "p"
      LEFT JOIN (
          SELECT "p0"."Id", "p0"."PostId", "p0"."TagId", "t"."Id" AS "Id0", "t"."Value"
          FROM "PostTags" AS "p0"
          INNER JOIN "Tags" AS "t" ON "p0"."TagId" = "t"."Id"
      ) AS "t0" ON "p"."Id" = "t0"."PostId"
      ORDER BY "p"."Id", "t0"."Id"
{
  "postTitle": "post1",
  "tags": [
    "asp-net-core"
  ]
}
{
  "postTitle": "post2",
  "tags": [
    "asp-net-core",
    "ef-core",
    "code-first"
  ]
}
```

## how this project was built

```sh
dotnet new console -n skeleton-netcore-ef-code-first -f net7.0 --langVersion 11

cd skeleton-netcore-ef-code-first
dotnet new tool-manifest
dotnet tool install dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design --version 7.0.5
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 7.0.5
dotnet ef migrations add init
```

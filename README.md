# skeleton-netcore-ef-code-first

- [description](#description)
- [quickstart](#quickstart)
- [Generated db structure](#generated-db-structure)
- [One (A) to One (B)](#one-a-to-one-b)
- [One (C) to Many (D)](#one-c-to-many-d)
- [Many (E) to Many (F)](#many-e-to-many-f)
- [Test futher configs](#test-futher-configs)
- [how this project was built](#how-this-project-was-built)

## description

Shows how to create a code-first local db using sqlite but the same approach can applied using other backends.

## quickstart

```sh
git clone https://github.com/devel0/skeleton-netcore-ef-code-first
cd skeleton-netcore-ef-code-first
git checkout one-to-many
dotnet tool restore
# dotnet ef database update # migrations auto-applied by code
dotnet run
```

## Generated db structure

![](doc/generated-db-structure.png)

## One (A) to One (B)

```csharp
public class TableA_One
{
    [Key]
    public int Id { get; set; }
    public int BObjectId { get; set; } // required foreign key
    public TableB_One BObject { get; set; } = null!; // A has one B ( required )
    public string? Data { get; set; }
}

public class TableB_One
{
    [Key]
    public int Id { get; set; }
    public TableA_One? AObject { get; set; } // B has one A ( optional )
    public string? Data { get; set; }
}
```

![](doc/one-to-one.svg)

note:

- `ObjectId` specified otherwise follow compile error generates:

_The dependent side could not be determined for the one-to-one relationship between 'TableA_One.BObject' and 'TableB_One.AObject'. To identify the dependent side of the relationship, configure the foreign key property. If these navigations should not be part of the same relationship, configure them independently via separate method chains in 'OnModelCreating'. See http://go.microsoft.com/fwlink/?LinkId=724062 for more details._

```csharp
var a1 = new TableA_One { Data = "a1" };
var a2 = new TableA_One { Data = "a2" };

var b1 = new TableB_One { Data = "b1", AObject = a1 }; // NOTE: AObject HERE WILL NULLIFIED
var b2 = new TableB_One { Data = "b2", AObject = a1 }; // CAUSE THIS FURTHER ASSIGNMENT ( relation is one-to-one )
var b3 = new TableB_One { Data = "b3", AObject = a2 };

dbContext.BRecords.AddRange(new[] { b1, b2, b3 });
```

**from A to B**

```csharp
var q = dbContext.ARecords
  .Include(x => x.BObject)
  .Select(x => new { a = new { x.Id, x.Data }, b = new { x.BObject.Id, x.BObject.Data } })
  .ToList();
```

```log
FROM A TO B
===========
(local) DB> info: 11/18/2023 11:25:41.570 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (0ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT "a"."Id", "a"."Data", "b"."Id", "b"."Data"
      FROM "ARecords" AS "a"
      INNER JOIN "BRecords" AS "b" ON "a"."BObjectId" = "b"."Id"
{
  "a": {
    "Id": 1,
    "Data": "a1"
  },
  "b": {
    "Id": 2,
    "Data": "b2"
  }
}
{
  "a": {
    "Id": 2,
    "Data": "a2"
  },
  "b": {
    "Id": 3,
    "Data": "b3"
  }
}
```

**from B to A**

```csharp
var q = dbContext.BRecords
  .Include(x => x.AObject)
  .Select(x => new
  {
      b = new { x.Id, x.Data },
      a = new
      {
          Id = x.AObject != null ? x.AObject.Id : (int?)null,
          Data = x.AObject != null ? x.AObject.Data : null
      }
  })
  .ToList();
```

```log
FROM B TO A
===========
(local) DB> info: 11/18/2023 11:47:05.376 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command) 
      Executed DbCommand (0ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT "b"."Id", "b"."Data", "a"."Id", "a"."Data"
      FROM "BRecords" AS "b"
      LEFT JOIN "ARecords" AS "a" ON "b"."Id" = "a"."BObjectId"
{
  "b": {
    "Id": 1,
    "Data": "b1"
  },
  "a": {
    "Id": null,
    "Data": null
  }
}
{
  "b": {
    "Id": 2,
    "Data": "b2"
  },
  "a": {
    "Id": 1,
    "Data": "a1"
  }
}
{
  "b": {
    "Id": 3,
    "Data": "b3"
  },
  "a": {
    "Id": 2,
    "Data": "a2"
  }
}
```

## One (C) to Many (D)

```csharp
public class TableC_One
{
    [Key]
    public int Id { get; set; }
    public List<TableD_Many> DObjects { get; set; } = new(); // C has many D
    public string? Data { get; set; }
}

public class TableD_Many
{
    [Key]
    public int Id { get; set; }    
    public TableC_One? CObject { get; set; } // D has one C
    public string? Data { get; set; }
}
```

![](doc/one-to-many.svg)

```csharp
var c1 = new TableC_One { Data = "c1" };
var c2 = new TableC_One { Data = "c2" };

var d1 = new TableD_Many { Data = "d1", CObject = c1 };
var d2 = new TableD_Many { Data = "d2", CObject = c1 };
var d3 = new TableD_Many { Data = "d3", CObject = c2 };
```

**from C to D**

```csharp
var q = dbContext.CRecords
  .Include(x => x.DObjects)
  .Select(x => new { C = new { x.Id, x.Data }, D = x.DObjects.Select(y => new { y.Id, y.Data }) })
  .ToList();
```

```log
FROM C TO D
===========
(local) DB> info: 11/18/2023 11:35:16.563 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (0ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT "c"."Id", "c"."Data", "d"."Id", "d"."Data"
      FROM "CRecords" AS "c"
      LEFT JOIN "DRecords" AS "d" ON "c"."Id" = "d"."CObjectId"
      ORDER BY "c"."Id"
{
  "C": {
    "Id": 1,
    "Data": "c1"
  },
  "D": [
    {
      "Id": 1,
      "Data": "d1"
    },
    {
      "Id": 2,
      "Data": "d2"
    }
  ]
}
{
  "C": {
    "Id": 2,
    "Data": "c2"
  },
  "D": [
    {
      "Id": 3,
      "Data": "d3"
    }
  ]
}
```

**from D to C**

```csharp
var q = dbContext.DRecords
  .Include(x => x.CObject)
  .Select(x => new
  {
      D = new { x.Id, x.Data },
      C = new
      {
          Id = x.CObject != null ? x.CObject.Id : (int?)null,
          Data = x.CObject != null ? x.CObject.Data : null
      }
  })
  .ToList();
```

```log
FROM D TO C
===========
(local) DB> info: 11/18/2023 11:52:58.785 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command) 
      Executed DbCommand (0ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT "d"."Id", "d"."Data", "c"."Id", "c"."Data"
      FROM "DRecords" AS "d"
      LEFT JOIN "CRecords" AS "c" ON "d"."CObjectId" = "c"."Id"
{
  "D": {
    "Id": 1,
    "Data": "d1"
  },
  "C": {
    "Id": 1,
    "Data": "c1"
  }
}
{
  "D": {
    "Id": 2,
    "Data": "d2"
  },
  "C": {
    "Id": 1,
    "Data": "c1"
  }
}
{
  "D": {
    "Id": 3,
    "Data": "d3"
  },
  "C": {
    "Id": 2,
    "Data": "c2"
  }
}
```

## Many (E) to Many (F)

```csharp
public class TableE_Many
{
    [Key]
    public int Id { get; set; }
    public List<TableF_Many>? FObjects { get; set; } = new(); // E has many
    public string? Data { get; set; }
}

public class TableF_Many
{
    [Key]
    public int Id { get; set; }
    public List<TableE_Many> EObjects { get; set; } = new(); // F has many
    public string? Data { get; set; }
}
```

![](doc/many-to-many.svg)

```csharp
var e1 = new TableE_Many { Data = "e1" };
var e2 = new TableE_Many { Data = "e2" };
var e3 = new TableE_Many { Data = "e3" };

var f1 = new TableF_Many { Data = "f1" };
var f2 = new TableF_Many { Data = "f2" };
var f3 = new TableF_Many { Data = "f3" };

e1.FObjects.Add(f1);
e1.FObjects.Add(f2);

e2.FObjects.Add(f3);

f3.EObjects.Add(e3);
f3.EObjects.Add(e1);

dbContext.ERecords.AddRange(new[] { e1, e2, e3 });
```

**from E to F**

```csharp
var q = dbContext.ERecords
  .Include(x => x.FObjects)
  .Select(x => new
  {
      E = new { x.Id, x.Data },
      F = x.FObjects.Select(y => new { y.Id, y.Data }),
  })
  .ToList();
```

```log
FROM E TO F
===========
(local) DB> info: 11/18/2023 11:25:41.757 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (0ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT "e"."Id", "e"."Data", "t0"."Id", "t0"."Data", "t0"."EObjectsId", "t0"."FObjectsId"
      FROM "ERecords" AS "e"
      LEFT JOIN (
          SELECT "f"."Id", "f"."Data", "t"."EObjectsId", "t"."FObjectsId"
          FROM "TableE_ManyTableF_Many" AS "t"
          INNER JOIN "FRecords" AS "f" ON "t"."FObjectsId" = "f"."Id"
      ) AS "t0" ON "e"."Id" = "t0"."EObjectsId"
      ORDER BY "e"."Id", "t0"."EObjectsId", "t0"."FObjectsId"
{
  "E": {
    "Id": 1,
    "Data": "e1"
  },
  "F": [
    {
      "Id": 1,
      "Data": "f1"
    },
    {
      "Id": 2,
      "Data": "f2"
    },
    {
      "Id": 3,
      "Data": "f3"
    }
  ]
}
{
  "E": {
    "Id": 2,
    "Data": "e2"
  },
  "F": [
    {
      "Id": 3,
      "Data": "f3"
    }
  ]
}
{
  "E": {
    "Id": 3,
    "Data": "e3"
  },
  "F": [
    {
      "Id": 3,
      "Data": "f3"
    }
  ]
}
```

**from F to E**

```csharp
var q = dbContext.FRecords
  .Include(x => x.EObjects)
  .Select(x => new
  {
      F = new { x.Id, x.Data },
      E = x.EObjects.Select(y => new { y.Id, y.Data }),
  })
  .ToList();
```

```log
FROM F TO E
===========
(local) DB> info: 11/18/2023 11:25:41.765 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (0ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT "f"."Id", "f"."Data", "t0"."Id", "t0"."Data", "t0"."EObjectsId", "t0"."FObjectsId"
      FROM "FRecords" AS "f"
      LEFT JOIN (
          SELECT "e"."Id", "e"."Data", "t"."EObjectsId", "t"."FObjectsId"
          FROM "TableE_ManyTableF_Many" AS "t"
          INNER JOIN "ERecords" AS "e" ON "t"."EObjectsId" = "e"."Id"
      ) AS "t0" ON "f"."Id" = "t0"."FObjectsId"
      ORDER BY "f"."Id", "t0"."EObjectsId", "t0"."FObjectsId"
{
  "F": {
    "Id": 1,
    "Data": "f1"
  },
  "E": [
    {
      "Id": 1,
      "Data": "e1"
    }
  ]
}
{
  "F": {
    "Id": 2,
    "Data": "f2"
  },
  "E": [
    {
      "Id": 1,
      "Data": "e1"
    }
  ]
}
{
  "F": {
    "Id": 3,
    "Data": "f3"
  },
  "E": [
    {
      "Id": 1,
      "Data": "e1"
    },
    {
      "Id": 2,
      "Data": "e2"
    },
    {
      "Id": 3,
      "Data": "e3"
    }
  ]
}
```

## Test futher configs

to redo other tests after mapping changes:

```sh
rm -fr bin obj Migrations
dotnet ef migrations add init
dotnet run
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

var dbContext = new LocalDbContext(readonlyMode: false);

//
// apply pending migrations
//
{
    var pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();
    if (pendingMigrations.Count > 0)
    {
        Console.WriteLine($"apply {pendingMigrations.Count} migrations");

        dbContext.Database.Migrate();
    }
}

//
// TableA (1)---(1) TableB
//
if (dbContext.DRecords.Count() == 0)
{
    var a1 = new TableA_One { Data = "a1" };
    var a2 = new TableA_One { Data = "a2" };

    var b1 = new TableB_One { Data = "b1", AObject = a1 }; // NOTE: THIS WILL GET SKIPPED
    var b2 = new TableB_One { Data = "b2", AObject = a1 }; // CAUSE THIS FURTHER ASSIGNMENT ( relation is one-to-one )
    var b3 = new TableB_One { Data = "b3", AObject = a2 };

    dbContext.BRecords.AddRange(new[] { b1, b2, b3 }); // NOTE: b1 "OVERWRITTEN" BY b2
    dbContext.SaveChanges();

    {
        Console.WriteLine();
        Console.WriteLine("FROM A TO B");
        Console.WriteLine("===========");

        var q = dbContext.ARecords
            .Include(x => x.BObject)
            .Select(x => new { a = new { x.Id, x.Data }, b = new { x.BObject.Id, x.BObject.Data } })
            .ToList();

        foreach (var x in q)
            Console.WriteLine(JsonSerializer.Serialize(x, new JsonSerializerOptions { WriteIndented = true }));
    }

    {
        Console.WriteLine();
        Console.WriteLine("FROM B TO A");
        Console.WriteLine("===========");

        var q = dbContext.BRecords
            .Where(x => x.AObject != null)
            .Include(x => x.AObject)
            .Select(x => new { b = new { x.Id, x.Data }, a = new { x.AObject!.Id, x.AObject.Data } })
            .ToList();

        foreach (var x in q)
            Console.WriteLine(JsonSerializer.Serialize(x, new JsonSerializerOptions { WriteIndented = true }));
    }
}

//
// TableC (1)---(*) TableD
//
if (dbContext.DRecords.Count() == 0)
{
    var c1 = new TableC_One { Data = "c1" };
    var c2 = new TableC_One { Data = "c2" };

    var d1 = new TableD_Many { Data = "d1", CObject = c1 };
    var d2 = new TableD_Many { Data = "d2", CObject = c1 };
    var d3 = new TableD_Many { Data = "d3", CObject = c2 };

    dbContext.DRecords.AddRange(new[] { d1, d2, d3 });
    dbContext.SaveChanges();

    {
        Console.WriteLine();
        Console.WriteLine("FROM C TO D");
        Console.WriteLine("===========");

        var q = dbContext.CRecords
            .Include(x => x.DObjects)
            .Select(x => new { C = new { x.Id, x.Data }, D = x.DObjects.Select(y => new { y.Id, y.Data }) })
            .ToList();

        foreach (var x in q)
            Console.WriteLine(JsonSerializer.Serialize(x, new JsonSerializerOptions { WriteIndented = true }));
    }

    {
        Console.WriteLine();
        Console.WriteLine("FROM D TO C");
        Console.WriteLine("===========");

        var q = dbContext.DRecords
            .Where(x => x.CObject != null)
            .Include(x => x.CObject)
            .Select(x => new { D = new { x.Id, x.Data }, C = new { x.CObject!.Id, x.CObject.Data } })
            .ToList();

        foreach (var x in q)
            Console.WriteLine(JsonSerializer.Serialize(x, new JsonSerializerOptions { WriteIndented = true }));
    }
}

//
// TableE (*)---(*) TableF
//
if (dbContext.ERecords.Count() == 0)
{
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
    dbContext.SaveChanges();

    {
        Console.WriteLine();
        Console.WriteLine("FROM E TO F");
        Console.WriteLine("===========");

        var q = dbContext.ERecords
            .Include(x => x.FObjects)
            .Select(x => new
            {
                E = new { x.Id, x.Data },
                F = x.FObjects.Select(y => new { y.Id, y.Data }),
            })
            .ToList();

        foreach (var x in q)
            Console.WriteLine(JsonSerializer.Serialize(x, new JsonSerializerOptions { WriteIndented = true }));
    }

    {
        Console.WriteLine();
        Console.WriteLine("FROM F TO E");
        Console.WriteLine("===========");

        var q = dbContext.FRecords
            .Include(x => x.EObjects)
            .Select(x => new
            {
                F = new { x.Id, x.Data },
                E = x.EObjects.Select(y => new { y.Id, y.Data }),
            })
            .ToList();

        foreach (var x in q)
            Console.WriteLine(JsonSerializer.Serialize(x, new JsonSerializerOptions { WriteIndented = true }));
    }
}
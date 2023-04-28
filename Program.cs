using skeleton_netcore_ef_code_first;

// create db context
var dbContext = new LocalDbContext(readonlyMode: false);

// query
var cnt = dbContext.Datas.Count();
System.Console.WriteLine($"Started with {cnt} records");

var newRecordName = $"newRecord{++cnt}";
System.Console.WriteLine($"add new one [{newRecordName}]");

// add record to the model
dbContext.Datas.Add(new SampleData { Name = newRecordName, Value = cnt });

System.Console.WriteLine($"changes: {dbContext.ChangeTracker.DebugView.ShortView}");

// save changes to db
dbContext.SaveChanges();
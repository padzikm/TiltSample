// See https://aka.ms/new-console-template for more information

using ConsoleApp1;
using MongoDB.Driver;
using MongoDB.Entities;

await DB.InitAsync("MigrateTest", "localhost", 27017);

await DB.MigrateAsync();

// var mongo = new MongoClient("mongodb://localhost:27017");
//
// var db = mongo.GetDatabase("sharded2");
// var coll = db.GetCollection<Perf>("dist3");
//
// for (var i = 0; i < 100; ++i)
// {
//     var l = new List<Perf>(10000);
//     for (var j = 0; j < 10000; ++j)
//     {
//         var p = new Perf()
//         {
//             Id = Guid.NewGuid().ToString(),
//             InstanceId = Guid.NewGuid().ToString()
//         };
//         l.Add(p);
//     }
//     await coll.InsertManyAsync(l);
// }

// Console.WriteLine((Guid.NewGuid().ToString()));
Console.WriteLine("Hello, World!");
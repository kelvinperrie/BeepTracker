// See https://aka.ms/new-console-template for more information
using BeepTracker.ApiClient.Models;
using BeepTracker.ApiClient;

Console.WriteLine("Hello, World!");

var options = new ClientOptions
{
    BaseAddress = "https://localhost:7056"
};

var service = new ClientService(options);

var birds = await service.GetBirds();



var records = await service.GetBeepRecords();

Console.WriteLine("test");

var newBeeps = new BeepRecord
{
    BirdName = "test",
    BeatsPerMinute = 10,
    Status = 1,
    RecordedDateTime = DateTime.Now,
};
await service.SaveBeepRecord(newBeeps);


newBeeps = new BeepRecord
{
    BirdName = "test2",
    BeatsPerMinute = 10,
    Status = 1,
    RecordedDateTime = DateTime.Now,
    BeepEntries = new List<BeepEntry>
    {
        new BeepEntry { Value = 2, Index = 1 },
        new BeepEntry { Value = 2, Index = 2 },
        new BeepEntry { Value = 2, Index = 3 }
    }
};
await service.SaveBeepRecord(newBeeps);
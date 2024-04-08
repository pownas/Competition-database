var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.CompetitionDB>("competitiondb");

builder.Build().Run();

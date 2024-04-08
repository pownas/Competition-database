var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.CompetitionDB>("competitionDB");

builder.Build().Run();

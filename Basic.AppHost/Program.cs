var builder = DistributedApplication.CreateBuilder(args);

var messaging = builder.AddRabbitMQContainer("messaging");

builder.AddProject<Projects.Consumer>("consumer").WithReference(messaging);

builder.AddProject<Projects.Producer>("producer").WithReference(messaging); 

builder.Build().Run();

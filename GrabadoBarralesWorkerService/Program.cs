using GrabadoBarralesWorkerService;
using GrabadoBarralesWorkerService.Models.MySql;
using GrabadoBarralesWorkerService.Models.PgSql;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();

builder.Services.AddDbContextFactory<MySqlContext>();
builder.Services.AddDbContextFactory<ESCORIALContext>();

builder.Services.AddSingleton<Tcp>();

var host = builder.Build();
await host.RunAsync();

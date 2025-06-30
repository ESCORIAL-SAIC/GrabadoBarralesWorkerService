using GrabadoBarralesWorkerService;
using GrabadoBarralesWorkerService.Models.MySql;
using GrabadoBarralesWorkerService.Models.PgSql;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();

builder.Services.AddDbContextFactory<MySqlContext>(options =>
{
    var connStr = builder.Configuration.GetConnectionString("MySql");
    options.UseMySql(connStr, ServerVersion.AutoDetect(connStr));
});

builder.Services.AddDbContextFactory<ESCORIALContext>(options =>
{
    var connStr = builder.Configuration.GetConnectionString("ESCORIAL");
    options.UseNpgsql(connStr);
});

builder.Services.AddSingleton<Tcp>();

var host = builder.Build();

await host.RunAsync();

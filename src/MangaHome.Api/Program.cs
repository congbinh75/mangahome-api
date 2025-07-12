using MangaHome.Api.Common;
using MangaHome.Api.Services;
using MangaHome.Api.Services.Implementations;
using MangaHome.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddSingleton<IHttpRequestInfoService, HttpRequestInfoService>();

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddDbContextPool<MangaHomeDbContext>(options => options
    .UseNpgsql(builder.Configuration.GetConnectionString("Database"))
    .UseSnakeCaseNamingConvention()
    .LogTo(Log.Logger.Information, LogLevel.Information));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Caching");
    options.InstanceName = "MangaHome";
});

builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .MinimumLevel.Override("System", LogEventLevel.Warning)
        .WriteTo.Console()
        .WriteTo.File(
            formatter: new CompactJsonFormatter(),
            path: "/logs/.log",
            rollingInterval: RollingInterval.Day,
            rollOnFileSizeLimit: true
        );
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.MapControllers();

app.UseExceptionHandler();

await app.RunAsync();

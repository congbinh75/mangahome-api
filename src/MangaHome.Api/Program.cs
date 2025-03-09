using MangaHome.Api.Common;
using MangaHome.Api.Models.Responses;
using MangaHome.Api.Services;
using MangaHome.Api.Services.Implementations;
using MangaHome.Core.Abstractions;
using MangaHome.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Net;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddSingleton<IPasswordHashingService, PasswordHashingService>();
builder.Services.AddSingleton<IDateTimeService, DateTimeService>();
builder.Services.AddSingleton<IRequestInfoService, RequestInfoService>();

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
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
    configuration.ReadFrom.Configuration(context.Configuration));

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

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var responseObject = new Response<object> 
        {
            Success = false,
            Errors = [
                new Error 
                {
                    Type = ErrorType.Error.ToString(),
                    Message = Messages.ERR_UNEXPECTED_ERROR,
                }
            ]
        };
        var jsonResponse = JsonSerializer.Serialize(responseObject);

        await context.Response.WriteAsync(jsonResponse);
    });
});

await app.RunAsync();

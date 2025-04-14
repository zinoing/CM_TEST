using ColorMemory.Data;
using ColorMemory.Repository.Implementations;
using ColorMemory.Services;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureHostOptions(options =>
{
    options.ShutdownTimeout = TimeSpan.FromSeconds(60);
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<WeeklyRankingDb>();
builder.Services.AddScoped<PlayerDb>();

builder.Services.AddScoped<PlayerService>();
builder.Services.AddScoped<ScoreService>();
builder.Services.AddScoped<ArtworkService>();
builder.Services.AddScoped<MoneyService>();
builder.Services.AddScoped<HintService>();
builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection")),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null)
    ));

builder.Services.AddHostedService<WeeklyResetService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    // app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

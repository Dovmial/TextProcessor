
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using TextProcessor.Services;
using TextProcessor.Data;
using Microsoft.Extensions.Configuration;
using TextProcessor.Helpers;

var builder = Host.CreateApplicationBuilder();

var configuration = builder.Configuration;
configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json");

builder.Services.AddDbService(configuration);

builder.Services.AddHostedService<TextProcessService>();

builder.Services.AddSingleton<FileReaderByLine>();
builder.Services.AddScoped<DbGateway>();

var app = builder.Build();

app.Run();
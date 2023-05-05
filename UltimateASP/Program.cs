using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

LogConfiguration.Configure();

builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

app.UseAllMiddlewares();

app.Run();
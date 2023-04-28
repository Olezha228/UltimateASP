var builder = WebApplication.CreateBuilder(args);

LogConfiguration.Configure();

builder.Services.ConfigureDI();
builder.Services.ConfigureServices(builder.Configuration);


var app = builder.Build();

app.UseAllMiddlewares();

app.Run();
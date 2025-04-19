using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfiguration) =>
    loggerConfiguration.ReadFrom.Configuration(context.Configuration)
);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Zen Arch API"));
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.UseSerilogRequestLogging(options =>
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} respondeu {StatusCode} em {Elapsed:0.0000} ms");

await app.RunAsync();

public partial class Program;

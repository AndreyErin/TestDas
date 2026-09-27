using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TestDas.Middlewares;
using TestDas.Models;
using TestDas.Services;
using TestDas.Services.DAL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.WriteIndented = true;
});
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.WriteIndented = true;
});
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddOpenApi();
builder.Services.AddScoped<IValidator<HtmlExtractionRequest>, HtmlExtractionRequestValidator>();
builder.Services.AddScoped<IParseService, ParseService>();
builder.Services.AddScoped<IElementsRepository, ElementRepository>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.MapOpenApi();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "v1");
    options.RoutePrefix = "api/swagger";
});
app.MapControllers();
app.Run();
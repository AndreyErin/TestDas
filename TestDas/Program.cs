using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TestDas.Models;
using TestDas.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddOpenApi();
builder.Services.AddScoped<IValidator<AnalysisRequest>, AnalysisRequestValidator>();


var app = builder.Build();

app.MapOpenApi();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "v1");
    options.RoutePrefix = "api/swagger";
});
app.MapControllers();
app.Run();
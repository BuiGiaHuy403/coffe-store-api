using Asp.Versioning.ApiExplorer;
using PRN232.Lab2.CoffeeStore.API;
using PRN232.Lab2.CoffeeStore.API.Utils;
using PRN232.Lab2.CoffeeStore.Repositories;
using PRN232.Lab2.CoffeeStore.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDataServices(builder.Configuration);
builder.Services.AddBusinessServices(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var desc in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{desc.GroupName}/swagger.json",
                $"CoffeeStore API {desc.GroupName.ToUpperInvariant()}" + (desc.IsDeprecated ? " (deprecated)" : "")
            );
        }
    });
    await app.InitialiseDatabaseAsync();
}

app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger/index.html", permanent: false);
    return Task.CompletedTask;
});
app.UseResponseCompression();

app.MapControllers();

app.Run();

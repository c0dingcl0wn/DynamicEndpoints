using DynamicEndpoints.Providers;
using Microsoft.AspNetCore.Mvc.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddSingleton<IActionDescriptorChangeProvider>(ActionDescriptorChangeProvider.Instance);

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();
app.UseRouting();
app.UseEndpoints(endpoint => endpoint.MapControllerRoute("default", "{controller}"));
app.Run();
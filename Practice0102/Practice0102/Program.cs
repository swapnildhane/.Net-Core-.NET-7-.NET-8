using Microsoft.AspNetCore.Builder;
using Practice0102.Interfaces;
using Practice0102.Services;

var builder = WebApplication.CreateBuilder(args);

// MVC + API
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();
builder.Services.AddHttpClient();

builder.Services.AddScoped<IDbService, DbServices>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger only in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

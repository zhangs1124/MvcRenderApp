var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// 雲端與容器健康檢查探針端點 (Render / Docker)
app.MapGet("/healthz", () => Results.Ok(new 
{ 
    status = "Healthy", 
    timestamp = DateTime.UtcNow, 
    environment = app.Environment.EnvironmentName,
    framework = ".NET 8.0",
    service = "MvcRenderApp-Inventory"
}));

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

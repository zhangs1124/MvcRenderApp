using Microsoft.EntityFrameworkCore;
using MvcRenderApp.Data;

var builder = WebApplication.CreateBuilder(args);

// 加入 SQLite 資料庫服務
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=inventory.db";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

// 加入 MVC 服務
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 自動確保 SQLite 資料庫建立並填入種子資料
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// HTTP 請求管線配置
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

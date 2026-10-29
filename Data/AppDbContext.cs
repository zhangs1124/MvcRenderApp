using Microsoft.EntityFrameworkCore;
using MvcRenderApp.Models;

namespace MvcRenderApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 初始種子資料 (Seed Data)
        modelBuilder.Entity<InventoryItem>().HasData(
            new InventoryItem { Id = 1, ItemCode = "MED-001", ItemName = "醫療無菌手套 (M)", Category = "醫療耗材", StockQuantity = 240, SafetyStock = 50, Unit = "盒", LastUpdated = DateTime.UtcNow },
            new InventoryItem { Id = 2, ItemCode = "MED-002", ItemName = "N95 防護口罩", Category = "防護裝備", StockQuantity = 18, SafetyStock = 30, Unit = "包", LastUpdated = DateTime.UtcNow },
            new InventoryItem { Id = 3, ItemCode = "MED-003", ItemName = "75% 酒精消毒液 500ml", Category = "消毒清潔", StockQuantity = 65, SafetyStock = 20, Unit = "瓶", LastUpdated = DateTime.UtcNow },
            new InventoryItem { Id = 4, ItemCode = "MED-004", ItemName = "生理食鹽水 500ml", Category = "醫藥製劑", StockQuantity = 8, SafetyStock = 25, Unit = "瓶", LastUpdated = DateTime.UtcNow },
            new InventoryItem { Id = 5, ItemCode = "OFF-001", ItemName = "A4 雙面影印紙 70g", Category = "行政耗材", StockQuantity = 50, SafetyStock = 10, Unit = "箱", LastUpdated = DateTime.UtcNow },
            new InventoryItem { Id = 6, ItemCode = "MED-005", ItemName = "一次性針筒 5ml", Category = "醫療耗材", StockQuantity = 0, SafetyStock = 15, Unit = "盒", LastUpdated = DateTime.UtcNow }
        );
    }
}

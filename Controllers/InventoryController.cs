using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcRenderApp.Data;
using MvcRenderApp.Models;

namespace MvcRenderApp.Controllers;

/// <summary>
/// 物料庫存管理控制器 (遵循 SPA-like Modal + AJAX 規範)
/// </summary>
public class InventoryController : Controller
{
    private readonly AppDbContext _context;

    public InventoryController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /Inventory
    public async Task<IActionResult> Index(string? search)
    {
        var query = _context.InventoryItems.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            string keyword = search.Trim().ToLower();
            query = query.Where(x => x.ItemName.ToLower().Contains(keyword) || 
                                     x.ItemCode.ToLower().Contains(keyword) ||
                                     x.Category.ToLower().Contains(keyword));
        }

        var items = await query.OrderBy(x => x.Id).ToListAsync();
        ViewBag.SearchKeyword = search ?? "";
        ViewBag.TotalCount = items.Count;
        ViewBag.WarningCount = items.Count(x => x.StockQuantity <= x.SafetyStock);
        return View(items);
    }

    // GET: /Inventory/AdjustStockModal?id=1&actionType=in
    // 回傳 Partial View 供全域 Modal 動態載入
    [HttpGet]
    public async Task<IActionResult> AdjustStockModal(int id, string actionType)
    {
        var item = await _context.InventoryItems.FindAsync(id);
        if (item == null)
        {
            return NotFound("找不到該物料品項");
        }

        ViewBag.ActionType = actionType.ToLower() == "out" ? "out" : "in";
        return PartialView("_AdjustStockModal", item);
    }

    // POST: /Inventory/AdjustStock
    // AJAX 提交快速出入庫更新
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdjustStock(int id, string actionType, int quantity, string? notes)
    {
        if (quantity <= 0)
        {
            return Json(new { success = false, message = "異動數量必須大於 0！" });
        }

        var item = await _context.InventoryItems.FindAsync(id);
        if (item == null)
        {
            return Json(new { success = false, message = "找不到該物料！" });
        }

        bool isOut = actionType.ToLower() == "out";
        if (isOut && item.StockQuantity < quantity)
        {
            return Json(new { success = false, message = $"出庫數量 ({quantity} {item.Unit}) 超出現有存量 ({item.StockQuantity} {item.Unit})！" });
        }

        // 調整存量
        if (isOut)
        {
            item.StockQuantity -= quantity;
        }
        else
        {
            item.StockQuantity += quantity;
        }

        item.LastUpdated = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        string actionText = isOut ? "出庫" : "入庫";
        return Json(new 
        { 
            success = true, 
            message = $"品項【{item.ItemName}】已成功{actionText} {quantity} {item.Unit}！",
            itemId = item.Id,
            newQuantity = item.StockQuantity,
            unit = item.Unit,
            statusBadge = item.StatusBadgeClass,
            statusText = item.StatusText,
            lastUpdated = item.LastUpdated.ToString("yyyy-MM-dd HH:mm")
        });
    }
}

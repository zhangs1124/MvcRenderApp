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

        var items = await query.OrderByDescending(x => x.Id).ToListAsync();
        ViewBag.SearchKeyword = search ?? "";
        ViewBag.TotalCount = items.Count;
        ViewBag.WarningCount = items.Count(x => x.StockQuantity <= x.SafetyStock);
        return View(items);
    }

    // GET: /Inventory/CreateModal
    // 回傳新增物料 Partial View 供全域 Modal 載入
    [HttpGet]
    public IActionResult CreateModal()
    {
        var newItem = new InventoryItem 
        { 
            StockQuantity = 10, 
            SafetyStock = 20, 
            Unit = "件" 
        };
        return PartialView("_CreateModal", newItem);
    }

    // POST: /Inventory/Create
    // AJAX 提交新增物料品項
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] InventoryItem item)
    {
        if (string.IsNullOrWhiteSpace(item.ItemCode) || string.IsNullOrWhiteSpace(item.ItemName) || string.IsNullOrWhiteSpace(item.Category))
        {
            return Json(new { success = false, message = "物料代碼、品名規格與分類皆為必填欄位！" });
        }

        item.ItemCode = item.ItemCode.Trim().ToUpper();
        item.ItemName = item.ItemName.Trim();
        item.Category = item.Category.Trim();

        // 代碼防呆：防止重複建立相同代碼
        bool exists = await _context.InventoryItems.AnyAsync(x => x.ItemCode == item.ItemCode);
        if (exists)
        {
            return Json(new { success = false, message = $"物料代碼【{item.ItemCode}】已存在，不可重複建立！" });
        }

        item.LastUpdated = DateTime.UtcNow;
        _context.InventoryItems.Add(item);
        await _context.SaveChangesAsync();

        return Json(new
        {
            success = true,
            message = $"新品項【{item.ItemName} ({item.ItemCode})】建立成功！",
            item = new
            {
                id = item.Id,
                itemCode = item.ItemCode,
                itemName = item.ItemName,
                category = item.Category,
                stockQuantity = item.StockQuantity,
                safetyStock = item.SafetyStock,
                unit = item.Unit,
                statusBadge = item.StatusBadgeClass,
                statusText = item.StatusText,
                lastUpdated = item.LastUpdated.ToString("yyyy-MM-dd HH:mm")
            }
        });
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

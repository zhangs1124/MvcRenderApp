using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcRenderApp.Data;
using MvcRenderApp.Models;

namespace MvcRenderApp.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _context.InventoryItems.ToListAsync();
        ViewBag.TotalItems = items.Count;
        ViewBag.WarningItems = items.Count(x => x.StockQuantity <= x.SafetyStock);
        ViewBag.OutOfStockItems = items.Count(x => x.StockQuantity <= 0);
        ViewBag.RecentItems = items.OrderByDescending(x => x.LastUpdated).Take(5).ToList();
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

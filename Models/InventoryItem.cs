using System.ComponentModel.DataAnnotations;

namespace MvcRenderApp.Models;

/// <summary>
/// 庫存物料實體模型
/// </summary>
public class InventoryItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    [Display(Name = "物料代碼")]
    public string ItemCode { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [Display(Name = "品名規格")]
    public string ItemName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    [Display(Name = "物料分類")]
    public string Category { get; set; } = string.Empty;

    [Display(Name = "現有存量")]
    public int StockQuantity { get; set; }

    [Display(Name = "安全水位")]
    public int SafetyStock { get; set; }

    [StringLength(10)]
    [Display(Name = "單位")]
    public string Unit { get; set; } = "件";

    [Display(Name = "最後異動時間")]
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 庫存水位狀態 (計算屬性)
    /// </summary>
    public string StatusBadgeClass
    {
        get
        {
            if (StockQuantity <= 0) return "bg-danger";
            if (StockQuantity <= SafetyStock) return "bg-warning text-dark";
            return "bg-success";
        }
    }

    public string StatusText
    {
        get
        {
            if (StockQuantity <= 0) return "缺料斷貨";
            if (StockQuantity <= SafetyStock) return "低於水位警戒";
            return "庫存充足";
        }
    }
}

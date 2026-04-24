namespace TechStoreWeb.Models;

public sealed class RevenueRow
{
    public int ProductID { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public decimal GiaTaiHQ { get; set; }

    public int SoLuongBan { get; set; }

    public decimal DoanhThu { get; set; }

    public DateTime? LanBanGanNhat { get; set; }
}

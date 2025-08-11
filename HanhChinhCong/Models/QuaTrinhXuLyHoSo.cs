using System;
using System.ComponentModel.DataAnnotations.Schema;

[Table("QuaTrinhXuLyHoSo")]
public class QuaTrinhXuLyHoSo
{
    public int Id { get; set; }
    public int IdHoSo { get; set; }
    public string Buoc { get; set; }           // PhanCong, XuLy, Duyet, TraKetQua
    public string GhiChu { get; set; }
    public string FileDinhKem { get; set; }    // Đường dẫn file hoặc nhiều file phân cách dấu phẩy
    public DateTime NgayThucHien { get; set; }
    public int? IdNguoiThucHien { get; set; }
}

public class QuaTrinhXuLyHoSoInfo
{
    public int Id { get; set; }
    public int IdHoSo { get; set; }
    public string Buoc { get; set; }
    public string GhiChu { get; set; }
    public string FileDinhKem { get; set; }
    public DateTime NgayThucHien { get; set; }
    public int? IdNguoiThucHien { get; set; }
    public string TenNguoiThucHien { get; set; } // Dùng cho hiển thị
}


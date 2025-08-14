using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HanhChinhCong.Models
{
    [Table("HoSo")]
    public class HoSo
    {
        public int Id { get; set; }
        public string MaHoSo { get; set; }
        public string TieuDe { get; set; }
        public string MoTa { get; set; }
        public DateTime? NgayTiepNhan { get; set; }
        public DateTime? HanXuLy { get; set; }
        public DateTime? NgayHoanThanh { get; set; }
        public string GhiChu { get; set; }
        public int? IdCanBoTiepNhan { get; set; }
        public int? IdPhongBan { get; set; }
        public int? IdLinhVuc { get; set; }
        public int? IdLoaiHoSo { get; set; }
        public int? IdTrangThai { get; set; }
        public string TenCongDan { get; set; }
        public string SoDienThoai { get; set; }
        public string CMND_CCCD { get; set; }
        public string DiaChi { get; set; }
        public string Email { get; set; }

        public int? IdCanBoXuLy { get; set; }
        public int? IdLanhDao { get; set; }
        public int? IdCanBoTraKetQua { get; set; }


    }

    public class HoSoInfo
    {
        public int Id { get; set; }
        public string MaHoSo { get; set; }
        public string TieuDe { get; set; }
        public string MoTa { get; set; }
        public DateTime? NgayTiepNhan { get; set; }
        public DateTime? HanXuLy { get; set; }
        public DateTime? NgayHoanThanh { get; set; }
        public string GhiChu { get; set; }
        public int? IdCanBoTiepNhan { get; set; }
        public int? IdCanBoXuLy { get; set; }
        public int? IdPhongBan { get; set; }
        public int? IdLinhVuc { get; set; }
        public int? IdLoaiHoSo { get; set; }
        public int? IdTrangThai { get; set; }
        public string TenCongDan { get; set; }
        public string SoDienThoai { get; set; }
        public string CMND_CCCD { get; set; }
        public string DiaChi { get; set; }
        public string Email { get; set; }

        // Các trường dùng cho hiển thị
        public string TenPhongBan { get; set; }
        public string TenLinhVuc { get; set; }
        public string TenLoaiHoSo { get; set; }
        public string TenCanBoTiepNhan { get; set; }
        public string TenCanBoXuLy { get; set; }
        public string TenLanhDao { get; set; }
        public string TenCanBoTraKetQua { get; set; }

        public string MaLoaiHoSo { get; set; }

    }

    public class HoSoDaXuLyViewModel
    {
        public int Id { get; set; }
        public string MaHoSo { get; set; }
        public string TieuDe { get; set; }
        public DateTime? NgayTiepNhan { get; set; }
        public string TenCongDan { get; set; }
        public string SoDienThoai { get; set; }
        public string CMND_CCCD { get; set; }
        public string DiaChi { get; set; }
        public string Email { get; set; }
        public int? IdTrangThai { get; set; }
        public string Buoc { get; set; }
        public DateTime? NgayXuLy { get; set; }
        public string GhiChu { get; set; }
    }




}
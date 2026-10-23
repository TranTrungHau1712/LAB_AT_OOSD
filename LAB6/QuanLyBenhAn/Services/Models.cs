using System;
using System.Collections.Generic;

namespace QuanLyBenhAn.Services
{
    /// <summary>Lỗi vi phạm quy tắc nghiệp vụ - thông báo trực tiếp cho người dùng.</summary>
    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message) { }
    }

    public class TrieuChungInput { public string Ten; public string GiaTri; public string DonVi; }
    public class XetNghiemInput { public string MaXN; public string KetQua; public string GhiChu; }

    public class PhieuKhamInput
    {
        public string MaHS, MaBS, MaKhoa, MaNV, MaLH, Loai, ChanDoan, KetLuan;
        public DateTime NgayGio;
        public List<TrieuChungInput> TrieuChung = new List<TrieuChungInput>();
        public List<XetNghiemInput> XetNghiem = new List<XetNghiemInput>();
    }

    public class ChiTietToaInput
    {
        public string MaThuoc; public int SoLuong; public string LieuDung; public string CachDung;
    }
}

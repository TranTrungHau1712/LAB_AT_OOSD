using System;
using System.Data;
using System.Data.SqlClient;

namespace QuanLyThuVien.Data
{
    public static class SachService
    {
        public static DataTable LayDanhSach()
        {
            return Db.Query(@"
                SELECT s.MaDauSach, s.TenSach, s.NamXuatBan, s.SoLuongHienCo,
                       s.MaTheLoai, tl.TenTheLoai,
                       s.MaNhaXuatBan, nxb.MaNhaXuatBan AS TenNXB
                FROM DauSach s
                JOIN TheLoai tl ON tl.MaTheLoai = s.MaTheLoai
                JOIN NhaXuatBan nxb ON nxb.MaNhaXuatBan = s.MaNhaXuatBan
                ORDER BY s.MaDauSach");
        }

        public static DataTable TimKiem(string tuKhoa)
        {
            return Db.Query(@"
                SELECT s.MaDauSach, s.TenSach, s.NamXuatBan, s.SoLuongHienCo,
                       s.MaTheLoai, tl.TenTheLoai,
                       s.MaNhaXuatBan, nxb.MaNhaXuatBan AS TenNXB
                FROM DauSach s
                JOIN TheLoai tl ON tl.MaTheLoai = s.MaTheLoai
                JOIN NhaXuatBan nxb ON nxb.MaNhaXuatBan = s.MaNhaXuatBan
                WHERE s.MaDauSach LIKE @tk OR s.TenSach LIKE @tk
                ORDER BY s.MaDauSach",
                new SqlParameter("@tk", "%" + tuKhoa + "%"));
        }

        public static KetQuaXuLy ThemSach(DauSach s)
        {
            try
            {
                Db.Execute(
                    "INSERT INTO DauSach(MaDauSach,TenSach,NamXuatBan,SoLuongHienCo,MaTheLoai,MaNhaXuatBan) " +
                    "VALUES(@ma,@ten,@nam,@sl,@tl,@nxb)",
                    new SqlParameter("@ma", s.MaDauSach),
                    new SqlParameter("@ten", s.TenSach),
                    new SqlParameter("@nam", s.NamXuatBan),
                    new SqlParameter("@sl", s.SoLuongHienCo),
                    new SqlParameter("@tl", s.MaTheLoai),
                    new SqlParameter("@nxb", s.MaNhaXuatBan));
                return KetQuaXuLy.Ok("Thêm đầu sách thành công.");
            }
            catch (SqlException ex)
            {
                return KetQuaXuLy.Loi("Lỗi: " + ex.Message);
            }
        }

        public static KetQuaXuLy CapNhatSach(DauSach s)
        {
            try
            {
                int rows = Db.Execute(
                    "UPDATE DauSach SET TenSach=@ten, NamXuatBan=@nam, SoLuongHienCo=@sl, MaTheLoai=@tl, MaNhaXuatBan=@nxb " +
                    "WHERE MaDauSach=@ma",
                    new SqlParameter("@ten", s.TenSach),
                    new SqlParameter("@nam", s.NamXuatBan),
                    new SqlParameter("@sl", s.SoLuongHienCo),
                    new SqlParameter("@tl", s.MaTheLoai),
                    new SqlParameter("@nxb", s.MaNhaXuatBan),
                    new SqlParameter("@ma", s.MaDauSach));
                return rows > 0 ? KetQuaXuLy.Ok("Cập nhật thành công.") : KetQuaXuLy.Loi("Không tìm thấy mã đầu sách.");
            }
            catch (SqlException ex)
            {
                return KetQuaXuLy.Loi("Lỗi: " + ex.Message);
            }
        }

        public static KetQuaXuLy XoaSach(string maSach)
        {
            try
            {
                int rows = Db.Execute("DELETE FROM DauSach WHERE MaDauSach=@ma", new SqlParameter("@ma", maSach));
                return rows > 0 ? KetQuaXuLy.Ok("Xóa thành công.") : KetQuaXuLy.Loi("Không tìm thấy mã đầu sách.");
            }
            catch (SqlException ex)
            {
                return KetQuaXuLy.Loi("Không thể xóa: đầu sách này đang được mượn hoặc tham chiếu ở dữ liệu khác.\n" + ex.Message);
            }
        }
        public static DataTable LaySachConTrongKho()
        {
            return Db.Query("SELECT MaDauSach, TenSach, SoLuongHienCo FROM DauSach WHERE SoLuongHienCo > 0 ORDER BY MaDauSach");
        }
    }
}
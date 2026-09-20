using System;
using System.Data;
using System.Data.SqlClient;

namespace QuanLyThuVien.Data
{
    public static class DanhMucService
    {
        // ===== NHÂN VIÊN =====
        public static DataTable LayDanhSachNhanVien()
        {
            return Db.Query("SELECT MaNhanVien, Ho, Ten, Phai, NgaySinh, ChucVu, SoDienThoai FROM NhanVien ORDER BY MaNhanVien");
        }

        public static KetQuaXuLy ThemNhanVien(NhanVien nv)
        {
            try
            {
                Db.Execute(
                    "INSERT INTO NhanVien(MaNhanVien,Ho,Ten,Phai,NgaySinh,ChucVu,SoDienThoai) VALUES(@ma,@ho,@ten,@phai,@ns,@cv,@sdt)",
                    new SqlParameter("@ma", nv.MaNhanVien),
                    new SqlParameter("@ho", nv.Ho),
                    new SqlParameter("@ten", nv.Ten),
                    new SqlParameter("@phai", nv.Phai),
                    new SqlParameter("@ns", nv.NgaySinh),
                    new SqlParameter("@cv", nv.ChucVu),
                    new SqlParameter("@sdt", (object)nv.SoDienThoai ?? DBNull.Value));
                return KetQuaXuLy.Ok("Thêm nhân viên thành công.");
            }
            catch (SqlException ex)
            {
                return KetQuaXuLy.Loi("Lỗi: " + ex.Message);
            }
        }

        public static KetQuaXuLy CapNhatNhanVien(NhanVien nv)
        {
            try
            {
                int rows = Db.Execute(
                    "UPDATE NhanVien SET Ho=@ho, Ten=@ten, Phai=@phai, NgaySinh=@ns, ChucVu=@cv, SoDienThoai=@sdt WHERE MaNhanVien=@ma",
                    new SqlParameter("@ho", nv.Ho),
                    new SqlParameter("@ten", nv.Ten),
                    new SqlParameter("@phai", nv.Phai),
                    new SqlParameter("@ns", nv.NgaySinh),
                    new SqlParameter("@cv", nv.ChucVu),
                    new SqlParameter("@sdt", (object)nv.SoDienThoai ?? DBNull.Value),
                    new SqlParameter("@ma", nv.MaNhanVien));
                return rows > 0 ? KetQuaXuLy.Ok("Cập nhật thành công.") : KetQuaXuLy.Loi("Không tìm thấy mã nhân viên.");
            }
            catch (SqlException ex)
            {
                return KetQuaXuLy.Loi("Lỗi: " + ex.Message);
            }
        }

        public static KetQuaXuLy XoaNhanVien(string maNV)
        {
            try
            {
                int rows = Db.Execute("DELETE FROM NhanVien WHERE MaNhanVien=@ma", new SqlParameter("@ma", maNV));
                return rows > 0 ? KetQuaXuLy.Ok("Xóa thành công.") : KetQuaXuLy.Loi("Không tìm thấy mã nhân viên.");
            }
            catch (SqlException ex)
            {
                return KetQuaXuLy.Loi("Không thể xóa: nhân viên này đang được tham chiếu ở dữ liệu khác.\n" + ex.Message);
            }
        }

        // ===== THỂ LOẠI =====
        public static DataTable LayDanhSachTheLoai()
        {
            return Db.Query("SELECT MaTheLoai, TenTheLoai FROM TheLoai ORDER BY MaTheLoai");
        }

        public static KetQuaXuLy ThemTheLoai(TheLoai tl)
        {
            try
            {
                Db.Execute("INSERT INTO TheLoai(MaTheLoai,TenTheLoai) VALUES(@ma,@ten)",
                    new SqlParameter("@ma", tl.MaTheLoai),
                    new SqlParameter("@ten", tl.TenTheLoai));
                return KetQuaXuLy.Ok("Thêm thể loại thành công.");
            }
            catch (SqlException ex)
            {
                return KetQuaXuLy.Loi("Lỗi: " + ex.Message);
            }
        }

        public static KetQuaXuLy CapNhatTheLoai(TheLoai tl)
        {
            try
            {
                int rows = Db.Execute("UPDATE TheLoai SET TenTheLoai=@ten WHERE MaTheLoai=@ma",
                    new SqlParameter("@ten", tl.TenTheLoai),
                    new SqlParameter("@ma", tl.MaTheLoai));
                return rows > 0 ? KetQuaXuLy.Ok("Cập nhật thành công.") : KetQuaXuLy.Loi("Không tìm thấy mã thể loại.");
            }
            catch (SqlException ex)
            {
                return KetQuaXuLy.Loi("Lỗi: " + ex.Message);
            }
        }

        public static KetQuaXuLy XoaTheLoai(string maTL)
        {
            try
            {
                int rows = Db.Execute("DELETE FROM TheLoai WHERE MaTheLoai=@ma", new SqlParameter("@ma", maTL));
                return rows > 0 ? KetQuaXuLy.Ok("Xóa thành công.") : KetQuaXuLy.Loi("Không tìm thấy mã thể loại.");
            }
            catch (SqlException ex)
            {
                return KetQuaXuLy.Loi("Không thể xóa: thể loại này đang được đầu sách sử dụng.\n" + ex.Message);
            }
        }

        // ===== NHÀ XUẤT BẢN =====
        public static DataTable LayDanhSachNhaXuatBan()
        {
            return Db.Query("SELECT MaNhaXuatBan, DiaChi, SoDienThoai FROM NhaXuatBan ORDER BY MaNhaXuatBan");
        }

        public static KetQuaXuLy ThemNhaXuatBan(NhaXuatBan nxb)
        {
            try
            {
                Db.Execute("INSERT INTO NhaXuatBan(MaNhaXuatBan,DiaChi,SoDienThoai) VALUES(@ma,@dc,@sdt)",
                    new SqlParameter("@ma", nxb.MaNhaXuatBan),
                    new SqlParameter("@dc", (object)nxb.DiaChi ?? DBNull.Value),
                    new SqlParameter("@sdt", (object)nxb.SoDienThoai ?? DBNull.Value));
                return KetQuaXuLy.Ok("Thêm nhà xuất bản thành công.");
            }
            catch (SqlException ex)
            {
                return KetQuaXuLy.Loi("Lỗi: " + ex.Message);
            }
        }

        public static KetQuaXuLy CapNhatNhaXuatBan(NhaXuatBan nxb)
        {
            try
            {
                int rows = Db.Execute("UPDATE NhaXuatBan SET DiaChi=@dc, SoDienThoai=@sdt WHERE MaNhaXuatBan=@ma",
                    new SqlParameter("@dc", (object)nxb.DiaChi ?? DBNull.Value),
                    new SqlParameter("@sdt", (object)nxb.SoDienThoai ?? DBNull.Value),
                    new SqlParameter("@ma", nxb.MaNhaXuatBan));
                return rows > 0 ? KetQuaXuLy.Ok("Cập nhật thành công.") : KetQuaXuLy.Loi("Không tìm thấy mã nhà xuất bản.");
            }
            catch (SqlException ex)
            {
                return KetQuaXuLy.Loi("Lỗi: " + ex.Message);
            }
        }

        public static KetQuaXuLy XoaNhaXuatBan(string maNXB)
        {
            try
            {
                int rows = Db.Execute("DELETE FROM NhaXuatBan WHERE MaNhaXuatBan=@ma", new SqlParameter("@ma", maNXB));
                return rows > 0 ? KetQuaXuLy.Ok("Xóa thành công.") : KetQuaXuLy.Loi("Không tìm thấy mã nhà xuất bản.");
            }
            catch (SqlException ex)
            {
                return KetQuaXuLy.Loi("Không thể xóa: nhà xuất bản này đang được đầu sách sử dụng.\n" + ex.Message);
            }
        }
    }
}
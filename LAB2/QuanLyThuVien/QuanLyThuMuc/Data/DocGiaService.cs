using System;
using System.Data;
using System.Data.SqlClient;

namespace QuanLyThuVien.Data
{
    public static class DocGiaService
    {
        public static DataTable LayDanhSach()
        {
            return Db.Query(@"
                SELECT dg.MaDocGia, dg.Ho, dg.Ten, dg.NgaySinh, dg.Phai,
                       dg.SoDienThoai, dg.DiaChi, dg.Email, dg.Anh3x4,
                       t.NgayCap, t.HanSuDung, t.DaDongLePhi, t.TrangThai
                FROM DocGia dg
                LEFT JOIN TheDocGia t ON t.MaDocGia = dg.MaDocGia AND t.TrangThai = 1
                ORDER BY dg.MaDocGia");
        }

        public static KetQuaXuLy ThemDocGia(DocGia dg)
        {
            try
            {
                Db.Execute(
                    "INSERT INTO DocGia(MaDocGia,Ho,Ten,NgaySinh,Phai,SoDienThoai,DiaChi,Email,Anh3x4) " +
                    "VALUES(@ma,@ho,@ten,@ns,@phai,@sdt,@dc,@email,@anh)",
                    new SqlParameter("@ma", dg.MaDocGia),
                    new SqlParameter("@ho", dg.Ho),
                    new SqlParameter("@ten", dg.Ten),
                    new SqlParameter("@ns", dg.NgaySinh),
                    new SqlParameter("@phai", dg.Phai),
                    new SqlParameter("@sdt", (object)dg.SoDienThoai ?? DBNull.Value),
                    new SqlParameter("@dc", dg.DiaChi),
                    new SqlParameter("@email", dg.Email),
                    new SqlParameter("@anh", (object)dg.Anh3x4 ?? DBNull.Value));
                return KetQuaXuLy.Ok("Thêm độc giả thành công.");
            }
            catch (SqlException ex)
            {
                return KetQuaXuLy.Loi("Lỗi: " + ex.Message);
            }
        }

        public static KetQuaXuLy CapNhatDocGia(DocGia dg)
        {
            try
            {
                int rows = Db.Execute(
                    "UPDATE DocGia SET Ho=@ho, Ten=@ten, NgaySinh=@ns, Phai=@phai, SoDienThoai=@sdt, DiaChi=@dc, Email=@email, Anh3x4=@anh " +
                    "WHERE MaDocGia=@ma",
                    new SqlParameter("@ho", dg.Ho),
                    new SqlParameter("@ten", dg.Ten),
                    new SqlParameter("@ns", dg.NgaySinh),
                    new SqlParameter("@phai", dg.Phai),
                    new SqlParameter("@sdt", (object)dg.SoDienThoai ?? DBNull.Value),
                    new SqlParameter("@dc", dg.DiaChi),
                    new SqlParameter("@email", dg.Email),
                    new SqlParameter("@anh", (object)dg.Anh3x4 ?? DBNull.Value),
                    new SqlParameter("@ma", dg.MaDocGia));
                return rows > 0 ? KetQuaXuLy.Ok("Cập nhật thành công.") : KetQuaXuLy.Loi("Không tìm thấy mã độc giả.");
            }
            catch (SqlException ex)
            {
                return KetQuaXuLy.Loi("Lỗi: " + ex.Message);
            }
        }

        public static KetQuaXuLy CapThe(string maDocGia, DateTime ngayCap, DateTime hanSuDung, bool daDongLePhi)
        {
            try
            {
                // Vô hiệu hóa thẻ cũ (nếu có) trước khi cấp thẻ mới
                Db.Execute("UPDATE TheDocGia SET TrangThai = 0 WHERE MaDocGia = @ma AND TrangThai = 1",
                    new SqlParameter("@ma", maDocGia));

                string maThe = "THE_" + maDocGia + "_" + ngayCap.Year;
                Db.Execute(
                    "INSERT INTO TheDocGia(MaThe,MaDocGia,NgayCap,HanSuDung,DaDongLePhi,TrangThai) " +
                    "VALUES(@mt,@ma,@nc,@han,@lp,1)",
                    new SqlParameter("@mt", maThe),
                    new SqlParameter("@ma", maDocGia),
                    new SqlParameter("@nc", ngayCap),
                    new SqlParameter("@han", hanSuDung),
                    new SqlParameter("@lp", daDongLePhi));
                return KetQuaXuLy.Ok("Cấp thẻ thành công.");
            }
            catch (SqlException ex)
            {
                return KetQuaXuLy.Loi("Lỗi: " + ex.Message);
            }
        }

        public static KetQuaXuLy GiaHan(string maDocGia, DateTime hanMoi)
        {
            try
            {
                int rows = Db.Execute(
                    "UPDATE TheDocGia SET HanSuDung = @han WHERE MaDocGia = @ma AND TrangThai = 1",
                    new SqlParameter("@han", hanMoi),
                    new SqlParameter("@ma", maDocGia));
                return rows > 0 ? KetQuaXuLy.Ok("Gia hạn thành công.") : KetQuaXuLy.Loi("Độc giả chưa có thẻ đang hoạt động để gia hạn.");
            }
            catch (SqlException ex)
            {
                return KetQuaXuLy.Loi("Lỗi: " + ex.Message);
            }
        }
    }
}
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace NhanSu
{
    /// <summary>Tính bảng lương hằng tháng bằng thủ tục sp_TinhLuongThang.</summary>
    public class FrmTinhLuong : Form
    {
        readonly NumericUpDown numThang = new NumericUpDown { Minimum = 1, Maximum = 12, Value = DateTime.Today.Month, Width = 60 };
        readonly NumericUpDown numNam = new NumericUpDown { Minimum = 2000, Maximum = 2100, Value = DateTime.Today.Year, Width = 80 };
        readonly TextBox txtNguoiLap = new TextBox { Text = "Kế toán", Width = 150 };
        readonly DataGridView grid = UI.Grid();
        readonly Label lblTong = UI.Lbl("", true);

        public FrmTinhLuong()
        {
            Text = "Tính lương hằng tháng";
            Size = new Size(1150, 600);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F);

            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 46, Padding = new Padding(6, 8, 0, 0) };
            top.Controls.Add(UI.Lbl("Tháng:"));
            top.Controls.Add(numThang);
            top.Controls.Add(UI.Lbl("Năm:"));
            top.Controls.Add(numNam);
            top.Controls.Add(UI.Lbl("Người lập:"));
            top.Controls.Add(txtNguoiLap);
            top.Controls.Add(UI.Btn("Tính lương", (s, e) => Tinh(), 100));
            top.Controls.Add(UI.Btn("Xem bảng đã tính", (s, e) => Xem(), 130));
            foreach (Control c in top.Controls) if (c is Label) c.Margin = new Padding(8, 7, 2, 0);

            var bottom = new Panel { Dock = DockStyle.Bottom, Height = 34, Padding = new Padding(8, 8, 0, 0) };
            bottom.Controls.Add(lblTong);

            Controls.Add(grid);
            Controls.Add(bottom);
            Controls.Add(top);
        }

        void Hien(DataTable dt)
        {
            grid.DataSource = dt;
            UI.FormatColumns(grid);
            decimal tong = 0;
            foreach (DataRow r in dt.Rows) tong += r["ThucLanh"] == DBNull.Value ? 0 : (decimal)r["ThucLanh"];
            lblTong.Text = "Tổng thực lãnh: " + tong.ToString("N0") + " đ  (" + dt.Rows.Count + " nhân viên)";
        }

        void Tinh()
        {
            try
            {
                if (!UI.Confirm("Tính lương tháng " + numThang.Value + "/" + numNam.Value + "? Bảng lương cũ của tháng này (nếu có) sẽ bị tính lại."))
                    return;
                var dt = new DataTable();
                using (var cn = new SqlConnection(Db.ConnStr))
                using (var cmd = new SqlCommand("sp_TinhLuongThang", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 120;
                    cmd.Parameters.AddWithValue("@Thang", (int)numThang.Value);
                    cmd.Parameters.AddWithValue("@Nam", (int)numNam.Value);
                    cmd.Parameters.AddWithValue("@NguoiLap", txtNguoiLap.Text.Trim());
                    new SqlDataAdapter(cmd).Fill(dt);
                }
                Hien(dt);
            }
            catch (Exception ex) { UI.Err(ex); }
        }

        void Xem()
        {
            try
            {
                var dt = Db.Query(
                    "SELECT b.MaBL, b.MaNV, nv.HoTen, b.LuongCoBan, b.TongPhuCap, b.TienPhuTroi, b.TienThuong, b.KhauTruBH, b.TienPhat, b.ThucLanh " +
                    "FROM ChiTietBangLuong b JOIN BangLuong l ON l.MaBL=b.MaBL JOIN NhanVien nv ON nv.MaNV=b.MaNV " +
                    "WHERE l.Thang=@t AND l.Nam=@n ORDER BY b.MaNV",
                    Db.P("@t", (int)numThang.Value), Db.P("@n", (int)numNam.Value));
                if (dt.Rows.Count == 0) UI.Info("Chưa có bảng lương tháng này.");
                Hien(dt);
            }
            catch (Exception ex) { UI.Err(ex); }
        }
    }
}

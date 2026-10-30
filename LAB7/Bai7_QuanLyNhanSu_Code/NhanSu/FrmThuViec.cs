using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace NhanSu
{
    /// <summary>Theo dõi nhân viên thử việc, chấm dứt hoặc ký hợp đồng chính thức (Câu 4, 5).</summary>
    public class FrmThuViec : Form
    {
        readonly DataGridView grid = UI.Grid();

        public FrmThuViec()
        {
            Text = "Thử việc và ký hợp đồng chính thức";
            Size = new Size(1150, 560);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F);

            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(6, 6, 0, 0) };
            top.Controls.Add(UI.Btn("Ký hợp đồng chính thức...", (s, e) => KyHopDong(), 190));
            top.Controls.Add(UI.Btn("Chấm dứt thử việc", (s, e) => ChamDut(), 140));
            top.Controls.Add(UI.Btn("Làm mới", (s, e) => LoadData(), 90));
            var note = UI.Lbl("Dòng tô vàng: đã hết thời gian thử việc, cần xét kết quả.");
            note.Margin = new Padding(14, 9, 0, 0);
            top.Controls.Add(note);

            Controls.Add(grid);
            Controls.Add(top);
            LoadData();
        }

        void LoadData()
        {
            try
            {
                var dt = Db.Query("SELECT MaNV, HoTen, MaPB, TenPB, SoQD, NgayBatDauThuViec, ThoiGianThuViec, LuongThuViec, NgayHetThuViec " +
                                  "FROM vw_ThuViec ORDER BY NgayHetThuViec");
                grid.DataSource = dt;
                UI.FormatColumns(grid);
                foreach (DataGridViewRow row in grid.Rows)
                {
                    var r = ((DataRowView)row.DataBoundItem).Row;
                    if ((DateTime)r["NgayHetThuViec"] <= DateTime.Today) row.DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
                }
            }
            catch (Exception ex) { UI.Err(ex); }
        }

        DataRow Current()
        {
            if (grid.CurrentRow == null) { UI.Warn("Chọn một nhân viên đang thử việc."); return null; }
            return ((DataRowView)grid.CurrentRow.DataBoundItem).Row;
        }

        void ChamDut()
        {
            var r = Current();
            if (r == null) return;
            if (!UI.Confirm("Chấm dứt thử việc của " + r["HoTen"] + "?")) return;
            try
            {
                Db.Exec("UPDATE NhanVien SET TrangThai = N'Thôi việc' WHERE MaNV = @m", Db.P("@m", r["MaNV"]));
                LoadData();
            }
            catch (Exception ex) { UI.Err(ex); }
        }

        void KyHopDong()
        {
            var r = Current();
            if (r == null) return;
            using (var dlg = new HopDongDialog(r))
                if (dlg.ShowDialog(this) == DialogResult.OK) LoadData();
        }
    }

    /// <summary>Hộp thoại lập hợp đồng chính thức, dựng bằng code.</summary>
    public class HopDongDialog : Form
    {
        readonly DataRow nv;
        readonly TextBox txtSoHD = new TextBox { ReadOnly = true, Width = 220 };
        readonly ComboBox cboLoai = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 220 };
        readonly DateTimePicker dtKy = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 220 };
        readonly DateTimePicker dtBatDau = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 220 };
        readonly NumericUpDown numThoiHan = new NumericUpDown { Minimum = 1, Maximum = 120, Value = 12, Width = 220 };
        readonly NumericUpDown numLuong = new NumericUpDown { Minimum = 0, Maximum = 1000000000, Increment = 100000, ThousandsSeparator = true, Width = 220 };
        readonly CheckBox chkBH = new CheckBox { Text = "Lập hồ sơ BHYT, BHXH", Checked = true, AutoSize = true };

        public HopDongDialog(DataRow row)
        {
            nv = row;
            Text = "Ký hợp đồng chính thức - " + row["HoTen"];
            Size = new Size(430, 380);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false; MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F);

            txtSoHD.Text = "HD" + DateTime.Now.ToString("yyyyMMddHHmmss");
            cboLoai.Items.AddRange(new object[] { "Dài hạn", "Ngắn hạn" });
            cboLoai.SelectedIndex = 0;
            numLuong.Value = (decimal)row["LuongThuViec"];
            numThoiHan.Enabled = false;

            var tl = new TableLayoutPanel { Dock = DockStyle.Top, Height = 270, ColumnCount = 2, RowCount = 7, Padding = new Padding(10) };
            tl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            tl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            AddRow(tl, 0, "Số hợp đồng:", txtSoHD);
            AddRow(tl, 1, "Loại hợp đồng:", cboLoai);
            AddRow(tl, 2, "Ngày ký:", dtKy);
            AddRow(tl, 3, "Ngày bắt đầu:", dtBatDau);
            AddRow(tl, 4, "Thời hạn (tháng):", numThoiHan);
            AddRow(tl, 5, "Lương thỏa thuận:", numLuong);
            AddRow(tl, 6, "", chkBH);
            Controls.Add(tl);

            var pnl = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 46, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(8, 6, 8, 0) };
            pnl.Controls.Add(UI.Btn("Hủy", (s, e) => DialogResult = DialogResult.Cancel, 90));
            pnl.Controls.Add(UI.Btn("Ký hợp đồng", (s, e) => Luu(), 110));
            Controls.Add(pnl);

            cboLoai.SelectedIndexChanged += (s, e) =>
            {
                bool dai = cboLoai.SelectedIndex == 0;
                numThoiHan.Enabled = !dai;
                chkBH.Enabled = dai;
                chkBH.Checked = dai;
            };
        }

        static void AddRow(TableLayoutPanel tl, int row, string label, Control c)
        {
            tl.Controls.Add(UI.Lbl(label), 0, row);
            tl.Controls.Add(c, 1, row);
        }

        void Luu()
        {
            try
            {
                bool dai = cboLoai.SelectedIndex == 0;
                string soHD = txtSoHD.Text;
                string maNV = nv["MaNV"].ToString();
                DateTime bd = dtBatDau.Value.Date;
                object ketThuc = dai ? null : (object)bd.AddMonths((int)numThoiHan.Value).AddDays(-1);
                string stamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                Db.Tran((cn, tx) =>
                {
                    Db.Run(cn, tx, "INSERT INTO HopDong(SoHD, LoaiHD, NgayKy, NgayBatDau, NgayKetThuc, LuongThoaThuan, MaNV) VALUES(@s,@l,@k,@b,@e,@lg,@nv)",
                        Db.P("@s", soHD), Db.P("@l", dai ? "Dài hạn" : "Ngắn hạn"), Db.P("@k", dtKy.Value.Date), Db.P("@b", bd),
                        Db.P("@e", ketThuc), Db.P("@lg", numLuong.Value), Db.P("@nv", maNV));
                    if (dai)
                    {
                        Db.Run(cn, tx, "INSERT INTO HopDongDaiHan(SoHD, CoBaoHiem) VALUES(@s,@bh)", Db.P("@s", soHD), Db.P("@bh", chkBH.Checked));
                        if (chkBH.Checked)
                        {
                            // Mức đóng của người lao động: BHYT 1,5%; BHXH 8%
                            Db.Run(cn, tx, "INSERT INTO HoSoBaoHiem(MaBH, LoaiBH, SoSo, NgayThamGia, MucDong, SoHD) VALUES(@m, N'BHYT', NULL, @n, 1.5, @s)",
                                Db.P("@m", "BH" + stamp + "Y"), Db.P("@n", bd), Db.P("@s", soHD));
                            Db.Run(cn, tx, "INSERT INTO HoSoBaoHiem(MaBH, LoaiBH, SoSo, NgayThamGia, MucDong, SoHD) VALUES(@m, N'BHXH', NULL, @n, 8, @s)",
                                Db.P("@m", "BH" + stamp + "X"), Db.P("@n", bd), Db.P("@s", soHD));
                        }
                    }
                    else
                        Db.Run(cn, tx, "INSERT INTO HopDongNganHan(SoHD, ThoiHan) VALUES(@s,@t)", Db.P("@s", soHD), Db.P("@t", (int)numThoiHan.Value));

                    // Quyết định lương sau thử việc (giám đốc ký cùng hợp đồng)
                    string soQD = "LU" + stamp;
                    Db.Run(cn, tx, "INSERT INTO QuyetDinh(SoQD, LoaiQD, NgayLap, NgayHieuLuc, NoiDung, NguoiKy, TrangThai, MaNV, MaPB) " +
                                   "VALUES(@q,'LUONG',@nl,@hl,N'Quyết định lương sau thử việc',N'Giám đốc',N'Đã ký',@nv,NULL)",
                        Db.P("@q", soQD), Db.P("@nl", DateTime.Today), Db.P("@hl", bd), Db.P("@nv", maNV));
                    Db.Run(cn, tx, "INSERT INTO QDLuong(SoQD, LuongCoBan, PhuCapLuong, PhuCapDocHai) VALUES(@q,@l,0,0)",
                        Db.P("@q", soQD), Db.P("@l", numLuong.Value));

                    Db.Run(cn, tx, "UPDATE NhanVien SET TrangThai = N'Chính thức' WHERE MaNV = @nv", Db.P("@nv", maNV));
                });
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex) { UI.Err(ex); }
        }
    }
}

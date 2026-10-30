using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace NhanSu
{
    /// <summary>Lập quyết định khen thưởng cho cá nhân hoặc tập thể phòng ban, giám đốc ký duyệt (Câu 6).</summary>
    public class FrmKhenThuong : Form
    {
        readonly RadioButton rbCaNhan = new RadioButton { Text = "Cá nhân", Checked = true, AutoSize = true };
        readonly RadioButton rbTapThe = new RadioButton { Text = "Tập thể phòng ban", AutoSize = true };
        readonly ComboBox cboDoiTuong = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 300 };
        readonly ComboBox cboHinhThuc = new ComboBox { DropDownStyle = ComboBoxStyle.DropDown, Width = 300 };
        readonly TextBox txtSoTien = new TextBox { Width = 140 };
        readonly TextBox txtLyDo = new TextBox { Width = 300 };
        readonly DateTimePicker dtHieuLuc = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 140 };
        readonly ComboBox cboChi = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 140 };
        readonly DataGridView grid = UI.Grid();

        public FrmKhenThuong()
        {
            Text = "Lập quyết định khen thưởng";
            Size = new Size(1150, 700);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F);

            cboHinhThuc.Items.AddRange(new object[] { "Thưởng ngày lễ", "Thưởng cuối năm", "Lương tháng 13", "Thưởng thành tích", "Thưởng tập thể" });
            cboChi.Items.AddRange(new object[] { "Tiền mặt", "Cộng vào lương" });
            cboChi.SelectedIndex = 0;

            var box = new GroupBox { Dock = DockStyle.Top, Height = 190, Text = "Thông tin quyết định" };
            var tl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 4, Padding = new Padding(8) };
            tl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            tl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 320));
            tl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            tl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            var pnlLoai = new FlowLayoutPanel { AutoSize = true };
            pnlLoai.Controls.Add(rbCaNhan);
            pnlLoai.Controls.Add(rbTapThe);
            tl.Controls.Add(UI.Lbl("Đối tượng:"), 0, 0);
            tl.Controls.Add(pnlLoai, 1, 0);
            tl.Controls.Add(UI.Lbl("Ngày hiệu lực:"), 2, 0);
            tl.Controls.Add(dtHieuLuc, 3, 0);
            tl.Controls.Add(UI.Lbl("Chọn:"), 0, 1);
            tl.Controls.Add(cboDoiTuong, 1, 1);
            tl.Controls.Add(UI.Lbl("Số tiền thưởng:"), 2, 1);
            tl.Controls.Add(txtSoTien, 3, 1);
            tl.Controls.Add(UI.Lbl("Hình thức:"), 0, 2);
            tl.Controls.Add(cboHinhThuc, 1, 2);
            tl.Controls.Add(UI.Lbl("Hình thức chi:"), 2, 2);
            tl.Controls.Add(cboChi, 3, 2);
            tl.Controls.Add(UI.Lbl("Lý do:"), 0, 3);
            tl.Controls.Add(txtLyDo, 1, 3);
            tl.Controls.Add(UI.Btn("Lập quyết định", (s, e) => Lap(), 130), 3, 3);
            box.Controls.Add(tl);

            var mid = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(6, 6, 0, 0) };
            mid.Controls.Add(UI.Btn("Giám đốc ký duyệt", (s, e) => Duyet(true), 150));
            mid.Controls.Add(UI.Btn("Từ chối", (s, e) => Duyet(false), 100));
            mid.Controls.Add(UI.Btn("Làm mới", (s, e) => LoadGrid(), 100));
            var note = UI.Lbl("Khi ký duyệt, hệ thống tự lập phiếu chi khen thưởng theo \"Hình thức chi\" đã chọn.");
            note.Margin = new Padding(14, 9, 0, 0);
            mid.Controls.Add(note);

            Controls.Add(grid);
            Controls.Add(mid);
            Controls.Add(box);

            rbCaNhan.CheckedChanged += (s, e) => LoadDoiTuong();
            LoadDoiTuong();
            LoadGrid();
        }

        void LoadDoiTuong()
        {
            try
            {
                DataTable dt = rbCaNhan.Checked
                    ? Db.Query("SELECT MaNV AS V, MaNV + N' - ' + HoTen AS T FROM NhanVien WHERE TrangThai <> N'Thôi việc' ORDER BY MaNV")
                    : Db.Query("SELECT MaPB AS V, MaPB + N' - ' + TenPB AS T FROM PhongBan ORDER BY MaPB");
                cboDoiTuong.DataSource = null;
                cboDoiTuong.ValueMember = "V";
                cboDoiTuong.DisplayMember = "T";
                cboDoiTuong.DataSource = dt;
            }
            catch (Exception ex) { UI.Err(ex); }
        }

        void LoadGrid()
        {
            try
            {
                var dt = Db.Query(
                    "SELECT q.SoQD, q.NgayLap, k.DoiTuong, ISNULL(q.MaNV, q.MaPB) AS MaDoiTuong, ISNULL(nv.HoTen, pb.TenPB) AS TenDoiTuong, " +
                    "k.HinhThuc, k.SoTienThuong, k.LyDo, q.TrangThai " +
                    "FROM QuyetDinh q JOIN QDKhenThuong k ON k.SoQD = q.SoQD " +
                    "LEFT JOIN NhanVien nv ON nv.MaNV = q.MaNV LEFT JOIN PhongBan pb ON pb.MaPB = q.MaPB " +
                    "WHERE q.LoaiQD = 'KHENTHUONG' ORDER BY q.NgayLap DESC, q.SoQD DESC");
                grid.DataSource = dt;
                UI.FormatColumns(grid);
                if (grid.Columns.Contains("MaDoiTuong")) grid.Columns["MaDoiTuong"].HeaderText = "Mã đối tượng";
            }
            catch (Exception ex) { UI.Err(ex); }
        }

        void Lap()
        {
            try
            {
                if (cboDoiTuong.SelectedValue == null) { UI.Warn("Chọn đối tượng được khen thưởng."); return; }
                if (string.IsNullOrWhiteSpace(cboHinhThuc.Text)) { UI.Warn("Nhập hình thức khen thưởng."); return; }
                decimal tien;
                if (!decimal.TryParse(txtSoTien.Text.Trim().Replace(",", "").Replace(".", ""), out tien) || tien <= 0)
                {
                    UI.Warn("Số tiền thưởng phải là số lớn hơn 0 (nhập không có dấu phân cách).");
                    return;
                }
                string ma = cboDoiTuong.SelectedValue.ToString();
                bool caNhan = rbCaNhan.Checked;
                string soQD = "KT" + DateTime.Now.ToString("yyyyMMddHHmmss");
                string hinhThuc = cboHinhThuc.Text.Trim();
                Db.Tran((cn, tx) =>
                {
                    Db.Run(cn, tx,
                        "INSERT INTO QuyetDinh(SoQD, LoaiQD, NgayLap, NgayHieuLuc, NoiDung, NguoiKy, TrangThai, MaNV, MaPB) " +
                        "VALUES(@s, 'KHENTHUONG', @nl, @hl, @nd, NULL, N'Chờ duyệt', @nv, @pb)",
                        Db.P("@s", soQD), Db.P("@nl", DateTime.Today), Db.P("@hl", dtHieuLuc.Value.Date),
                        Db.P("@nd", "Khen thưởng: " + hinhThuc),
                        Db.P("@nv", caNhan ? ma : null), Db.P("@pb", caNhan ? null : ma));
                    Db.Run(cn, tx,
                        "INSERT INTO QDKhenThuong(SoQD, DoiTuong, HinhThuc, SoTienThuong, LyDo) VALUES(@s, @dt, @ht, @tien, @ld)",
                        Db.P("@s", soQD), Db.P("@dt", caNhan ? "Cá nhân" : "Tập thể"), Db.P("@ht", hinhThuc),
                        Db.P("@tien", tien), Db.P("@ld", string.IsNullOrWhiteSpace(txtLyDo.Text) ? null : txtLyDo.Text.Trim()));
                });
                txtSoTien.Text = ""; txtLyDo.Text = "";
                LoadGrid();
                UI.Info("Đã lập quyết định " + soQD + " (chờ giám đốc duyệt).");
            }
            catch (Exception ex) { UI.Err(ex); }
        }

        void Duyet(bool dongY)
        {
            try
            {
                if (grid.CurrentRow == null) { UI.Warn("Chọn một quyết định trong danh sách."); return; }
                var r = ((DataRowView)grid.CurrentRow.DataBoundItem).Row;
                if (r["TrangThai"].ToString() != "Chờ duyệt") { UI.Warn("Chỉ xử lý được quyết định đang ở trạng thái Chờ duyệt."); return; }
                string soQD = r["SoQD"].ToString();
                decimal tien = (decimal)r["SoTienThuong"];
                Db.Tran((cn, tx) =>
                {
                    Db.Run(cn, tx, "UPDATE QuyetDinh SET TrangThai=@tt, NguoiKy=N'Giám đốc' WHERE SoQD=@s",
                        Db.P("@tt", dongY ? "Đã ký" : "Từ chối"), Db.P("@s", soQD));
                    if (dongY)
                        Db.Run(cn, tx,
                            "INSERT INTO PhieuChiKhenThuong(SoPhieuChi, SoQD, NgayChi, SoTien, HinhThucChi) VALUES(@p, @s, @n, @t, @h)",
                            Db.P("@p", "PC" + soQD.Substring(2)), Db.P("@s", soQD), Db.P("@n", DateTime.Today),
                            Db.P("@t", tien), Db.P("@h", cboChi.SelectedItem.ToString()));
                });
                LoadGrid();
            }
            catch (Exception ex) { UI.Err(ex); }
        }
    }
}

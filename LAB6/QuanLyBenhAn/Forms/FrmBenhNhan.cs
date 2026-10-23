using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using QuanLyBenhAn.Services;

namespace QuanLyBenhAn.Forms
{
    /// <summary>Đăng ký bệnh nhân, thu lệ phí năm, cấp / gia hạn hồ sơ bệnh án.</summary>
    public class FrmBenhNhan : Form
    {
        private readonly TextBox txtSearch = Ui.Txt(200);
        private readonly TextBox txtMa = Ui.Txt(), txtTen = Ui.Txt(), txtDiaChi = Ui.Txt(), txtDT = Ui.Txt();
        private readonly DateTimePicker dtpSinh = new DateTimePicker { Format = DateTimePickerFormat.Short, ShowCheckBox = true, Checked = false, Width = 220, Font = Ui.Normal };
        private readonly PictureBox pic = new PictureBox { Width = 90, Height = 120, SizeMode = PictureBoxSizeMode.Zoom, BorderStyle = BorderStyle.FixedSingle, Margin = new Padding(3) };
        private readonly NumericUpDown numLePhi = new NumericUpDown { Minimum = 0, Maximum = 100000000, Increment = 50000, Value = 500000, ThousandsSeparator = true, Width = 220, Font = Ui.Normal };
        private readonly NumericUpDown numNam = new NumericUpDown { Minimum = 2000, Maximum = 2100, Width = 220, Font = Ui.Normal };
        private readonly DataGridView grid = Ui.Grid();
        private byte[] anh;
        private bool loading;

        public FrmBenhNhan()
        {
            Text = "Bệnh nhân - Đăng ký khám và cấp hồ sơ";
            Size = new Size(1180, 680);
            numNam.Value = DateTime.Today.Year;
            txtMa.ReadOnly = true;
            BuildUi();
            grid.SelectionChanged += (s, e) => { if (!loading) FillFromGrid(); };
            Load += (s, e) => Ui.Guard(Reload);
        }

        private void BuildUi()
        {
            var top = Ui.Row(Ui.Lbl("Tìm (mã BN, tên, SĐT, mã hồ sơ):"), txtSearch, Ui.Btn("Tìm", (s, e) => Ui.Guard(Reload), 80));
            txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) Ui.Guard(Reload); };

            var photo = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.LeftToRight };
            photo.Controls.Add(pic);
            photo.Controls.Add(Ui.Btn("Chọn ảnh...", ChonAnh, 100));

            var right = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true, Padding = new Padding(6) };
            right.Controls.Add(Ui.Fields("Mã bệnh nhân", txtMa, "Họ tên *", txtTen, "Địa chỉ", txtDiaChi,
                "Ngày sinh", dtpSinh, "Điện thoại", txtDT, "Ảnh 3x4", photo));
            right.Controls.Add(Ui.Row(
                Ui.Btn("Thêm mới", (s, e) => Ui.Guard(Them), 90),
                Ui.Btn("Cập nhật", (s, e) => Ui.Guard(CapNhat), 90),
                Ui.Btn("Xóa", (s, e) => Ui.Guard(Xoa), 70),
                Ui.Btn("Làm mới", (s, e) => ClearFields(), 80)));
            right.Controls.Add(Ui.Lbl("Đăng ký khám bệnh - đóng lệ phí - cấp hồ sơ", true));
            right.Controls.Add(Ui.Fields("Lệ phí năm (đ)", numLePhi, "Năm hiệu lực", numNam));
            right.Controls.Add(Ui.Btn("Đăng ký / gia hạn hồ sơ", (s, e) => Ui.Guard(DangKy), 330));

            Controls.Add(Ui.Layout(top, grid, right, 420));
        }

        private void Reload()
        {
            string keep = txtMa.Text;
            loading = true;
            grid.DataSource = BenhNhanService.Search(txtSearch.Text);
            Ui.Headers(grid, "maBN", "Mã BN", "hoTen", "Họ tên", "ngaySinh", "Ngày sinh", "dienThoai", "Điện thoại", "diaChi", "Địa chỉ", "maHS", "Mã hồ sơ");
            Ui.Format(grid, "ngaySinh", "dd/MM/yyyy");
            loading = false;
            if (!string.IsNullOrEmpty(keep)) SelectRow(keep);
            FillFromGrid();
        }

        private bool SelectRow(string ma)
        {
            foreach (DataGridViewRow r in grid.Rows)
                if (Ui.Str(r.Cells["maBN"].Value) == ma)
                {
                    grid.CurrentCell = r.Cells[1];
                    return true;
                }
            return false;
        }

        private void FillFromGrid()
        {
            var r = grid.CurrentRow;
            if (r == null || r.IsNewRow) { ClearFields(); return; }
            txtMa.Text = Ui.Str(r.Cells["maBN"].Value);
            txtTen.Text = Ui.Str(r.Cells["hoTen"].Value);
            txtDiaChi.Text = Ui.Str(r.Cells["diaChi"].Value);
            txtDT.Text = Ui.Str(r.Cells["dienThoai"].Value);
            object ns = r.Cells["ngaySinh"].Value;
            if (ns == null || ns == DBNull.Value) dtpSinh.Checked = false;
            else { dtpSinh.Value = (DateTime)ns; dtpSinh.Checked = true; }
            anh = BenhNhanService.GetPhoto(txtMa.Text);
            ShowPic();
        }

        private void ClearFields()
        {
            txtMa.Clear(); txtTen.Clear(); txtDiaChi.Clear(); txtDT.Clear();
            dtpSinh.Checked = false;
            anh = null; ShowPic();
        }

        private void ShowPic()
        {
            if (pic.Image != null) { pic.Image.Dispose(); pic.Image = null; }
            if (anh != null && anh.Length > 0)
            {
                try { pic.Image = Image.FromStream(new MemoryStream(anh)); } catch { pic.Image = null; }
            }
        }

        private void ChonAnh(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog { Filter = "Ảnh (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp" })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;
                var bytes = File.ReadAllBytes(dlg.FileName);
                if (bytes.Length > 2 * 1024 * 1024) { Ui.Warn("Ảnh 3x4 tối đa 2 MB."); return; }
                anh = bytes;
                ShowPic();
            }
        }

        private DateTime? NgaySinh() { return dtpSinh.Checked ? (DateTime?)dtpSinh.Value.Date : null; }

        private void Them()
        {
            string ma = BenhNhanService.Insert(txtTen.Text, Ui.NullIfEmpty(txtDiaChi.Text), NgaySinh(), Ui.NullIfEmpty(txtDT.Text), anh);
            txtMa.Text = ma;
            Reload();
            Ui.Info("Đã thêm bệnh nhân " + ma + ". Hãy bấm 'Đăng ký / gia hạn hồ sơ' để đóng lệ phí và cấp hồ sơ.");
        }

        private void CapNhat()
        {
            BenhNhanService.Update(txtMa.Text, txtTen.Text, Ui.NullIfEmpty(txtDiaChi.Text), NgaySinh(), Ui.NullIfEmpty(txtDT.Text), anh);
            Reload();
            Ui.Info("Đã cập nhật thông tin bệnh nhân.");
        }

        private void Xoa()
        {
            if (string.IsNullOrEmpty(txtMa.Text)) throw new BusinessException("Hãy chọn bệnh nhân cần xóa.");
            if (!Ui.Confirm("Xóa bệnh nhân " + txtMa.Text + " - " + txtTen.Text + "?")) return;
            BenhNhanService.Delete(txtMa.Text);
            txtMa.Clear();
            Reload();
        }

        private void DangKy()
        {
            if (!Ui.Confirm("Thu lệ phí " + numLePhi.Value.ToString("N0") + "đ cho năm " + numNam.Value + " và cấp/gia hạn hồ sơ?")) return;
            string maHS = BenhNhanService.DangKyHoSo(txtMa.Text, numLePhi.Value, (int)numNam.Value);
            Reload();
            Ui.Info("Đã lập phiếu đăng ký, thu lệ phí và cấp/gia hạn hồ sơ bệnh án: " + maHS);
        }
    }
}

using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBenhAn.Services;

namespace QuanLyBenhAn.Forms
{
    /// <summary>Đặt lịch hẹn khám (kể cả khám sức khỏe định kỳ theo quý).</summary>
    public class FrmLichHen : Form
    {
        private readonly TextBox txtSearch = Ui.Txt(200), txtMa = Ui.Txt(), txtBN = Ui.Txt();
        private readonly ComboBox cboNV = Ui.ComboEmpty("hoTen", "maNV");
        private readonly DateTimePicker dtp = Ui.DateTimeBox();
        private readonly ComboBox cboLoai = Ui.ComboList(LichHenService.LoaiHen);
        private readonly ComboBox cboTT = Ui.ComboList(LichHenService.TrangThai);
        private readonly DataGridView grid = Ui.Grid();
        private bool loading;

        public FrmLichHen()
        {
            Text = "Lịch hẹn khám";
            Size = new Size(1150, 620);
            txtMa.ReadOnly = true;
            dtp.Value = DateTime.Today.AddDays(1).AddHours(9);
            BuildUi();
            grid.SelectionChanged += (s, e) => { if (!loading) FillFromGrid(); };
            Load += (s, e) => Ui.Guard(() => { cboNV.DataSource = DanhMucService.NhanVien(); Reload(); });
        }

        private void BuildUi()
        {
            var top = Ui.Row(Ui.Lbl("Tìm (mã lịch, mã BN, tên):"), txtSearch, Ui.Btn("Tìm", (s, e) => Ui.Guard(Reload), 80));
            txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) Ui.Guard(Reload); };
            var right = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true, Padding = new Padding(6) };
            right.Controls.Add(Ui.Fields("Mã lịch hẹn", txtMa, "Mã bệnh nhân *", txtBN, "Nhân viên tiếp nhận *", cboNV,
                "Ngày giờ hẹn *", dtp, "Loại hẹn *", cboLoai, "Trạng thái", cboTT));
            right.Controls.Add(Ui.Row(Ui.Btn("Đặt lịch", (s, e) => Ui.Guard(Dat), 90),
                Ui.Btn("Đổi trạng thái", (s, e) => Ui.Guard(DoiTT), 120), Ui.Btn("Xóa", (s, e) => Ui.Guard(Xoa), 70)));
            right.Controls.Add(Ui.Lbl("Giờ hẹn trong khoảng 07:00 - 17:00."));
            Controls.Add(Ui.Layout(top, grid, right, 400));
        }

        private void Reload()
        {
            loading = true;
            grid.DataSource = LichHenService.List(txtSearch.Text);
            Ui.Headers(grid, "maLH", "Mã lịch", "maBN", "Mã BN", "hoTen", "Bệnh nhân", "ngayGioHen", "Ngày giờ hẹn", "loaiHen", "Loại",
                "trangThai", "Trạng thái", "nhanVien", "Nhân viên");
            Ui.Format(grid, "ngayGioHen", "dd/MM/yyyy HH:mm");
            loading = false;
            FillFromGrid();
        }

        private void FillFromGrid()
        {
            var r = grid.CurrentRow;
            if (r == null) { txtMa.Clear(); return; }
            txtMa.Text = Ui.Str(r.Cells["maLH"].Value);
            txtBN.Text = Ui.Str(r.Cells["maBN"].Value);
            cboLoai.SelectedItem = Ui.Str(r.Cells["loaiHen"].Value);
            cboTT.SelectedItem = Ui.Str(r.Cells["trangThai"].Value);
        }

        private void Dat()
        {
            string ma = LichHenService.Add(txtBN.Text, cboNV.SelectedValue as string, dtp.Value, cboLoai.SelectedItem as string);
            Reload();
            Ui.Info("Đã đặt lịch hẹn " + ma + ".");
        }

        private void DoiTT()
        {
            LichHenService.UpdateStatus(txtMa.Text, cboTT.SelectedItem as string);
            string keep = txtMa.Text;
            Reload();
            Ui.Info("Đã cập nhật trạng thái lịch hẹn " + keep + ".");
        }

        private void Xoa()
        {
            if (string.IsNullOrEmpty(txtMa.Text)) throw new BusinessException("Hãy chọn lịch hẹn cần xóa.");
            if (!Ui.Confirm("Xóa lịch hẹn " + txtMa.Text + "?")) return;
            LichHenService.Delete(txtMa.Text);
            Reload();
        }
    }
}

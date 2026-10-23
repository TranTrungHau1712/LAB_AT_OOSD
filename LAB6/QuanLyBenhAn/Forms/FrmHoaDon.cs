using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBenhAn.Services;

namespace QuanLyBenhAn.Forms
{
    /// <summary>Thanh toán viện phí sau khi khám. Khám sức khỏe định kỳ được miễn phí.</summary>
    public class FrmHoaDon : Form
    {
        private readonly TextBox txtPK = Ui.Txt(), txtLoai = Ui.Txt(), txtXN = Ui.Txt(), txtTong = Ui.Txt();
        private readonly NumericUpDown numKhac = new NumericUpDown { Minimum = 0, Maximum = 1000000000, Increment = 10000, ThousandsSeparator = true, Width = 220, Font = Ui.Normal };
        private readonly ComboBox cboHT = Ui.ComboList(HoaDonService.HinhThuc);
        private readonly Button btnPay;
        private readonly DataGridView grid = Ui.Grid();
        private decimal tienXN;
        private bool loading;

        public FrmHoaDon()
        {
            Text = "Thanh toán viện phí";
            Size = new Size(1180, 620);
            txtPK.ReadOnly = txtLoai.ReadOnly = txtXN.ReadOnly = txtTong.ReadOnly = true;
            btnPay = Ui.Btn("Thanh toán", (s, e) => Ui.Guard(Thanh), 150);
            BuildUi();
            numKhac.ValueChanged += (s, e) => Tinh();
            grid.SelectionChanged += (s, e) => { if (!loading) FillFromGrid(); };
            Load += (s, e) => Ui.Guard(Reload);
        }

        private void BuildUi()
        {
            var top = Ui.Row(Ui.Lbl("Danh sách phiếu khám và tình trạng thanh toán", true), Ui.Btn("Tải lại", (s, e) => Ui.Guard(Reload), 80));
            var right = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true, Padding = new Padding(6) };
            right.Controls.Add(Ui.Fields("Mã phiếu khám", txtPK, "Loại khám", txtLoai, "Tiền xét nghiệm (đ)", txtXN,
                "Phí khác (đ)", numKhac, "Tổng viện phí (đ)", txtTong, "Hình thức", cboHT));
            right.Controls.Add(btnPay);
            Controls.Add(Ui.Layout(top, grid, right, 420));
        }

        private void Reload()
        {
            string keep = txtPK.Text;
            loading = true;
            grid.DataSource = HoaDonService.List();
            Ui.Headers(grid, "maPK", "Mã PK", "hoTen", "Bệnh nhân", "ngayGioKham", "Ngày khám", "loaiKham", "Loại", "tienXN", "Tiền XN",
                "maHD", "Mã HĐ", "tongVienPhi", "Tổng viện phí", "hinhThuc", "Hình thức", "trangThai", "Trạng thái");
            Ui.Format(grid, "ngayGioKham", "dd/MM/yyyy HH:mm"); Ui.Format(grid, "tienXN", "N0"); Ui.Format(grid, "tongVienPhi", "N0");
            loading = false;
            foreach (DataGridViewRow r in grid.Rows)
                if (Ui.Str(r.Cells["maPK"].Value) == keep) { grid.CurrentCell = r.Cells[1]; break; }
            FillFromGrid();
        }

        private void FillFromGrid()
        {
            var r = grid.CurrentRow;
            if (r == null) { txtPK.Clear(); txtLoai.Clear(); txtXN.Clear(); txtTong.Clear(); btnPay.Enabled = false; return; }
            txtPK.Text = Ui.Str(r.Cells["maPK"].Value);
            txtLoai.Text = Ui.Str(r.Cells["loaiKham"].Value);
            tienXN = Convert.ToDecimal(r.Cells["tienXN"].Value);
            txtXN.Text = tienXN.ToString("N0");
            bool daTra = Ui.Str(r.Cells["trangThai"].Value) == "Đã thanh toán";
            bool mienPhi = txtLoai.Text == PhieuKhamService.DinhKy;
            numKhac.Enabled = !daTra && !mienPhi;
            cboHT.Enabled = !daTra;
            btnPay.Enabled = !daTra;
            if (daTra)
            {
                txtTong.Text = Convert.ToDecimal(r.Cells["tongVienPhi"].Value).ToString("N0");
                cboHT.SelectedItem = Ui.Str(r.Cells["hinhThuc"].Value);
            }
            else Tinh();
        }

        private void Tinh()
        {
            if (txtLoai.Text == PhieuKhamService.DinhKy) { txtTong.Text = "0"; return; }
            txtTong.Text = (tienXN + numKhac.Value).ToString("N0");
        }

        private decimal Tong()
        {
            return txtLoai.Text == PhieuKhamService.DinhKy ? 0m : tienXN + numKhac.Value;
        }

        private void Thanh()
        {
            if (!Ui.Confirm("Thanh toán " + Tong().ToString("N0") + "đ cho phiếu khám " + txtPK.Text + "?")) return;
            string ma = HoaDonService.Pay(txtPK.Text, Tong(), cboHT.SelectedItem as string);
            Reload();
            Ui.Info("Đã lập hóa đơn " + ma + ".");
        }
    }
}

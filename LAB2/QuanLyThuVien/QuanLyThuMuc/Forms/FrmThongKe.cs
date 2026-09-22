using System;
using System.Windows.Forms;
using QuanLyThuVien.Data;

namespace QuanLyThuVien
{
    public partial class FrmThongKe : Form
    {
        public FrmThongKe()
        {
            InitializeComponent();
        }

        private void FrmThongKe_Load(object sender, EventArgs e)
        {
            dtTu.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtDen.Value = DateTime.Today;
            ThucHienThongKe();
        }

        private void btnThongKe_Click(object sender, EventArgs e)
        {
            ThucHienThongKe();
        }

        private void ThucHienThongKe()
        {
            ThongKeTongHop kq = ThongKeService.LayTongHop(dtTu.Value, dtDen.Value);

            lblMuon.Text = kq.LuotSachMuon.ToString();
            lblQuaHan.Text = kq.SachQuaHan.ToString();
            lblMat.Text = kq.SachMat.ToString();
            lblHuHong.Text = kq.SachHuHong.ToString();
            lblPhiPhat.Text = kq.TongPhiPhat.ToString("N0") + " đ";

            dgvPhat.DataSource = ThongKeService.LayChiTietPhat(dtTu.Value, dtDen.Value);
            dgvPhat.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
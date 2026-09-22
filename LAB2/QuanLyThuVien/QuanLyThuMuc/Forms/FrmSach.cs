using System;
using System.Data;
using System.Windows.Forms;
using QuanLyThuVien.Data;

namespace QuanLyThuVien
{
    public partial class FrmSach : Form
    {
        public FrmSach()
        {
            InitializeComponent();
        }

        private void FrmSach_Load(object sender, EventArgs e)
        {
            NapComboBox();
            NapDanhSach();
            LamMoi();
        }

        private void NapComboBox()
        {
            DataTable dtTL = DanhMucService.LayDanhSachTheLoai();
            cboTheLoai.DataSource = dtTL;
            cboTheLoai.DisplayMember = "TenTheLoai";
            cboTheLoai.ValueMember = "MaTheLoai";

            DataTable dtNXB = DanhMucService.LayDanhSachNhaXuatBan();
            cboNXB.DataSource = dtNXB;
            cboNXB.DisplayMember = "MaNhaXuatBan";
            cboNXB.ValueMember = "MaNhaXuatBan";
        }

        private void NapDanhSach()
        {
            dgvSach.DataSource = SachService.LayDanhSach();
        }

        private void ShowResult(KetQuaXuLy kq)
        {
            MessageBox.Show(kq.ThongBao, kq.ThanhCong ? "Thông báo" : "Lỗi",
                MessageBoxButtons.OK, kq.ThanhCong ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (kq.ThanhCong)
            {
                NapDanhSach();
                LamMoi();
            }
        }

        private DauSach LayTuForm()
        {
            return new DauSach
            {
                MaDauSach = txtMa.Text.Trim(),
                TenSach = txtTen.Text.Trim(),
                NamXuatBan = (int)numNam.Value,
                SoLuongHienCo = (int)numSoLuong.Value,
                MaTheLoai = Convert.ToString(cboTheLoai.SelectedValue),
                MaNhaXuatBan = Convert.ToString(cboNXB.SelectedValue)
            };
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            ShowResult(SachService.ThemSach(LayTuForm()));
        }

        private void btnCapNhat_Click(object sender, EventArgs e)
        {
            ShowResult(SachService.CapNhatSach(LayTuForm()));
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Xóa đầu sách đang chọn?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ShowResult(SachService.XoaSach(txtMa.Text.Trim()));
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LamMoi();
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            string tk = txtTim.Text.Trim();
            dgvSach.DataSource = string.IsNullOrEmpty(tk) ? SachService.LayDanhSach() : SachService.TimKiem(tk);
        }

        private void dgvSach_SelectionChanged(object sender, EventArgs e)
        {
            DataRowView r = dgvSach.CurrentRow == null ? null : dgvSach.CurrentRow.DataBoundItem as DataRowView;
            if (r == null) return;

            txtMa.Text = Convert.ToString(r["MaDauSach"]);
            txtTen.Text = Convert.ToString(r["TenSach"]);
            numNam.Value = Convert.ToInt32(r["NamXuatBan"]);
            numSoLuong.Value = Convert.ToInt32(r["SoLuongHienCo"]);
            cboTheLoai.SelectedValue = Convert.ToString(r["MaTheLoai"]);
            cboNXB.SelectedValue = Convert.ToString(r["MaNhaXuatBan"]);

            txtMa.ReadOnly = true;
            btnThem.Enabled = false;
            btnCapNhat.Enabled = true;
            btnXoa.Enabled = true;
        }

        private void LamMoi()
        {
            txtMa.Clear();
            txtTen.Clear();
            txtTim.Clear();
            numNam.Value = numNam.Minimum;
            numSoLuong.Value = 0;
            if (cboTheLoai.Items.Count > 0) cboTheLoai.SelectedIndex = 0;
            if (cboNXB.Items.Count > 0) cboNXB.SelectedIndex = 0;

            txtMa.ReadOnly = false;
            btnThem.Enabled = true;
            btnCapNhat.Enabled = false;
            btnXoa.Enabled = false;
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnThem_Click_1(object sender, EventArgs e)
        {

        }

        private void btnCapNhat_Click_1(object sender, EventArgs e)
        {

        }

        private void btnXoa_Click_1(object sender, EventArgs e)
        {

        }

        private void btnLamMoi_Click_1(object sender, EventArgs e)
        {

        }

        private void btnDong_Click_1(object sender, EventArgs e)
        {

        }
    }
}
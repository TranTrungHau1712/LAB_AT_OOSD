using System;
using System.Data;
using System.Windows.Forms;
using QuanLyThuVien.Data;

namespace QuanLyThuVien
{
    public partial class FrmDocGia : Form
    {
        public FrmDocGia()
        {
            InitializeComponent();
        }

        private void FrmDocGia_Load(object sender, EventArgs e)
        {
            NapDanhSach();
            LamMoi();
        }

        private void NapDanhSach()
        {
            dgvDocGia.DataSource = DocGiaService.LayDanhSach();
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

        private DocGia LayTuForm()
        {
            return new DocGia
            {
                MaDocGia = txtMa.Text.Trim(),
                Ho = txtHo.Text.Trim(),
                Ten = txtTen.Text.Trim(),
                NgaySinh = dtNgaySinh.Value,
                Phai = Convert.ToString(cboPhai.SelectedItem),
                SoDienThoai = txtSDT.Text.Trim(),
                DiaChi = txtDiaChi.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Anh3x4 = txtAnh.Text.Trim()
            };
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            ShowResult(DocGiaService.ThemDocGia(LayTuForm()));
        }

        private void btnCapNhat_Click(object sender, EventArgs e)
        {
            ShowResult(DocGiaService.CapNhatDocGia(LayTuForm()));
        }

        private void btnCapThe_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMa.Text.Trim()))
            {
                MessageBox.Show("Chọn hoặc nhập mã độc giả trước khi cấp thẻ.", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            ShowResult(DocGiaService.CapThe(txtMa.Text.Trim(), dtNgayCap.Value, dtHan.Value, chkLePhi.Checked));
        }

        private void btnGiaHan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMa.Text.Trim()))
            {
                MessageBox.Show("Chọn độc giả trước khi gia hạn.", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            ShowResult(DocGiaService.GiaHan(txtMa.Text.Trim(), dtHan.Value));
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LamMoi();
        }

        private void dgvDocGia_SelectionChanged(object sender, EventArgs e)
        {
            DataRowView r = dgvDocGia.CurrentRow == null ? null : dgvDocGia.CurrentRow.DataBoundItem as DataRowView;
            if (r == null) return;

            txtMa.Text = Convert.ToString(r["MaDocGia"]);
            txtHo.Text = Convert.ToString(r["Ho"]);
            txtTen.Text = Convert.ToString(r["Ten"]);
            dtNgaySinh.Value = Convert.ToDateTime(r["NgaySinh"]);
            cboPhai.SelectedItem = Convert.ToString(r["Phai"]);
            txtSDT.Text = Convert.ToString(r["SoDienThoai"]);
            txtDiaChi.Text = Convert.ToString(r["DiaChi"]);
            txtEmail.Text = Convert.ToString(r["Email"]);
            txtAnh.Text = Convert.ToString(r["Anh3x4"]);

            if (r["HanSuDung"] != DBNull.Value)
            {
                dtHan.Value = Convert.ToDateTime(r["HanSuDung"]);
            }
            if (r["NgayCap"] != DBNull.Value)
            {
                dtNgayCap.Value = Convert.ToDateTime(r["NgayCap"]);
            }
            if (r["DaDongLePhi"] != DBNull.Value)
            {
                chkLePhi.Checked = Convert.ToBoolean(r["DaDongLePhi"]);
            }

            txtMa.ReadOnly = true;
            btnThem.Enabled = false;
            btnCapNhat.Enabled = true;
        }

        private void LamMoi()
        {
            txtMa.Clear();
            txtHo.Clear();
            txtTen.Clear();
            txtSDT.Clear();
            txtDiaChi.Clear();
            txtEmail.Clear();
            txtAnh.Clear();
            dtNgaySinh.Value = DateTime.Today.AddYears(-18);
            if (cboPhai.Items.Count > 0) cboPhai.SelectedIndex = 0;
            dtNgayCap.Value = DateTime.Today;
            dtHan.Value = DateTime.Today.AddYears(1);
            chkLePhi.Checked = false;

            txtMa.ReadOnly = false;
            btnThem.Enabled = true;
            btnCapNhat.Enabled = false;
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

        private void btnCapThe_Click_1(object sender, EventArgs e)
        {

        }

        private void btnGiaHan_Click_1(object sender, EventArgs e)
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
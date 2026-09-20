using System;
using System.Data;
using System.Windows.Forms;
using QuanLyThuVien.Data;

namespace QuanLyThuVien
{
    public partial class FrmDanhMuc : Form
    {
        public FrmDanhMuc()
        {
            InitializeComponent();
        }

        private void FrmDanhMuc_Load(object sender, EventArgs e)
        {
            TaiTatCa();
            LamMoiNV();
            LamMoiTL();
            LamMoiNXB();
        }

        private void TaiTatCa()
        {
            dgvNV.DataSource = DanhMucService.LayDanhSachNhanVien();
            dgvTL.DataSource = DanhMucService.LayDanhSachTheLoai();
            dgvNXB.DataSource = DanhMucService.LayDanhSachNhaXuatBan();
        }

        private void ShowResult(KetQuaXuLy kq)
        {
            MessageBox.Show(kq.ThongBao, kq.ThanhCong ? "Thông báo" : "Lỗi",
                MessageBoxButtons.OK, kq.ThanhCong ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            if (kq.ThanhCong) TaiTatCa();
        }

        private bool XacNhanXoa()
        {
            return MessageBox.Show("Xóa dữ liệu đang chọn?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        // ===== NHÂN VIÊN =====
        private void btnNVThem_Click(object sender, EventArgs e)
        {
            ShowResult(DanhMucService.ThemNhanVien(new NhanVien
            {
                MaNhanVien = txtNVMa.Text.Trim(),
                Ho = txtNVHo.Text.Trim(),
                Ten = txtNVTen.Text.Trim(),
                Phai = Convert.ToString(cboNVPhai.SelectedItem),
                NgaySinh = dtNVNgaySinh.Value,
                ChucVu = txtNVChucVu.Text.Trim(),
                SoDienThoai = txtNVSDT.Text.Trim()
            }));
        }

        private void btnNVCapNhat_Click(object sender, EventArgs e)
        {
            ShowResult(DanhMucService.CapNhatNhanVien(new NhanVien
            {
                MaNhanVien = txtNVMa.Text.Trim(),
                Ho = txtNVHo.Text.Trim(),
                Ten = txtNVTen.Text.Trim(),
                Phai = Convert.ToString(cboNVPhai.SelectedItem),
                NgaySinh = dtNVNgaySinh.Value,
                ChucVu = txtNVChucVu.Text.Trim(),
                SoDienThoai = txtNVSDT.Text.Trim()
            }));
        }

        private void btnNVXoa_Click(object sender, EventArgs e)
        {
            if (XacNhanXoa())
                ShowResult(DanhMucService.XoaNhanVien(txtNVMa.Text.Trim()));
        }

        private void btnNVMoi_Click(object sender, EventArgs e) { LamMoiNV(); }

        private void dgvNV_SelectionChanged(object sender, EventArgs e)
        {
            DataRowView r = dgvNV.CurrentRow == null ? null : dgvNV.CurrentRow.DataBoundItem as DataRowView;
            if (r == null) return;
            txtNVMa.Text = Convert.ToString(r["MaNhanVien"]);
            txtNVHo.Text = Convert.ToString(r["Ho"]);
            txtNVTen.Text = Convert.ToString(r["Ten"]);
            cboNVPhai.SelectedItem = Convert.ToString(r["Phai"]);
            dtNVNgaySinh.Value = Convert.ToDateTime(r["NgaySinh"]);
            txtNVChucVu.Text = Convert.ToString(r["ChucVu"]);
            txtNVSDT.Text = Convert.ToString(r["SoDienThoai"]);
            txtNVMa.ReadOnly = true;
            btnNVThem.Enabled = false;
            btnNVCapNhat.Enabled = true;
            btnNVXoa.Enabled = true;
        }

        private void LamMoiNV()
        {
            txtNVMa.Clear(); txtNVHo.Clear(); txtNVTen.Clear();
            txtNVChucVu.Clear(); txtNVSDT.Clear();
            dtNVNgaySinh.Value = DateTime.Today.AddYears(-25);
            txtNVMa.ReadOnly = false;
            btnNVThem.Enabled = true;
            btnNVCapNhat.Enabled = false;
            btnNVXoa.Enabled = false;
        }

        // ===== THỂ LOẠI =====
        private void btnTLThem_Click(object sender, EventArgs e)
        {
            ShowResult(DanhMucService.ThemTheLoai(new TheLoai { MaTheLoai = txtTLMa.Text.Trim(), TenTheLoai = txtTLTen.Text.Trim() }));
        }

        private void btnTLCapNhat_Click(object sender, EventArgs e)
        {
            ShowResult(DanhMucService.CapNhatTheLoai(new TheLoai { MaTheLoai = txtTLMa.Text.Trim(), TenTheLoai = txtTLTen.Text.Trim() }));
        }

        private void btnTLXoa_Click(object sender, EventArgs e)
        {
            if (XacNhanXoa())
                ShowResult(DanhMucService.XoaTheLoai(txtTLMa.Text.Trim()));
        }

        private void btnTLMoi_Click(object sender, EventArgs e) { LamMoiTL(); }

        private void dgvTL_SelectionChanged(object sender, EventArgs e)
        {
            DataRowView r = dgvTL.CurrentRow == null ? null : dgvTL.CurrentRow.DataBoundItem as DataRowView;
            if (r == null) return;
            txtTLMa.Text = Convert.ToString(r["MaTheLoai"]);
            txtTLTen.Text = Convert.ToString(r["TenTheLoai"]);
            txtTLMa.ReadOnly = true;
            btnTLThem.Enabled = false;
            btnTLCapNhat.Enabled = true;
            btnTLXoa.Enabled = true;
        }

        private void LamMoiTL()
        {
            txtTLMa.Clear(); txtTLTen.Clear();
            txtTLMa.ReadOnly = false;
            btnTLThem.Enabled = true;
            btnTLCapNhat.Enabled = false;
            btnTLXoa.Enabled = false;
        }

        // ===== NHÀ XUẤT BẢN =====
        private void btnNXBThem_Click(object sender, EventArgs e)
        {
            ShowResult(DanhMucService.ThemNhaXuatBan(new NhaXuatBan { MaNhaXuatBan = txtNXBMa.Text.Trim(), DiaChi = txtNXBDiaChi.Text.Trim(), SoDienThoai = txtNXBSDT.Text.Trim() }));
        }

        private void btnNXBCapNhat_Click(object sender, EventArgs e)
        {
            ShowResult(DanhMucService.CapNhatNhaXuatBan(new NhaXuatBan { MaNhaXuatBan = txtNXBMa.Text.Trim(), DiaChi = txtNXBDiaChi.Text.Trim(), SoDienThoai = txtNXBSDT.Text.Trim() }));
        }

        private void btnNXBXoa_Click(object sender, EventArgs e)
        {
            if (XacNhanXoa())
                ShowResult(DanhMucService.XoaNhaXuatBan(txtNXBMa.Text.Trim()));
        }

        private void btnNXBMoi_Click(object sender, EventArgs e) { LamMoiNXB(); }

        private void dgvNXB_SelectionChanged(object sender, EventArgs e)
        {
            DataRowView r = dgvNXB.CurrentRow == null ? null : dgvNXB.CurrentRow.DataBoundItem as DataRowView;
            if (r == null) return;
            txtNXBMa.Text = Convert.ToString(r["MaNhaXuatBan"]);
            txtNXBDiaChi.Text = Convert.ToString(r["DiaChi"]);
            txtNXBSDT.Text = Convert.ToString(r["SoDienThoai"]);
            txtNXBMa.ReadOnly = true;
            btnNXBThem.Enabled = false;
            btnNXBCapNhat.Enabled = true;
            btnNXBXoa.Enabled = true;
        }

        private void LamMoiNXB()
        {
            txtNXBMa.Clear(); txtNXBDiaChi.Clear(); txtNXBSDT.Clear();
            txtNXBMa.ReadOnly = false;
            btnNXBThem.Enabled = true;
            btnNXBCapNhat.Enabled = false;
            btnNXBXoa.Enabled = false;
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnNVThem_Click_1(object sender, EventArgs e)
        {

        }

        private void btnNVCapNhat_Click_1(object sender, EventArgs e)
        {

        }

        private void btnNVXoa_Click_1(object sender, EventArgs e)
        {

        }

        private void btnNVMoi_Click_1(object sender, EventArgs e)
        {

        }

        private void btnTLThem_Click_1(object sender, EventArgs e)
        {

        }

        private void btnTLCapNhat_Click_1(object sender, EventArgs e)
        {

        }

        private void btnTLXoa_Click_1(object sender, EventArgs e)
        {

        }

        private void btnTLMoi_Click_1(object sender, EventArgs e)
        {

        }

        private void btnNXBThem_Click_1(object sender, EventArgs e)
        {

        }

        private void btnNXBCapNhat_Click_1(object sender, EventArgs e)
        {

        }

        private void btnNXBXoa_Click_1(object sender, EventArgs e)
        {

        }

        private void btnNXBMoi_Click_1(object sender, EventArgs e)
        {

        }

        private void btnDong_Click_1(object sender, EventArgs e)
        {

        }
    }
}
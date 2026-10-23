using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBenhAn.Services;

namespace QuanLyBenhAn.Forms
{
    /// <summary>Lập phiếu khám: ghi nhận triệu chứng, chỉ định xét nghiệm (điều trị hoặc khám định kỳ).</summary>
    public class FrmPhieuKham : Form
    {
        private readonly TextBox txtHS = Ui.Txt(120);
        private readonly Label lblHS = Ui.Lbl("(chưa chọn hồ sơ)", true);
        private readonly ComboBox cboLoai = Ui.ComboList(new[] { PhieuKhamService.Dieutri, PhieuKhamService.DinhKy });
        private readonly DateTimePicker dtp = Ui.DateTimeBox();
        private readonly ComboBox cboKhoa = Ui.ComboEmpty("tenKhoa", "maKhoa");
        private readonly ComboBox cboBS = Ui.ComboEmpty("hoTen", "maBS");
        private readonly ComboBox cboNV = Ui.ComboEmpty("hoTen", "maNV");
        private readonly ComboBox cboLH = Ui.ComboEmpty("moTa", "maLH", 300);
        private readonly TextBox txtCD = Ui.Txt(300, true, 60), txtKL = Ui.Txt(300, true, 60);
        private readonly DataGridView gridTC = Ui.Grid(true), gridXN = Ui.Grid(true);
        private readonly ComboBox cboXN = Ui.ComboEmpty("ten", "maXN", 300);
        private bool binding;

        public FrmPhieuKham()
        {
            Text = "Lập phiếu khám";
            Size = new Size(1250, 760);
            BuildUi();
            Load += (s, e) => Ui.Guard(InitData);
        }

        private void BuildUi()
        {
            var info = Ui.Row(Ui.Lbl("Mã hồ sơ *"), txtHS, Ui.Btn("Tìm hồ sơ", (s, e) => Ui.Guard(TimHoSo), 100));
            var left = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true, Padding = new Padding(6) };
            left.Controls.Add(info);
            left.Controls.Add(lblHS);
            left.Controls.Add(Ui.Fields("Loại khám *", cboLoai, "Ngày giờ khám *", dtp, "Khoa *", cboKhoa, "Bác sĩ *", cboBS,
                "Nhân viên ghi nhận *", cboNV, "Theo lịch hẹn", cboLH, "Chẩn đoán", txtCD, "Kết luận", txtKL));
            left.Controls.Add(Ui.Row(Ui.Btn("Lưu phiếu khám", (s, e) => Ui.Guard(Luu), 150), Ui.Btn("Làm mới", (s, e) => Reset(), 90)));
            left.Controls.Add(Ui.Lbl("Khám định kỳ: miễn phí, 1 lần mỗi quý, không cần chẩn đoán bệnh."));

            gridTC.Columns.Add(new DataGridViewTextBoxColumn { Name = "ten", HeaderText = "Triệu chứng (Nhức đầu, Thân nhiệt, Huyết áp...)" });
            gridTC.Columns.Add(new DataGridViewTextBoxColumn { Name = "giaTri", HeaderText = "Giá trị", FillWeight = 40 });
            gridTC.Columns.Add(new DataGridViewTextBoxColumn { Name = "donVi", HeaderText = "Đơn vị", FillWeight = 25 });

            gridXN.AllowUserToAddRows = false;
            gridXN.AllowUserToDeleteRows = false;
            gridXN.Columns.Add(new DataGridViewTextBoxColumn { Name = "maXN", HeaderText = "Mã XN", ReadOnly = true, FillWeight = 20 });
            gridXN.Columns.Add(new DataGridViewTextBoxColumn { Name = "tenXN", HeaderText = "Xét nghiệm", ReadOnly = true, FillWeight = 50 });
            gridXN.Columns.Add(new DataGridViewTextBoxColumn { Name = "ketQua", HeaderText = "Kết quả (nếu đã có)", FillWeight = 50 });
            gridXN.Columns.Add(new DataGridViewTextBoxColumn { Name = "ghiChu", HeaderText = "Ghi chú", FillWeight = 30 });

            var xnBar = Ui.Row(Ui.Lbl("Chỉ định xét nghiệm:", true), cboXN,
                Ui.Btn("Thêm", (s, e) => Ui.Guard(ThemXN), 70), Ui.Btn("Bỏ dòng chọn", (s, e) => BoXN(), 110));

            var right = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4 };
            right.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            right.RowStyles.Add(new RowStyle(SizeType.Percent, 45));
            right.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            right.RowStyles.Add(new RowStyle(SizeType.Percent, 55));
            right.Controls.Add(Ui.Lbl("TRIỆU CHỨNG", true), 0, 0);
            right.Controls.Add(gridTC, 0, 1);
            right.Controls.Add(xnBar, 0, 2);
            right.Controls.Add(gridXN, 0, 3);

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 480));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            left.Dock = DockStyle.Fill;
            root.Controls.Add(left, 0, 0);
            root.Controls.Add(right, 1, 0);
            Controls.Add(root);

            cboKhoa.SelectedIndexChanged += (s, e) => { if (!binding) LoadBacSi(); };
        }

        private void InitData()
        {
            binding = true;
            cboKhoa.DataSource = DanhMucService.Khoa();
            cboNV.DataSource = DanhMucService.NhanVien();
            cboXN.DataSource = DanhMucService.XetNghiem();
            binding = false;
            LoadBacSi();
        }

        private void LoadBacSi()
        {
            string k = cboKhoa.SelectedValue as string;
            cboBS.DataSource = k == null ? null : DanhMucService.BacSi(k);
        }

        private void TimHoSo()
        {
            var r = HoSoService.Get(txtHS.Text);
            if (r == null) { lblHS.Text = "(không tìm thấy hồ sơ)"; lblHS.ForeColor = Color.Firebrick; cboLH.DataSource = null; throw new BusinessException("Không tìm thấy hồ sơ " + txtHS.Text.Trim() + "."); }
            string tt = Ui.Str(r["trangThai"]);
            lblHS.Text = Ui.Str(r["hoTen"]) + " | sinh " + Ui.Date(r["ngaySinh"]) + " | hạn " + Ui.Date(r["ngayHetHan"]) + " | " + tt;
            lblHS.ForeColor = tt == "Hết hạn" ? Color.Firebrick : Color.DarkGreen;

            var dt = LichHenService.PendingByHoSo(Ui.Str(r["maHS"]));
            var blank = dt.NewRow();
            blank["maLH"] = DBNull.Value;
            blank["moTa"] = "(không theo lịch hẹn)";
            dt.Rows.InsertAt(blank, 0);
            cboLH.DataSource = dt;
            cboLH.SelectedIndex = 0;
        }

        private void ThemXN()
        {
            var drv = cboXN.SelectedItem as DataRowView;
            if (drv == null) throw new BusinessException("Hãy chọn xét nghiệm.");
            string ma = drv["maXN"].ToString();
            foreach (DataGridViewRow r in gridXN.Rows)
                if (Ui.Str(r.Cells["maXN"].Value) == ma) throw new BusinessException("Xét nghiệm này đã được chỉ định.");
            gridXN.Rows.Add(ma, drv["tenXN"].ToString(), "", "");
        }

        private void BoXN()
        {
            if (gridXN.CurrentRow != null && !gridXN.CurrentRow.IsNewRow) gridXN.Rows.Remove(gridXN.CurrentRow);
        }

        private string Selected(ComboBox c) { return c.SelectedValue as string; }

        private void Luu()
        {
            gridTC.EndEdit(); gridXN.EndEdit();
            var x = new PhieuKhamInput
            {
                MaHS = txtHS.Text,
                Loai = cboLoai.SelectedItem as string,
                NgayGio = dtp.Value,
                MaKhoa = Selected(cboKhoa),
                MaBS = Selected(cboBS),
                MaNV = Selected(cboNV),
                MaLH = cboLH.SelectedIndex > 0 ? ((DataRowView)cboLH.SelectedItem)["maLH"] as string : null,
                ChanDoan = txtCD.Text,
                KetLuan = Ui.NullIfEmpty(txtKL.Text)
            };
            foreach (DataGridViewRow r in gridTC.Rows)
            {
                if (r.IsNewRow) continue;
                string ten = Ui.Str(r.Cells["ten"].Value), gt = Ui.Str(r.Cells["giaTri"].Value), dv = Ui.Str(r.Cells["donVi"].Value);
                if (ten == "" && gt == "" && dv == "") continue;
                x.TrieuChung.Add(new TrieuChungInput { Ten = ten, GiaTri = Ui.NullIfEmpty(gt), DonVi = Ui.NullIfEmpty(dv) });
            }
            foreach (DataGridViewRow r in gridXN.Rows)
                x.XetNghiem.Add(new XetNghiemInput
                {
                    MaXN = Ui.Str(r.Cells["maXN"].Value),
                    KetQua = Ui.Str(r.Cells["ketQua"].Value),
                    GhiChu = Ui.NullIfEmpty(Ui.Str(r.Cells["ghiChu"].Value))
                });

            string maPK = PhieuKhamService.Create(x);
            Ui.Info("Đã lưu phiếu khám " + maPK + ". Dùng menu 'Lập toa thuốc' với mã phiếu này để kê toa.");
            Reset();
        }

        private void Reset()
        {
            txtCD.Clear(); txtKL.Clear();
            gridTC.Rows.Clear(); gridXN.Rows.Clear();
            dtp.Value = DateTime.Now;
            if (cboLH.SelectedIndex > 0) cboLH.SelectedIndex = 0;
        }
    }
}

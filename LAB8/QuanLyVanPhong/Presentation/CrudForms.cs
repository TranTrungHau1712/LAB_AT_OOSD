using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyVanPhong.BLL;
using QuanLyVanPhong.Common;

namespace QuanLyVanPhong.Presentation
{
    // ------------------------------------------------------------ 3. Tầng
    public class TangForm : BaseListForm
    {
        public TangForm() { Init(); }
        protected override string Title { get { return "Quản lý tầng"; } }
        protected override string IdColumn { get { return "MaTang"; } }
        protected override void DefineFields()
        {
            Fields.Add(new FieldDef("MaTang", "Mã tầng", FieldType.Integer).GridCol().Hide());
            Fields.Add(new FieldDef("SoTang", "Số tầng", FieldType.Integer).Req().Range(1, 200));
            Fields.Add(new FieldDef("TenTang", "Tên tầng").Req().Len(50));
            Fields.Add(new FieldDef("MoTa", "Mô tả").Len(200).Full());
        }
        protected override DataTable LoadData(string k) { return TangBll.Search(k); }
        protected override void DoInsert(Dictionary<string, object> v) { TangBll.Insert(v); }
        protected override void DoUpdate(object id, Dictionary<string, object> v) { TangBll.Update(Convert.ToInt32(id), v); }
        protected override void DoDelete(object id) { TangBll.Delete(Convert.ToInt32(id)); }
    }

    // ------------------------------------------------------- 4. Văn phòng
    public class VanPhongForm : BaseListForm
    {
        public VanPhongForm() { Init(); }
        protected override string Title { get { return "Quản lý văn phòng"; } }
        protected override string IdColumn { get { return "MaVP"; } }
        protected override void DefineFields()
        {
            Fields.Add(new FieldDef("MaVP", "ID", FieldType.Integer).GridCol().Hide());
            Fields.Add(new FieldDef("MaPhong", "Mã phòng").Req().Len(10));
            Fields.Add(new FieldDef("MaTang", "Tầng", FieldType.Combo).Lookup(TangBll.Lookup(), "MaTang", "TenTang").Req().Hide());
            Fields.Add(new FieldDef("TenTang", "Tầng").GridCol());
            Fields.Add(new FieldDef("DienTich", "Diện tích (m²)", FieldType.Decimal).Dec(2).Req().Range(0, 100000));
            Fields.Add(new FieldDef("Huong", "Hướng", FieldType.Combo).Choices("Đông", "Tây", "Nam", "Bắc", "Đông Nam", "Đông Bắc", "Tây Nam", "Tây Bắc").Req());
            Fields.Add(new FieldDef("GiaThue", "Giá thuê / tháng (đ)", FieldType.Decimal).Req());
            Fields.Add(new FieldDef("TrangThai", "Trạng thái", FieldType.Combo).Choices("Trống", "Đang thuê", "Bảo trì").Req());
        }
        protected override void OnNew() { SetFieldValue("TrangThai", "Trống"); }
        protected override DataTable LoadData(string k) { return VanPhongBll.Search(k); }
        protected override void DoInsert(Dictionary<string, object> v) { VanPhongBll.Insert(v); }
        protected override void DoUpdate(object id, Dictionary<string, object> v) { VanPhongBll.Update(Convert.ToInt32(id), v); }
        protected override void DoDelete(object id) { VanPhongBll.Delete(Convert.ToInt32(id)); }
    }

    // ------------------------------------- 5. Khách hàng & Công ty (2 tab)
    public class KhachHangView : BaseListForm
    {
        public KhachHangView() { Init(); }
        protected override string Title { get { return "Khách hàng"; } }
        protected override string IdColumn { get { return "MaKH"; } }
        protected override void DefineFields()
        {
            Fields.Add(new FieldDef("MaKH", "ID", FieldType.Integer).GridCol().Hide());
            Fields.Add(new FieldDef("HoTen", "Họ tên").Req().Len(100));
            Fields.Add(new FieldDef("CCCD", "CCCD/CMND").Req().Len(12));
            Fields.Add(new FieldDef("SDT", "Số điện thoại").Req().Len(15));
            Fields.Add(new FieldDef("Email", "Email").Len(100));
            Fields.Add(new FieldDef("MaCT", "Thuộc công ty", FieldType.Combo).Lookup(CongTyBll.Lookup(), "MaCT", "TenCT", true).Hide());
            Fields.Add(new FieldDef("TenCT", "Công ty").GridCol());
            Fields.Add(new FieldDef("DiaChi", "Địa chỉ").Len(200).Full());
        }
        protected override DataTable LoadData(string k) { return KhachHangBll.Search(k); }
        protected override void DoInsert(Dictionary<string, object> v) { KhachHangBll.Insert(v); }
        protected override void DoUpdate(object id, Dictionary<string, object> v) { KhachHangBll.Update(Convert.ToInt32(id), v); }
        protected override void DoDelete(object id) { KhachHangBll.Delete(Convert.ToInt32(id)); }
    }

    public class CongTyView : BaseListForm
    {
        public CongTyView() { Init(); }
        protected override string Title { get { return "Công ty"; } }
        protected override string IdColumn { get { return "MaCT"; } }
        protected override void DefineFields()
        {
            Fields.Add(new FieldDef("MaCT", "ID", FieldType.Integer).GridCol().Hide());
            Fields.Add(new FieldDef("TenCT", "Tên công ty").Req().Len(150));
            Fields.Add(new FieldDef("MaSoThue", "Mã số thuế").Req().Len(20));
            Fields.Add(new FieldDef("SDT", "Số điện thoại").Len(15));
            Fields.Add(new FieldDef("Email", "Email").Len(100));
            Fields.Add(new FieldDef("NguoiDaiDien", "Người đại diện").Len(100));
            Fields.Add(new FieldDef("DiaChi", "Địa chỉ").Len(200).Full());
        }
        protected override DataTable LoadData(string k) { return CongTyBll.Search(k); }
        protected override void DoInsert(Dictionary<string, object> v) { CongTyBll.Insert(v); }
        protected override void DoUpdate(object id, Dictionary<string, object> v) { CongTyBll.Update(Convert.ToInt32(id), v); }
        protected override void DoDelete(object id) { CongTyBll.Delete(Convert.ToInt32(id)); }
    }

    public class KhachHangCongTyForm : EmbeddedForm
    {
        public KhachHangCongTyForm()
        {
            var tabs = new TabControl();
            tabs.Dock = DockStyle.Fill;
            tabs.Font = Theme.Bold;
            tabs.Padding = new Point(18, 6);
            tabs.TabPages.Add(MakePage("Công ty", new CongTyView()));
            tabs.TabPages.Add(MakePage("Khách hàng", new KhachHangView()));
            tabs.SelectedIndexChanged += delegate
            {
                // nạp lại danh sách công ty khi chuyển sang tab khách hàng
                if (tabs.SelectedIndex == 1)
                {
                    tabs.TabPages[1].Controls.Clear();
                    var kv = new KhachHangView();
                    tabs.TabPages[1].Controls.Add(kv);
                    kv.Show();
                }
            };
            Controls.Add(tabs);
        }

        private static Form Embed(Form f) { return f; }

        private static TabPage MakePage(string text, Form f)
        {
            var p = new TabPage(text);
            p.BackColor = Theme.Back;
            p.Controls.Add(f);
            f.Show();
            return p;
        }
    }

    // ------------------------------------------------ 9. Danh mục dịch vụ
    public class DichVuView : BaseListForm
    {
        public DichVuView() { Init(); }
        protected override string Title { get { return "Danh mục dịch vụ"; } }
        protected override string IdColumn { get { return "MaDV"; } }
        protected override void DefineFields()
        {
            Fields.Add(new FieldDef("MaDV", "ID", FieldType.Integer).GridCol().Hide());
            Fields.Add(new FieldDef("TenDV", "Tên dịch vụ").Req().Len(100));
            Fields.Add(new FieldDef("DonVi", "Đơn vị tính").Req().Len(20));
            Fields.Add(new FieldDef("DonGia", "Đơn giá (đ)", FieldType.Decimal).Req());
            Fields.Add(new FieldDef("LoaiTinh", "Cách tính", FieldType.Combo).Choices("Theo chỉ số", "Cố định").Req());
        }
        protected override DataTable LoadData(string k) { return DichVuBll.Search(k); }
        protected override void DoInsert(Dictionary<string, object> v) { DichVuBll.Insert(v); }
        protected override void DoUpdate(object id, Dictionary<string, object> v) { DichVuBll.Update(Convert.ToInt32(id), v); }
        protected override void DoDelete(object id) { DichVuBll.Delete(Convert.ToInt32(id)); }
    }

    /// <summary>Form dịch vụ: tab danh mục + tab nhập chỉ số điện/nước và tính tiền nhanh.</summary>
    public class DichVuForm : EmbeddedForm
    {
        private ComboBox cboDv;
        private NumericUpDown nudCu, nudMoi;
        private Label lblDonGia, lblSoLuong, lblThanhTien;

        public DichVuForm()
        {
            var tabs = new TabControl();
            tabs.Dock = DockStyle.Fill;
            tabs.Font = Theme.Bold;
            tabs.Padding = new Point(18, 6);

            var p1 = new TabPage("Danh mục dịch vụ");
            p1.BackColor = Theme.Back;
            var v = new DichVuView();
            p1.Controls.Add(v);
            v.Show();

            var p2 = new TabPage("Nhập chỉ số & tính tiền");
            p2.BackColor = Theme.Back;
            p2.Controls.Add(BuildCalc());

            tabs.TabPages.Add(p1);
            tabs.TabPages.Add(p2);
            tabs.SelectedIndexChanged += delegate
            {
                if (tabs.SelectedIndex == 1) LoadServices();
            };
            Controls.Add(tabs);
            LoadServices();
        }

        private Control BuildCalc()
        {
            var pnl = new Panel();
            pnl.Dock = DockStyle.Fill;
            pnl.Padding = new Padding(24);

            var title = new Label();
            title.Text = "TÍNH TIỀN DỊCH VỤ THEO CHỈ SỐ";
            title.Font = Theme.Title;
            title.ForeColor = Theme.Primary;
            title.AutoSize = true;
            title.Location = new Point(24, 16);
            pnl.Controls.Add(title);

            var tbl = new TableLayoutPanel();
            tbl.Location = new Point(24, 64);
            tbl.AutoSize = true;
            tbl.ColumnCount = 2;
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260));

            cboDv = new ComboBox();
            cboDv.DropDownStyle = ComboBoxStyle.DropDownList;
            cboDv.Width = 250;
            nudCu = new NumericUpDown();
            nudCu.Maximum = 99999999; nudCu.DecimalPlaces = 2; nudCu.Width = 250;
            nudMoi = new NumericUpDown();
            nudMoi.Maximum = 99999999; nudMoi.DecimalPlaces = 2; nudMoi.Width = 250;
            lblDonGia = new Label(); lblDonGia.AutoSize = true;
            lblSoLuong = new Label(); lblSoLuong.AutoSize = true;
            lblThanhTien = new Label(); lblThanhTien.AutoSize = true;
            lblThanhTien.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblThanhTien.ForeColor = Theme.Danger;

            AddRow(tbl, "Dịch vụ", cboDv);
            AddRow(tbl, "Đơn giá", lblDonGia);
            AddRow(tbl, "Chỉ số cũ", nudCu);
            AddRow(tbl, "Chỉ số mới", nudMoi);
            AddRow(tbl, "Số lượng tiêu thụ", lblSoLuong);
            AddRow(tbl, "Thành tiền", lblThanhTien);
            pnl.Controls.Add(tbl);

            var btn = Theme.Btn("Tính tiền", Theme.Accent);
            btn.Location = new Point(24, 330);
            btn.Click += delegate { Calc(); };
            var btnReset = Theme.Btn("Làm mới", Theme.Primary);
            btnReset.Location = new Point(134, 330);
            btnReset.Click += delegate { nudCu.Value = 0; nudMoi.Value = 0; Calc(); };
            pnl.Controls.Add(btn);
            pnl.Controls.Add(btnReset);

            cboDv.SelectedIndexChanged += delegate { Calc(); };
            return pnl;
        }

        private static void AddRow(TableLayoutPanel t, string label, Control c)
        {
            var l = new Label();
            l.Text = label;
            l.AutoSize = true;
            l.Margin = new Padding(0, 10, 16, 10);
            c.Margin = new Padding(0, 6, 0, 6);
            int r = t.RowCount++;
            t.Controls.Add(l, 0, r);
            t.Controls.Add(c, 1, r);
        }

        private void LoadServices()
        {
            try
            {
                DataTable dt = DichVuBll.All();
                var meter = dt.Clone();
                foreach (DataRow r in dt.Select("LoaiTinh = 'Theo chỉ số'")) meter.ImportRow(r);
                cboDv.DataSource = null;
                cboDv.DisplayMember = "TenDV";
                cboDv.ValueMember = "MaDV";
                cboDv.DataSource = meter;
                Calc();
            }
            catch (Exception ex) { Theme.Error(ex.Message); }
        }

        private void Calc()
        {
            var row = cboDv.SelectedItem as DataRowView;
            if (row == null) return;
            decimal gia = Convert.ToDecimal(row["DonGia"]);
            lblDonGia.Text = gia.ToString("N0") + " đ / " + row["DonVi"];
            try
            {
                decimal tien = DichVuBll.Calc(nudCu.Value, nudMoi.Value, gia);
                lblSoLuong.Text = (nudMoi.Value - nudCu.Value).ToString("N2") + " " + row["DonVi"];
                lblThanhTien.Text = tien.ToString("N0") + " đ";
            }
            catch (BusinessException ex)
            {
                lblSoLuong.Text = "—";
                lblThanhTien.Text = ex.Message;
            }
        }
    }

    // ------------------------------------------------------ 11. Nhân viên
    public class NhanVienForm : BaseListForm
    {
        public NhanVienForm() { Init(); }
        protected override string Title { get { return "Quản lý nhân viên"; } }
        protected override string IdColumn { get { return "MaNV"; } }
        protected override void DefineFields()
        {
            Fields.Add(new FieldDef("MaNV", "Mã nhân viên").Req().Lock().Len(10));
            Fields.Add(new FieldDef("HoTen", "Họ tên").Req().Len(100));
            Fields.Add(new FieldDef("NgaySinh", "Ngày sinh", FieldType.Date));
            Fields.Add(new FieldDef("GioiTinh", "Giới tính", FieldType.Combo).Choices("Nam", "Nữ").Req());
            Fields.Add(new FieldDef("CCCD", "CCCD/CMND").Req().Len(12));
            Fields.Add(new FieldDef("SDT", "Điện thoại").Len(15));
            Fields.Add(new FieldDef("MaCT", "Công ty", FieldType.Combo).Lookup(CongTyBll.Lookup(), "MaCT", "TenCT").Req().Hide());
            Fields.Add(new FieldDef("TenCT", "Công ty").GridCol());
            Fields.Add(new FieldDef("MaVP", "Phòng làm việc", FieldType.Combo).Lookup(VanPhongBll.Lookup(), "MaVP", "Ten").Req().Hide());
            Fields.Add(new FieldDef("MaPhong", "Phòng").GridCol());
            Fields.Add(new FieldDef("TenTang", "Tầng").GridCol());
            Fields.Add(new FieldDef("ChucVu", "Chức vụ").Req().Len(50));
            Fields.Add(new FieldDef("TrangThai", "Trạng thái", FieldType.Combo).Choices("Đang làm", "Đã nghỉ").Req());
            Fields.Add(new FieldDef("HinhAnh", "Hình ảnh", FieldType.Image));
        }
        protected override void OnNew() { SetFieldValue("TrangThai", "Đang làm"); SetFieldValue("NgaySinh", DateTime.Today.AddYears(-25)); }
        protected override DataTable LoadData(string k) { return NhanVienBll.Search(k); }
        protected override void DoInsert(Dictionary<string, object> v) { NhanVienBll.Insert(v); }
        protected override void DoUpdate(object id, Dictionary<string, object> v) { NhanVienBll.Update(id.ToString(), v); }
        protected override void DoDelete(object id) { NhanVienBll.Delete(id.ToString()); }
    }

    // ----------------------------------------------------- 12. Thẻ giữ xe
    public class TheGiuXeForm : BaseListForm
    {
        public TheGiuXeForm() { Init(); }
        protected override string Title { get { return "Quản lý thẻ giữ xe"; } }
        protected override string IdColumn { get { return "MaThe"; } }
        protected override void DefineFields()
        {
            Fields.Add(new FieldDef("MaThe", "Mã thẻ").Req().Lock().Len(10));
            Fields.Add(new FieldDef("MaNV", "Nhân viên", FieldType.Combo).Lookup(NhanVienBll.Lookup(), "MaNV", "Ten").Lock().Hide());
            Fields.Add(new FieldDef("HoTen", "Họ tên").GridCol());
            Fields.Add(new FieldDef("TenCT", "Công ty").GridCol());
            Fields.Add(new FieldDef("NgayCap", "Ngày cấp", FieldType.Date));
            Fields.Add(new FieldDef("TrangThai", "Trạng thái thẻ", FieldType.Combo).Choices("Hoạt động", "Đã khóa").Req());
            Fields.Add(new FieldDef("LoaiXe", "Loại xe", FieldType.Combo).Choices("Xe máy", "Ô tô").Req());
            Fields.Add(new FieldDef("BienSo", "Biển số").Req().Len(15));
            Fields.Add(new FieldDef("HangXe", "Hãng xe").Len(50));
            Fields.Add(new FieldDef("MauSac", "Màu sắc").Len(30));
        }
        protected override void OnNew() { SetFieldValue("TrangThai", "Hoạt động"); SetFieldValue("LoaiXe", "Xe máy"); }
        protected override DataTable LoadData(string k) { return TheXeBll.Search(k); }
        protected override void DoInsert(Dictionary<string, object> v) { TheXeBll.Insert(v); }
        protected override void DoUpdate(object id, Dictionary<string, object> v) { TheXeBll.Update(id.ToString(), v); }
        protected override void DoDelete(object id) { TheXeBll.Delete(id.ToString()); }
    }

    // -------------------------------------------------- 13. Kiểm tra phòng
    public class KiemTraForm : BaseListForm
    {
        private bool showDue;
        public KiemTraForm()
        {
            Init();
            AddExtraButton("Phòng đến hạn kiểm tra", Theme.Warning, delegate
            {
                showDue = !showDue;
                Reload();
            }, 200);
        }
        protected override string Title { get { return "Quản lý kiểm tra văn phòng"; } }
        protected override string IdColumn { get { return "MaPKT"; } }
        protected override void DefineFields()
        {
            Fields.Add(new FieldDef("MaPKT", "Số phiếu", FieldType.Integer).GridCol().Hide());
            Fields.Add(new FieldDef("MaVP", "Văn phòng", FieldType.Combo).Lookup(VanPhongBll.Lookup(), "MaVP", "Ten").Req().Hide());
            Fields.Add(new FieldDef("MaPhong", "Phòng").GridCol());
            Fields.Add(new FieldDef("TenTang", "Tầng").GridCol());
            Fields.Add(new FieldDef("NgayKiemTra", "Ngày kiểm tra", FieldType.Date));
            Fields.Add(new FieldDef("NguoiKiemTra", "Người kiểm tra").Req().Len(100));
            Fields.Add(new FieldDef("TinhTrang", "Tình trạng", FieldType.Combo).Choices("Tốt", "Cần sửa chữa", "Hư hỏng").Req());
            Fields.Add(new FieldDef("CoViPham", "Tự ý thay đổi kết cấu", FieldType.Check));
            Fields.Add(new FieldDef("GhiChu", "Ghi chú", FieldType.Multiline).Len(300));
            Fields.Add(new FieldDef("TrangThaiPhong", "Trạng thái phòng").GridCol());
            Fields.Add(new FieldDef("LanKiemTraCuoi", "Lần kiểm tra cuối", FieldType.Date).GridCol());
        }
        protected override void OnNew() { SetFieldValue("NguoiKiemTra", Session.FullName); }
        protected override DataTable LoadData(string k) { return showDue ? KiemTraBll.DueRooms() : KiemTraBll.Search(k); }
        protected override void DoInsert(Dictionary<string, object> v) { KiemTraBll.Insert(v); }
        protected override void DoUpdate(object id, Dictionary<string, object> v) { KiemTraBll.Update(Convert.ToInt32(id), v); }
        protected override void DoDelete(object id) { KiemTraBll.Delete(Convert.ToInt32(id)); }
    }

    // ------------------------------------------ 14. Vi phạm và tiền phạt
    public class ViPhamForm : BaseListForm
    {
        public ViPhamForm()
        {
            Init();
            AddExtraButton("Ghi nhận nộp phạt", Theme.Success, delegate
            {
                DataRow r = CurrentRow;
                if (r == null) { Theme.Warn("Vui lòng chọn biên bản cần ghi nhận."); return; }
                if (!Theme.Confirm("Ghi nhận khách đã nộp phạt cho biên bản này?")) return;
                try
                {
                    ViPhamBll.Pay(Convert.ToInt32(r["MaVPh"]));
                    Theme.Info("Đã ghi nhận nộp phạt.");
                    Reload();
                }
                catch (Exception ex) { Theme.Error(QuanLyVanPhong.DAL.Db.Friendly(ex)); }
            }, 170);
        }
        protected override string Title { get { return "Quản lý vi phạm và tiền phạt"; } }
        protected override string IdColumn { get { return "MaVPh"; } }
        protected override void DefineFields()
        {
            Fields.Add(new FieldDef("MaVPh", "Số biên bản", FieldType.Integer).GridCol().Hide());
            Fields.Add(new FieldDef("MaPKT", "Phiếu kiểm tra", FieldType.Combo).Lookup(ViPhamBll.InspectionLookup(), "MaPKT", "Ten").Req());
            Fields.Add(new FieldDef("MaPhong", "Phòng").GridCol());
            Fields.Add(new FieldDef("SoHD", "Hợp đồng").GridCol());
            Fields.Add(new FieldDef("NgayLap", "Ngày lập", FieldType.Date));
            Fields.Add(new FieldDef("MucPhat", "Mức phạt (đ)", FieldType.Decimal).Req());
            Fields.Add(new FieldDef("TrangThai", "Trạng thái", FieldType.Combo).Choices("Chưa nộp", "Đã nộp").Req());
            Fields.Add(new FieldDef("NgayNop", "Ngày nộp", FieldType.Date).GridCol());
            Fields.Add(new FieldDef("NoiDung", "Nội dung vi phạm", FieldType.Multiline).Req().Len(300));
        }
        protected override void OnNew() { SetFieldValue("TrangThai", "Chưa nộp"); }
        protected override DataTable LoadData(string k) { return ViPhamBll.Search(k); }
        protected override void DoInsert(Dictionary<string, object> v) { ViPhamBll.Insert(v); }
        protected override void DoUpdate(object id, Dictionary<string, object> v) { ViPhamBll.Update(Convert.ToInt32(id), v); }
        protected override void DoDelete(object id) { ViPhamBll.Delete(Convert.ToInt32(id)); }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyVanPhong.BLL;
using QuanLyVanPhong.Common;
using QuanLyVanPhong.DAL;

namespace QuanLyVanPhong.Presentation
{
    /// <summary>6. Quản lý hợp đồng thuê (chọn nhiều văn phòng trong cùng một hợp đồng).</summary>
    public class HopDongForm : EmbeddedForm
    {
        private enum Mode { View, Add, EditNote }
        private Mode mode = Mode.View;

        private TextBox txtSearch, txtSoHD, txtGhiChu;
        private DataGridView gridHd, gridRooms;
        private ComboBox cboKhach;
        private DateTimePicker dtKy, dtBatDau;
        private NumericUpDown nudThang;
        private CheckBox chkThu6;
        private Label lblKetThuc, lblTongThang, lblTong6;
        private Button btnAdd, btnEdit, btnDelete, btnSave, btnCancel, btnLiquidate;
        private bool loading;

        public HopDongForm()
        {
            BuildUi();
            LoadContracts();
            ApplyMode(Mode.View);
        }

        private DataRow Current
        {
            get
            {
                if (gridHd.CurrentRow == null) return null;
                var v = gridHd.CurrentRow.DataBoundItem as DataRowView;
                return v == null ? null : v.Row;
            }
        }

        private void BuildUi()
        {
            var root = new TableLayoutPanel();
            root.Dock = DockStyle.Fill;
            root.ColumnCount = 1;
            root.RowCount = 5;
            root.Padding = new Padding(16, 12, 16, 12);
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 42));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 58));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var title = new Label();
            title.Text = "QUẢN LÝ HỢP ĐỒNG THUÊ";
            title.Font = Theme.Title;
            title.ForeColor = Theme.Primary;
            title.AutoSize = true;
            title.Margin = new Padding(0, 0, 0, 8);
            root.Controls.Add(title, 0, 0);

            var bar = new FlowLayoutPanel();
            bar.AutoSize = true;
            bar.WrapContents = false;
            bar.Margin = new Padding(0, 0, 0, 8);
            txtSearch = new TextBox();
            txtSearch.Width = 280;
            txtSearch.Margin = new Padding(0, 6, 8, 0);
            txtSearch.KeyDown += delegate (object s, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; LoadContracts(); }
            };
            var btnSearch = Theme.Btn("Tìm kiếm", Theme.Accent);
            btnSearch.Click += delegate { LoadContracts(); };
            var btnRefresh = Theme.Btn("Làm mới", Theme.Primary);
            btnRefresh.Click += delegate { txtSearch.Clear(); LoadContracts(); };
            var lbl = new Label();
            lbl.Text = "Từ khóa:";
            lbl.AutoSize = true;
            lbl.Margin = new Padding(0, 10, 8, 0);
            bar.Controls.AddRange(new Control[] { lbl, txtSearch, btnSearch, btnRefresh });
            root.Controls.Add(bar, 0, 1);

            gridHd = new DataGridView();
            gridHd.Dock = DockStyle.Fill;
            Theme.StyleGrid(gridHd);
            gridHd.SelectionChanged += delegate { if (mode == Mode.View) ShowCurrent(); ApplyButtons(); };
            root.Controls.Add(gridHd, 0, 2);

            // khu nhập liệu: trái = thông tin hợp đồng, phải = chọn văn phòng
            var split = new TableLayoutPanel();
            split.Dock = DockStyle.Fill;
            split.ColumnCount = 2;
            split.RowCount = 1;
            split.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42));
            split.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58));

            var g1 = new GroupBox();
            g1.Text = "Thông tin hợp đồng";
            g1.Dock = DockStyle.Fill;
            g1.Font = Theme.Bold;
            g1.ForeColor = Theme.Primary;
            var t = new TableLayoutPanel();
            t.Dock = DockStyle.Fill;
            t.AutoScroll = true;
            t.ColumnCount = 2;
            t.Font = Theme.Base;
            t.ForeColor = Theme.TextColor;
            t.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            txtSoHD = new TextBox(); txtSoHD.ReadOnly = true;
            cboKhach = new ComboBox(); cboKhach.DropDownStyle = ComboBoxStyle.DropDownList;
            dtKy = NewDate(); dtBatDau = NewDate();
            nudThang = new NumericUpDown(); nudThang.Minimum = 6; nudThang.Maximum = 120; nudThang.Value = 6;
            chkThu6 = new CheckBox(); chkThu6.Text = "Thu tiền 6 tháng đầu ngay khi ký"; chkThu6.Checked = true; chkThu6.AutoSize = true;
            txtGhiChu = new TextBox();
            lblKetThuc = new Label(); lblKetThuc.AutoSize = true;
            lblTongThang = new Label(); lblTongThang.AutoSize = true;
            lblTong6 = new Label(); lblTong6.AutoSize = true; lblTong6.ForeColor = Theme.Danger; lblTong6.Font = Theme.Bold;

            AddRow(t, "Số hợp đồng", txtSoHD);
            AddRow(t, "Khách hàng *", cboKhach);
            AddRow(t, "Ngày ký *", dtKy);
            AddRow(t, "Ngày bắt đầu *", dtBatDau);
            AddRow(t, "Số tháng thuê * (≥ 6)", nudThang);
            AddRow(t, "Ngày kết thúc", lblKetThuc);
            AddRow(t, "Tiền thuê / tháng", lblTongThang);
            AddRow(t, "Trả trước 6 tháng đầu", lblTong6);
            AddRow(t, "", chkThu6);
            AddRow(t, "Ghi chú", txtGhiChu);
            g1.Controls.Add(t);
            split.Controls.Add(g1, 0, 0);

            var g2 = new GroupBox();
            g2.Text = "Văn phòng thuê (tick chọn nhiều văn phòng)";
            g2.Dock = DockStyle.Fill;
            g2.Font = Theme.Bold;
            g2.ForeColor = Theme.Primary;
            gridRooms = new DataGridView();
            gridRooms.Dock = DockStyle.Fill;
            Theme.StyleGrid(gridRooms);
            gridRooms.ReadOnly = false;
            gridRooms.CurrentCellDirtyStateChanged += delegate
            {
                if (gridRooms.IsCurrentCellDirty) gridRooms.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };
            gridRooms.CellValueChanged += delegate { if (!loading) UpdateTotals(); };
            g2.Controls.Add(gridRooms);
            split.Controls.Add(g2, 1, 0);
            root.Controls.Add(split, 0, 3);

            var btns = new FlowLayoutPanel();
            btns.AutoSize = true;
            btns.WrapContents = false;
            btns.Margin = new Padding(0, 8, 0, 0);
            btnAdd = Theme.Btn("Thêm", Theme.Success);
            btnEdit = Theme.Btn("Sửa ghi chú", Theme.Accent, 120);
            btnDelete = Theme.Btn("Xóa", Theme.Danger);
            btnSave = Theme.Btn("Lưu", Theme.Primary);
            btnCancel = Theme.Btn("Hủy", Color.Gray);
            btnLiquidate = Theme.Btn("Thanh lý", Theme.Warning);
            btnAdd.Click += delegate { ApplyMode(Mode.Add); };
            btnEdit.Click += delegate { if (Current != null) ApplyMode(Mode.EditNote); };
            btnDelete.Click += delegate { DeleteContract(); };
            btnSave.Click += delegate { Save(); };
            btnCancel.Click += delegate { ApplyMode(Mode.View); };
            btnLiquidate.Click += delegate { Liquidate(); };
            btns.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete, btnSave, btnCancel, btnLiquidate });
            root.Controls.Add(btns, 0, 4);

            Controls.Add(root);

            dtKy.ValueChanged += delegate { if (mode == Mode.Add) { dtBatDau.Value = dtKy.Value; UpdateTotals(); } };
            dtBatDau.ValueChanged += delegate { UpdateTotals(); };
            nudThang.ValueChanged += delegate { UpdateTotals(); };
        }

        private static DateTimePicker NewDate()
        {
            var d = new DateTimePicker();
            d.Format = DateTimePickerFormat.Custom;
            d.CustomFormat = "dd/MM/yyyy";
            return d;
        }

        private static void AddRow(TableLayoutPanel t, string label, Control c)
        {
            var l = new Label();
            l.Text = label;
            l.AutoSize = true;
            l.Margin = new Padding(0, 8, 12, 4);
            c.Margin = new Padding(0, 4, 8, 4);
            if (c is TextBox || c is ComboBox) c.Dock = DockStyle.Fill;
            int r = t.RowCount++;
            t.Controls.Add(l, 0, r);
            t.Controls.Add(c, 1, r);
        }

        // -------------------------------------------------------------- dữ liệu
        private void LoadContracts()
        {
            try
            {
                gridHd.DataSource = ContractBll.Search(txtSearch.Text.Trim());
                Theme.FormatColumns(gridHd);
                var heads = new Dictionary<string, string> {
                    {"MaHD","ID"},{"SoHD","Số HĐ"},{"MaKH","MaKH"},{"TenKH","Khách hàng"},{"TenCT","Công ty"},{"NgayKy","Ngày ký"},
                    {"NgayBatDau","Bắt đầu"},{"SoThang","Số tháng"},{"NgayKetThuc","Kết thúc"},{"DanhSachPhong","Các phòng"},
                    {"TongThueThang","Tiền thuê/tháng"},{"TrangThai","Trạng thái"},{"GhiChu","Ghi chú"} };
                foreach (DataGridViewColumn c in gridHd.Columns)
                {
                    string h;
                    if (heads.TryGetValue(c.DataPropertyName, out h)) c.HeaderText = h;
                    if (c.DataPropertyName == "MaHD" || c.DataPropertyName == "MaKH") c.Visible = false;
                }
            }
            catch (Exception ex) { Theme.Error("Không tải được dữ liệu: " + Db.Friendly(ex)); }
            if (mode == Mode.View) ShowCurrent();
            ApplyButtons();
        }

        private void LoadKhach()
        {
            cboKhach.DisplayMember = "Ten";
            cboKhach.ValueMember = "MaKH";
            cboKhach.DataSource = ContractBll.KhachLookup();
            cboKhach.SelectedIndex = -1;
        }

        private DataTable BuildRoomTable(DataTable rooms, bool checkedAll)
        {
            var dt = new DataTable();
            dt.Columns.Add("Chon", typeof(bool));
            dt.Columns.Add("MaVP", typeof(int));
            dt.Columns.Add("MaPhong", typeof(string));
            dt.Columns.Add("TenTang", typeof(string));
            dt.Columns.Add("DienTich", typeof(decimal));
            dt.Columns.Add("Huong", typeof(string));
            dt.Columns.Add("GiaThue", typeof(decimal));
            foreach (DataRow r in rooms.Rows)
                dt.Rows.Add(checkedAll, r["MaVP"], r["MaPhong"], r["TenTang"], r["DienTich"], r["Huong"], r["GiaThue"]);
            return dt;
        }

        private void BindRooms(DataTable dt, bool editable)
        {
            loading = true;
            gridRooms.DataSource = dt;
            Theme.FormatColumns(gridRooms);
            string[] heads = { "Chọn", "ID", "Phòng", "Tầng", "Diện tích (m²)", "Hướng", "Giá thuê/tháng" };
            for (int i = 0; i < gridRooms.Columns.Count; i++)
            {
                gridRooms.Columns[i].HeaderText = heads[i];
                gridRooms.Columns[i].ReadOnly = !(editable && i == 0);
            }
            gridRooms.Columns["MaVP"].Visible = false;
            loading = false;
        }

        private void ShowCurrent()
        {
            DataRow r = Current;
            if (r == null)
            {
                txtSoHD.Text = ""; txtGhiChu.Text = "";
                BindRooms(BuildRoomTable(new DataTable(), false).Clone(), false);
                lblKetThuc.Text = lblTongThang.Text = lblTong6.Text = "";
                return;
            }
            loading = true;
            LoadKhachIfNeeded();
            txtSoHD.Text = r["SoHD"].ToString();
            cboKhach.SelectedValue = r["MaKH"];
            dtKy.Value = Convert.ToDateTime(r["NgayKy"]);
            dtBatDau.Value = Convert.ToDateTime(r["NgayBatDau"]);
            nudThang.Value = Convert.ToDecimal(r["SoThang"]);
            txtGhiChu.Text = r["GhiChu"] == DBNull.Value ? "" : r["GhiChu"].ToString();
            lblKetThuc.Text = Convert.ToDateTime(r["NgayKetThuc"]).ToString("dd/MM/yyyy") + "  (" + r["TrangThai"] + ")";
            loading = false;
            BindRooms(BuildRoomTable(ContractBll.Rooms(Convert.ToInt32(r["MaHD"])), true), false);
            decimal tong = r["TongThueThang"] == DBNull.Value ? 0 : Convert.ToDecimal(r["TongThueThang"]);
            lblTongThang.Text = tong.ToString("N0") + " đ";
            lblTong6.Text = (tong * 6).ToString("N0") + " đ";
        }

        private void LoadKhachIfNeeded()
        {
            if (cboKhach.DataSource == null) LoadKhach();
        }

        private void UpdateTotals()
        {
            if (mode != Mode.Add) return;
            decimal tong = 0;
            foreach (DataGridViewRow row in gridRooms.Rows)
                if (Convert.ToBoolean(row.Cells["Chon"].Value)) tong += Convert.ToDecimal(row.Cells["GiaThue"].Value);
            lblTongThang.Text = tong.ToString("N0") + " đ";
            lblTong6.Text = (tong * 6).ToString("N0") + " đ";
            lblKetThuc.Text = ContractBll.EndDate(dtBatDau.Value.Date, (int)nudThang.Value).ToString("dd/MM/yyyy");
        }

        // --------------------------------------------------------------- chế độ
        private void ApplyMode(Mode m)
        {
            mode = m;
            bool add = m == Mode.Add;
            cboKhach.Enabled = dtKy.Enabled = dtBatDau.Enabled = nudThang.Enabled = chkThu6.Enabled = add;
            txtGhiChu.Enabled = add || m == Mode.EditNote;
            gridHd.Enabled = m == Mode.View;
            if (add)
            {
                LoadKhach();
                loading = true;
                txtSoHD.Text = "(tự sinh khi lưu)";
                dtKy.Value = DateTime.Today; dtBatDau.Value = DateTime.Today; nudThang.Value = 6;
                chkThu6.Checked = true; txtGhiChu.Text = "";
                loading = false;
                BindRooms(BuildRoomTable(ContractBll.Available(), false), true);
                UpdateTotals();
            }
            else if (m == Mode.View) ShowCurrent();
            ApplyButtons();
        }

        private void ApplyButtons()
        {
            if (btnAdd == null) return;
            bool view = mode == Mode.View;
            bool has = Current != null;
            bool active = has && Current["TrangThai"].ToString() == "Hiệu lực";
            btnAdd.Enabled = view;
            btnEdit.Enabled = view && has;
            btnDelete.Enabled = view && has;
            btnLiquidate.Enabled = view && active;
            btnSave.Enabled = !view;
            btnCancel.Enabled = !view;
        }

        // --------------------------------------------------------------- hành động
        private void Save()
        {
            try
            {
                if (mode == Mode.EditNote)
                {
                    ContractBll.UpdateNote(Convert.ToInt32(Current["MaHD"]), txtGhiChu.Text.Trim());
                    Theme.Info("Cập nhật ghi chú thành công.");
                    ApplyMode(Mode.View); LoadContracts();
                    return;
                }
                if (cboKhach.SelectedValue == null) { Theme.Warn("Vui lòng chọn khách hàng."); return; }
                var c = new ContractInput();
                c.MaKH = Convert.ToInt32(cboKhach.SelectedValue);
                c.NgayKy = dtKy.Value.Date;
                c.NgayBatDau = dtBatDau.Value.Date;
                c.SoThang = (int)nudThang.Value;
                c.ThuSauThangDau = chkThu6.Checked;
                c.GhiChu = txtGhiChu.Text.Trim();
                foreach (DataGridViewRow row in gridRooms.Rows)
                    if (Convert.ToBoolean(row.Cells["Chon"].Value)) c.Rooms.Add(Convert.ToInt32(row.Cells["MaVP"].Value));
                if (c.Rooms.Count == 0) { Theme.Warn("Vui lòng chọn ít nhất một văn phòng."); return; }
                int id = ContractBll.Create(c);
                Theme.Info("Lập hợp đồng thành công (" + c.Rooms.Count + " văn phòng). Lịch thanh toán tiền thuê đã được phát sinh.");
                ApplyMode(Mode.View); LoadContracts();
            }
            catch (Exception ex) { Theme.Error("Lưu thất bại: " + Db.Friendly(ex)); }
        }

        private void DeleteContract()
        {
            DataRow r = Current;
            if (r == null) return;
            if (!Theme.Confirm("Xóa hợp đồng " + r["SoHD"] + "? Chỉ xóa được hợp đồng nhập nhầm, chưa phát sinh thanh toán.")) return;
            try
            {
                ContractBll.Delete(Convert.ToInt32(r["MaHD"]));
                Theme.Info("Đã xóa hợp đồng.");
                LoadContracts();
            }
            catch (Exception ex) { Theme.Error("Xóa thất bại: " + Db.Friendly(ex)); }
        }

        private void Liquidate()
        {
            DataRow r = Current;
            if (r == null) return;
            if (!Theme.Confirm("Thanh lý hợp đồng " + r["SoHD"] + "? Các văn phòng sẽ được trả về trạng thái Trống.")) return;
            try
            {
                ContractBll.Terminate(Convert.ToInt32(r["MaHD"]));
                Theme.Info("Đã thanh lý hợp đồng.");
                LoadContracts();
            }
            catch (Exception ex) { Theme.Error("Thanh lý thất bại: " + Db.Friendly(ex)); }
        }
    }
}

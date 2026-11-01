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
    /// <summary>10. Lập hóa đơn dịch vụ (điện, nước theo chỉ số; internet... cố định).</summary>
    public class HoaDonForm : EmbeddedForm
    {
        private ComboBox cboHd;
        private NumericUpDown nudThang, nudNam;
        private DateTimePicker dtLap, dtPay;
        private DataGridView gridLines, gridInvoices, gridDetail;
        private Label lblTong;
        private TextBox txtSearch;
        private DataTable lines;
        private bool busy;

        public HoaDonForm()
        {
            BuildUi();
            LoadContracts();
            LoadInvoices();
        }

        private void BuildUi()
        {
            var root = new TableLayoutPanel();
            root.Dock = DockStyle.Fill;
            root.ColumnCount = 1;
            root.RowCount = 6;
            root.Padding = new Padding(16, 12, 16, 12);
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 42));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 58));

            var title = new Label();
            title.Text = "LẬP HÓA ĐƠN DỊCH VỤ";
            title.Font = Theme.Title;
            title.ForeColor = Theme.Primary;
            title.AutoSize = true;
            title.Margin = new Padding(0, 0, 0, 8);
            root.Controls.Add(title, 0, 0);

            var top = new FlowLayoutPanel();
            top.AutoSize = true;
            top.WrapContents = false;
            top.Margin = new Padding(0, 0, 0, 8);
            cboHd = new ComboBox(); cboHd.DropDownStyle = ComboBoxStyle.DropDownList; cboHd.Width = 280; cboHd.Margin = new Padding(0, 6, 12, 0);
            nudThang = new NumericUpDown(); nudThang.Minimum = 1; nudThang.Maximum = 12; nudThang.Value = DateTime.Today.Month; nudThang.Width = 55; nudThang.Margin = new Padding(0, 6, 12, 0);
            nudNam = new NumericUpDown(); nudNam.Minimum = 2000; nudNam.Maximum = 2100; nudNam.Value = DateTime.Today.Year; nudNam.Width = 75; nudNam.Margin = new Padding(0, 6, 12, 0);
            dtLap = new DateTimePicker(); dtLap.Format = DateTimePickerFormat.Custom; dtLap.CustomFormat = "dd/MM/yyyy"; dtLap.Width = 120; dtLap.Margin = new Padding(0, 6, 12, 0);
            var btnLoad = Theme.Btn("Nạp dịch vụ", Theme.Accent, 120);
            btnLoad.Click += delegate { LoadLines(); };
            top.Controls.Add(Lbl("Hợp đồng:")); top.Controls.Add(cboHd);
            top.Controls.Add(Lbl("Tháng:")); top.Controls.Add(nudThang);
            top.Controls.Add(Lbl("Năm:")); top.Controls.Add(nudNam);
            top.Controls.Add(Lbl("Ngày lập:")); top.Controls.Add(dtLap);
            top.Controls.Add(btnLoad);
            root.Controls.Add(top, 0, 1);

            gridLines = new DataGridView();
            gridLines.Dock = DockStyle.Fill;
            Theme.StyleGrid(gridLines);
            gridLines.ReadOnly = false;
            gridLines.CurrentCellDirtyStateChanged += delegate
            {
                if (gridLines.IsCurrentCellDirty) gridLines.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };
            gridLines.CellValueChanged += delegate (object s, DataGridViewCellEventArgs e) { if (e.RowIndex >= 0) Recalc(e.RowIndex); };
            gridLines.DataError += delegate (object s, DataGridViewDataErrorEventArgs e) { e.ThrowException = false; };
            root.Controls.Add(gridLines, 0, 2);

            var act = new FlowLayoutPanel();
            act.AutoSize = true;
            act.WrapContents = false;
            act.Margin = new Padding(0, 8, 0, 8);
            var btnCalc = Theme.Btn("Tính tiền", Theme.Accent);
            var btnCreate = Theme.Btn("Lập hóa đơn", Theme.Success, 130);
            var btnCancel = Theme.Btn("Hủy", Color.Gray);
            btnCalc.Click += delegate { RecalcAll(); };
            btnCreate.Click += delegate { CreateInvoice(); };
            btnCancel.Click += delegate { lines = null; gridLines.DataSource = null; lblTong.Text = "Tổng tiền: 0 đ"; };
            lblTong = new Label(); lblTong.Text = "Tổng tiền: 0 đ"; lblTong.AutoSize = true; lblTong.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTong.ForeColor = Theme.Danger; lblTong.Margin = new Padding(16, 6, 0, 0);
            act.Controls.AddRange(new Control[] { btnCalc, btnCreate, btnCancel, lblTong });
            root.Controls.Add(act, 0, 3);

            var bar = new FlowLayoutPanel();
            bar.AutoSize = true;
            bar.WrapContents = false;
            txtSearch = new TextBox(); txtSearch.Width = 220; txtSearch.Margin = new Padding(0, 6, 8, 0);
            txtSearch.KeyDown += delegate (object s, KeyEventArgs e) { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; LoadInvoices(); } };
            var btnS = Theme.Btn("Tìm kiếm", Theme.Accent);
            btnS.Click += delegate { LoadInvoices(); };
            dtPay = new DateTimePicker(); dtPay.Format = DateTimePickerFormat.Custom; dtPay.CustomFormat = "dd/MM/yyyy"; dtPay.Width = 120; dtPay.Margin = new Padding(12, 6, 8, 0);
            var btnPay = Theme.Btn("Thanh toán", Theme.Success, 120);
            var btnDel = Theme.Btn("Xóa hóa đơn", Theme.Danger, 120);
            btnPay.Click += delegate { PayInvoice(); };
            btnDel.Click += delegate { DeleteInvoice(); };
            var lb = new Label(); lb.Text = "Danh sách hóa đơn:"; lb.Font = Theme.Bold; lb.AutoSize = true; lb.Margin = new Padding(0, 10, 8, 0);
            bar.Controls.AddRange(new Control[] { lb, txtSearch, btnS, Lbl("Ngày thu:"), dtPay, btnPay, btnDel });
            root.Controls.Add(bar, 0, 4);

            var lower = new TableLayoutPanel();
            lower.Dock = DockStyle.Fill;
            lower.ColumnCount = 2;
            lower.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62));
            lower.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38));
            gridInvoices = new DataGridView(); gridInvoices.Dock = DockStyle.Fill; Theme.StyleGrid(gridInvoices);
            gridInvoices.SelectionChanged += delegate { ShowDetail(); };
            gridDetail = new DataGridView(); gridDetail.Dock = DockStyle.Fill; Theme.StyleGrid(gridDetail);
            gridDetail.Margin = new Padding(8, 3, 3, 3);
            lower.Controls.Add(gridInvoices, 0, 0);
            lower.Controls.Add(gridDetail, 1, 0);
            root.Controls.Add(lower, 0, 5);

            Controls.Add(root);
        }

        private static Label Lbl(string t) { var l = new Label(); l.Text = t; l.AutoSize = true; l.Margin = new Padding(0, 10, 6, 0); return l; }

        private void LoadContracts()
        {
            try
            {
                cboHd.DisplayMember = "Ten";
                cboHd.ValueMember = "MaHD";
                cboHd.DataSource = ContractBll.ActiveLookup();
                cboHd.SelectedIndex = -1;
            }
            catch (Exception ex) { Theme.Error(Db.Friendly(ex)); }
        }

        private void LoadLines()
        {
            if (cboHd.SelectedValue == null) { Theme.Warn("Vui lòng chọn hợp đồng."); return; }
            try
            {
                lines = HoaDonBll.PrepareLines(Convert.ToInt32(cboHd.SelectedValue), (int)nudThang.Value, (int)nudNam.Value);
                busy = true;
                gridLines.DataSource = lines;
                Theme.FormatColumns(gridLines);
                var heads = new Dictionary<string, string> {
                    {"Chon","Chọn"},{"MaDV","ID"},{"TenDV","Dịch vụ"},{"DonVi","Đơn vị"},{"LoaiTinh","Cách tính"},{"DonGia","Đơn giá"},
                    {"ChiSoCu","Chỉ số cũ"},{"ChiSoMoi","Chỉ số mới"},{"SoLuong","Số lượng"},{"ThanhTien","Thành tiền"} };
                foreach (DataGridViewColumn c in gridLines.Columns)
                {
                    c.HeaderText = heads[c.DataPropertyName];
                    c.ReadOnly = !(c.DataPropertyName == "Chon" || c.DataPropertyName == "ChiSoCu" || c.DataPropertyName == "ChiSoMoi");
                }
                gridLines.Columns["MaDV"].Visible = false;
                busy = false;
                RecalcAll();
            }
            catch (Exception ex) { busy = false; Theme.Error(Db.Friendly(ex)); }
        }

        private void Recalc(int rowIndex)
        {
            if (busy || lines == null || rowIndex >= lines.Rows.Count) return;
            busy = true;
            try
            {
                DataRow r = lines.Rows[rowIndex];
                bool meter = r["LoaiTinh"].ToString() == "Theo chỉ số";
                decimal sl = 1;
                if (meter)
                {
                    decimal cu = r["ChiSoCu"] == DBNull.Value ? 0 : Convert.ToDecimal(r["ChiSoCu"]);
                    decimal moi = r["ChiSoMoi"] == DBNull.Value ? 0 : Convert.ToDecimal(r["ChiSoMoi"]);
                    sl = Math.Max(0, moi - cu);
                }
                r["SoLuong"] = sl;
                r["ThanhTien"] = Math.Round(sl * Convert.ToDecimal(r["DonGia"]), 0);
            }
            finally { busy = false; }
            UpdateTotal();
        }

        private void RecalcAll()
        {
            if (lines == null) return;
            for (int i = 0; i < lines.Rows.Count; i++) Recalc(i);
            UpdateTotal();
        }

        private void UpdateTotal()
        {
            decimal t = 0;
            if (lines != null)
                foreach (DataRow r in lines.Rows)
                    if (r["Chon"] != DBNull.Value && Convert.ToBoolean(r["Chon"])) t += Convert.ToDecimal(r["ThanhTien"]);
            lblTong.Text = "Tổng tiền: " + t.ToString("N0") + " đ";
        }

        private void CreateInvoice()
        {
            if (cboHd.SelectedValue == null) { Theme.Warn("Vui lòng chọn hợp đồng."); return; }
            if (lines == null) { Theme.Warn("Vui lòng nạp danh sách dịch vụ trước."); return; }
            RecalcAll();
            var list = new List<InvoiceLine>();
            foreach (DataRow r in lines.Rows)
            {
                if (!Convert.ToBoolean(r["Chon"])) continue;
                var l = new InvoiceLine();
                l.MaDV = Convert.ToInt32(r["MaDV"]);
                l.TheoChiSo = r["LoaiTinh"].ToString() == "Theo chỉ số";
                l.DonGia = Convert.ToDecimal(r["DonGia"]);
                l.SoLuong = Convert.ToDecimal(r["SoLuong"]);
                if (l.TheoChiSo)
                {
                    l.ChiSoCu = r["ChiSoCu"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["ChiSoCu"]);
                    l.ChiSoMoi = r["ChiSoMoi"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["ChiSoMoi"]);
                }
                list.Add(l);
            }
            if (!Theme.Confirm("Lập hóa đơn dịch vụ tháng " + nudThang.Value + "/" + nudNam.Value + "?")) return;
            try
            {
                HoaDonBll.Create(Convert.ToInt32(cboHd.SelectedValue), (int)nudThang.Value, (int)nudNam.Value, dtLap.Value.Date, list);
                Theme.Info("Lập hóa đơn dịch vụ thành công.");
                lines = null; gridLines.DataSource = null; lblTong.Text = "Tổng tiền: 0 đ";
                LoadInvoices();
            }
            catch (Exception ex) { Theme.Error("Lập hóa đơn thất bại: " + Db.Friendly(ex)); }
        }

        private void LoadInvoices()
        {
            try
            {
                gridInvoices.DataSource = HoaDonBll.Search(txtSearch.Text.Trim());
                Theme.FormatColumns(gridInvoices);
                var heads = new Dictionary<string, string> {
                    {"SoHD","Số HĐ"},{"TenKH","Khách hàng"},{"Thang","Tháng"},{"Nam","Năm"},{"NgayLap","Ngày lập"},
                    {"TongTien","Tổng tiền (đ)"},{"NgayThanhToan","Ngày thanh toán"},{"TrangThai","Trạng thái"} };
                foreach (DataGridViewColumn c in gridInvoices.Columns)
                {
                    string h;
                    if (heads.TryGetValue(c.DataPropertyName, out h)) c.HeaderText = h;
                    if (c.DataPropertyName == "MaHDDV" || c.DataPropertyName == "MaHD") c.Visible = false;
                }
            }
            catch (Exception ex) { Theme.Error(Db.Friendly(ex)); }
            ShowDetail();
        }

        private int? CurrentInvoice()
        {
            if (gridInvoices.CurrentRow == null) return null;
            var d = gridInvoices.CurrentRow.DataBoundItem as DataRowView;
            return d == null ? (int?)null : Convert.ToInt32(d["MaHDDV"]);
        }

        private void ShowDetail()
        {
            int? id = CurrentInvoice();
            if (!id.HasValue) { gridDetail.DataSource = null; return; }
            try
            {
                gridDetail.DataSource = HoaDonBll.Details(id.Value);
                Theme.FormatColumns(gridDetail);
                var heads = new Dictionary<string, string> {
                    {"TenDV","Dịch vụ"},{"DonVi","ĐVT"},{"ChiSoCu","Chỉ số cũ"},{"ChiSoMoi","Chỉ số mới"},{"SoLuong","SL"},{"DonGia","Đơn giá"},{"ThanhTien","Thành tiền"} };
                foreach (DataGridViewColumn c in gridDetail.Columns)
                {
                    string h;
                    if (heads.TryGetValue(c.DataPropertyName, out h)) c.HeaderText = h;
                }
            }
            catch (Exception ex) { Theme.Error(Db.Friendly(ex)); }
        }

        private void PayInvoice()
        {
            int? id = CurrentInvoice();
            if (!id.HasValue) { Theme.Warn("Vui lòng chọn hóa đơn."); return; }
            if (!Theme.Confirm("Xác nhận đã thu tiền hóa đơn này?")) return;
            try { HoaDonBll.Pay(id.Value, dtPay.Value.Date); Theme.Info("Thanh toán hóa đơn thành công."); LoadInvoices(); }
            catch (Exception ex) { Theme.Error("Thanh toán thất bại: " + Db.Friendly(ex)); }
        }

        private void DeleteInvoice()
        {
            int? id = CurrentInvoice();
            if (!id.HasValue) { Theme.Warn("Vui lòng chọn hóa đơn."); return; }
            if (!Theme.Confirm("Bạn có chắc chắn muốn xóa hóa đơn đã chọn?")) return;
            try { HoaDonBll.Delete(id.Value); Theme.Info("Đã xóa hóa đơn."); LoadInvoices(); }
            catch (Exception ex) { Theme.Error("Xóa thất bại: " + Db.Friendly(ex)); }
        }
    }
}

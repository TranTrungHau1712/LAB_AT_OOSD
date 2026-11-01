using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyVanPhong.BLL;
using QuanLyVanPhong.DAL;

namespace QuanLyVanPhong.Presentation
{
    /// <summary>8. Quản lý thanh toán tiền thuê (kỳ thanh toán, số tiền, ngày thanh toán, trạng thái).</summary>
    public class ThanhToanForm : EmbeddedForm
    {
        private ComboBox cboHd, cboTrangThai;
        private TextBox txtSearch;
        private DataGridView grid;
        private DateTimePicker dtPay;
        private Label lblSummary;

        public ThanhToanForm()
        {
            BuildUi();
            LoadFilter();
            Reload();
        }

        private void BuildUi()
        {
            var root = new TableLayoutPanel();
            root.Dock = DockStyle.Fill;
            root.ColumnCount = 1;
            root.RowCount = 4;
            root.Padding = new Padding(16, 12, 16, 12);
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var title = new Label();
            title.Text = "THANH TOÁN TIỀN THUÊ";
            title.Font = Theme.Title;
            title.ForeColor = Theme.Primary;
            title.AutoSize = true;
            title.Margin = new Padding(0, 0, 0, 8);
            root.Controls.Add(title, 0, 0);

            var bar = new FlowLayoutPanel();
            bar.AutoSize = true;
            bar.WrapContents = false;
            bar.Margin = new Padding(0, 0, 0, 8);
            cboHd = new ComboBox(); cboHd.DropDownStyle = ComboBoxStyle.DropDownList; cboHd.Width = 280; cboHd.Margin = new Padding(0, 6, 12, 0);
            cboTrangThai = new ComboBox(); cboTrangThai.DropDownStyle = ComboBoxStyle.DropDownList; cboTrangThai.Width = 150; cboTrangThai.Margin = new Padding(0, 6, 12, 0);
            cboTrangThai.Items.AddRange(new object[] { "Tất cả", "Chưa thanh toán", "Đã thanh toán" });
            cboTrangThai.SelectedIndex = 0;
            txtSearch = new TextBox(); txtSearch.Width = 180; txtSearch.Margin = new Padding(0, 6, 8, 0);
            txtSearch.KeyDown += delegate (object s, KeyEventArgs e) { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; Reload(); } };
            var btnS = Theme.Btn("Tìm kiếm", Theme.Accent);
            btnS.Click += delegate { Reload(); };
            var btnR = Theme.Btn("Làm mới", Theme.Primary);
            btnR.Click += delegate { txtSearch.Clear(); cboTrangThai.SelectedIndex = 0; LoadFilter(); Reload(); };
            bar.Controls.Add(Lbl("Hợp đồng:"));
            bar.Controls.Add(cboHd);
            bar.Controls.Add(Lbl("Trạng thái:"));
            bar.Controls.Add(cboTrangThai);
            bar.Controls.Add(Lbl("Từ khóa:"));
            bar.Controls.Add(txtSearch);
            bar.Controls.Add(btnS);
            bar.Controls.Add(btnR);
            root.Controls.Add(bar, 0, 1);

            grid = new DataGridView();
            grid.Dock = DockStyle.Fill;
            Theme.StyleGrid(grid);
            grid.CellFormatting += GridCellFormatting;
            root.Controls.Add(grid, 0, 2);

            var bottom = new FlowLayoutPanel();
            bottom.AutoSize = true;
            bottom.WrapContents = false;
            bottom.Margin = new Padding(0, 8, 0, 0);
            dtPay = new DateTimePicker(); dtPay.Format = DateTimePickerFormat.Custom; dtPay.CustomFormat = "dd/MM/yyyy"; dtPay.Width = 130; dtPay.Margin = new Padding(0, 6, 12, 0);
            var btnPay = Theme.Btn("Xác nhận thanh toán", Theme.Success, 190);
            var btnUndo = Theme.Btn("Hủy thanh toán", Theme.Danger, 150);
            btnPay.Click += delegate { Pay(); };
            btnUndo.Click += delegate { Unpay(); };
            lblSummary = new Label(); lblSummary.AutoSize = true; lblSummary.Font = Theme.Bold; lblSummary.Margin = new Padding(16, 10, 0, 0);
            bottom.Controls.Add(Lbl("Ngày thanh toán:"));
            bottom.Controls.Add(dtPay);
            bottom.Controls.Add(btnPay);
            bottom.Controls.Add(btnUndo);
            bottom.Controls.Add(lblSummary);
            root.Controls.Add(bottom, 0, 3);

            Controls.Add(root);
        }

        private static Label Lbl(string t) { var l = new Label(); l.Text = t; l.AutoSize = true; l.Margin = new Padding(0, 10, 6, 0); return l; }

        private void LoadFilter()
        {
            DataTable dt = ContractBll.AllLookup();
            DataRow all = dt.NewRow();
            all["MaHD"] = 0;
            all["Ten"] = "(Tất cả hợp đồng)";
            dt.Rows.InsertAt(all, 0);
            cboHd.DisplayMember = "Ten";
            cboHd.ValueMember = "MaHD";
            cboHd.DataSource = dt;
            cboHd.SelectedIndex = 0;
        }

        private void Reload()
        {
            try
            {
                int? hd = null;
                if (cboHd.SelectedValue != null && Convert.ToInt32(cboHd.SelectedValue) > 0) hd = Convert.ToInt32(cboHd.SelectedValue);
                string st = cboTrangThai.SelectedIndex <= 0 ? null : cboTrangThai.SelectedItem.ToString();
                DataTable dt = ThanhToanBll.Search(txtSearch.Text.Trim(), hd, st);
                grid.DataSource = dt;
                Theme.FormatColumns(grid);
                var heads = new Dictionary<string, string> {
                    {"SoHD","Số HĐ"},{"TenKH","Khách hàng"},{"KyThu","Kỳ"},{"KyThanhToan","Kỳ thanh toán"},{"SoTien","Số tiền (đ)"},
                    {"HanThanhToan","Hạn thanh toán"},{"NgayThanhToan","Ngày thanh toán"},{"TrangThai","Trạng thái"} };
                foreach (DataGridViewColumn c in grid.Columns)
                {
                    string h;
                    if (heads.TryGetValue(c.DataPropertyName, out h)) c.HeaderText = h;
                    if (c.DataPropertyName == "MaTT" || c.DataPropertyName == "MaHD") c.Visible = false;
                }
                decimal paid = 0, unpaid = 0;
                foreach (DataRow r in dt.Rows)
                {
                    decimal v = Convert.ToDecimal(r["SoTien"]);
                    if (r["TrangThai"].ToString() == "Đã thanh toán") paid += v; else unpaid += v;
                }
                lblSummary.Text = "Đã thu: " + paid.ToString("N0") + " đ   |   Chưa thu: " + unpaid.ToString("N0") + " đ";
            }
            catch (Exception ex) { Theme.Error("Không tải được dữ liệu: " + Db.Friendly(ex)); }
        }

        private void GridCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var drv = grid.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (drv == null) return;
            bool unpaid = drv["TrangThai"].ToString() == "Chưa thanh toán";
            if (unpaid && Convert.ToDateTime(drv["HanThanhToan"]) <= DateTime.Today)
                e.CellStyle.ForeColor = Theme.Danger;          // quá hạn / đến hạn chưa thu
            else if (!unpaid)
                e.CellStyle.ForeColor = Theme.Success;
        }

        private int? SelectedId()
        {
            if (grid.CurrentRow == null) return null;
            var drv = grid.CurrentRow.DataBoundItem as DataRowView;
            return drv == null ? (int?)null : Convert.ToInt32(drv["MaTT"]);
        }

        private void Pay()
        {
            int? id = SelectedId();
            if (!id.HasValue) { Theme.Warn("Vui lòng chọn kỳ thanh toán."); return; }
            if (!Theme.Confirm("Xác nhận đã thu tiền kỳ này vào ngày " + dtPay.Value.ToString("dd/MM/yyyy") + "?")) return;
            try
            {
                ThanhToanBll.Pay(id.Value, dtPay.Value.Date);
                Theme.Info("Thanh toán thành công.");
                Reload();
            }
            catch (Exception ex) { Theme.Error("Thanh toán thất bại: " + Db.Friendly(ex)); }
        }

        private void Unpay()
        {
            int? id = SelectedId();
            if (!id.HasValue) { Theme.Warn("Vui lòng chọn kỳ thanh toán."); return; }
            if (!Theme.Confirm("Hủy ghi nhận thanh toán của kỳ này?")) return;
            try
            {
                ThanhToanBll.Unpay(id.Value);
                Theme.Info("Đã hủy thanh toán.");
                Reload();
            }
            catch (Exception ex) { Theme.Error("Hủy thất bại: " + Db.Friendly(ex)); }
        }
    }
}

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
    /// <summary>7. Gia hạn hợp đồng: kiểm tra thời hạn gia hạn và công nợ trước khi xác nhận.</summary>
    public class GiaHanForm : EmbeddedForm
    {
        private ComboBox cboHd;
        private NumericUpDown nudThang;
        private DateTimePicker dtLap;
        private TextBox txtGhiChu, txtSearch;
        private Label lblKH, lblPhong, lblBatDau, lblGoc, lblKetThuc, lblDaGH, lblToiDa, lblNo, lblKetQua;
        private DataGridView grid;
        private Button btnCheck, btnConfirm;
        private ExtensionInfo info;

        public GiaHanForm()
        {
            BuildUi();
            LoadContracts();
            LoadHistory();
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
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var title = new Label();
            title.Text = "GIA HẠN HỢP ĐỒNG";
            title.Font = Theme.Title;
            title.ForeColor = Theme.Primary;
            title.AutoSize = true;
            title.Margin = new Padding(0, 0, 0, 8);
            root.Controls.Add(title, 0, 0);

            var g = new GroupBox();
            g.Text = "Lập phiếu gia hạn";
            g.Dock = DockStyle.Fill;
            g.AutoSize = true;
            g.Font = Theme.Bold;
            g.ForeColor = Theme.Primary;
            var t = new TableLayoutPanel();
            t.Dock = DockStyle.Fill;
            t.AutoSize = true;
            t.ColumnCount = 4;
            t.Font = Theme.Base;
            t.ForeColor = Theme.TextColor;
            t.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            cboHd = new ComboBox(); cboHd.DropDownStyle = ComboBoxStyle.DropDownList; cboHd.Dock = DockStyle.Fill;
            cboHd.SelectionChangeCommitted += delegate { LoadInfo(); };
            nudThang = new NumericUpDown(); nudThang.Minimum = 1; nudThang.Maximum = 60; nudThang.Value = 1; nudThang.Dock = DockStyle.Fill;
            dtLap = new DateTimePicker(); dtLap.Format = DateTimePickerFormat.Custom; dtLap.CustomFormat = "dd/MM/yyyy"; dtLap.Dock = DockStyle.Fill;
            txtGhiChu = new TextBox(); txtGhiChu.Dock = DockStyle.Fill;
            lblKH = L(); lblPhong = L(); lblBatDau = L(); lblGoc = L(); lblKetThuc = L(); lblDaGH = L(); lblToiDa = L(); lblNo = L();
            lblKetQua = new Label(); lblKetQua.AutoSize = true; lblKetQua.MaximumSize = new Size(900, 0); lblKetQua.Font = Theme.Bold;

            AddPair(t, 0, "Hợp đồng *", cboHd, "Khách hàng", lblKH);
            AddPair(t, 1, "Các phòng thuê", lblPhong, "Ngày bắt đầu", lblBatDau);
            AddPair(t, 2, "Thời gian thuê ban đầu", lblGoc, "Ngày kết thúc hiện tại", lblKetThuc);
            AddPair(t, 3, "Đã gia hạn", lblDaGH, "Còn được gia hạn tối đa", lblToiDa);
            AddPair(t, 4, "Công nợ hiện tại", lblNo, "Ngày lập phiếu", dtLap);
            AddPair(t, 5, "Số tháng gia hạn *", nudThang, "Ghi chú", txtGhiChu);
            g.Controls.Add(t);
            root.Controls.Add(g, 0, 1);

            var panel = new FlowLayoutPanel();
            panel.AutoSize = true;
            panel.Dock = DockStyle.Fill;
            panel.FlowDirection = FlowDirection.TopDown;
            panel.WrapContents = false;
            panel.Margin = new Padding(0, 8, 0, 8);
            var btns = new FlowLayoutPanel();
            btns.AutoSize = true;
            btns.WrapContents = false;
            btnCheck = Theme.Btn("Kiểm tra điều kiện", Theme.Accent, 170);
            btnConfirm = Theme.Btn("Xác nhận gia hạn", Theme.Success, 170);
            var btnReset = Theme.Btn("Làm mới", Theme.Primary);
            btnCheck.Click += delegate { CheckConditions(true); };
            btnConfirm.Click += delegate { Confirm(); };
            btnReset.Click += delegate { LoadContracts(); LoadHistory(); lblKetQua.Text = ""; };
            btns.Controls.AddRange(new Control[] { btnCheck, btnConfirm, btnReset });
            panel.Controls.Add(btns);
            panel.Controls.Add(lblKetQua);
            root.Controls.Add(panel, 0, 2);

            var bottom = new TableLayoutPanel();
            bottom.Dock = DockStyle.Fill;
            bottom.ColumnCount = 1;
            bottom.RowCount = 2;
            bottom.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            bottom.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            var bar = new FlowLayoutPanel();
            bar.AutoSize = true;
            bar.WrapContents = false;
            txtSearch = new TextBox(); txtSearch.Width = 260; txtSearch.Margin = new Padding(0, 6, 8, 0);
            var btnS = Theme.Btn("Tìm kiếm", Theme.Accent);
            btnS.Click += delegate { LoadHistory(); };
            var lb = new Label(); lb.Text = "Lịch sử phiếu gia hạn:"; lb.AutoSize = true; lb.Margin = new Padding(0, 10, 8, 0); lb.Font = Theme.Bold;
            bar.Controls.AddRange(new Control[] { lb, txtSearch, btnS });
            grid = new DataGridView();
            grid.Dock = DockStyle.Fill;
            Theme.StyleGrid(grid);
            bottom.Controls.Add(bar, 0, 0);
            bottom.Controls.Add(grid, 0, 1);
            root.Controls.Add(bottom, 0, 3);

            Controls.Add(root);
        }

        private static Label L() { var l = new Label(); l.AutoSize = true; l.Margin = new Padding(0, 8, 8, 8); return l; }

        private static void AddPair(TableLayoutPanel t, int row, string l1, Control c1, string l2, Control c2)
        {
            while (t.RowStyles.Count <= row) t.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            t.RowCount = t.RowStyles.Count;
            var a = new Label(); a.Text = l1; a.AutoSize = true; a.Margin = new Padding(0, 8, 8, 8);
            var b = new Label(); b.Text = l2; b.AutoSize = true; b.Margin = new Padding(16, 8, 8, 8);
            c1.Margin = new Padding(0, 4, 8, 4);
            c2.Margin = new Padding(0, 4, 8, 4);
            t.Controls.Add(a, 0, row); t.Controls.Add(c1, 1, row);
            t.Controls.Add(b, 2, row); t.Controls.Add(c2, 3, row);
        }

        private void LoadContracts()
        {
            try
            {
                cboHd.DisplayMember = "Ten";
                cboHd.ValueMember = "MaHD";
                cboHd.DataSource = ContractBll.ActiveLookup();
                cboHd.SelectedIndex = -1;
                info = null;
                lblKH.Text = lblPhong.Text = lblBatDau.Text = lblGoc.Text = lblKetThuc.Text = lblDaGH.Text = lblToiDa.Text = lblNo.Text = "";
            }
            catch (Exception ex) { Theme.Error(Db.Friendly(ex)); }
        }

        private void LoadHistory()
        {
            try
            {
                grid.DataSource = GiaHanBll.Search(txtSearch.Text.Trim());
                Theme.FormatColumns(grid);
                var heads = new Dictionary<string, string> {
                    {"MaPG","ID"},{"SoPhieu","Số phiếu"},{"SoHD","Số HĐ"},{"TenKH","Khách hàng"},{"DanhSachPhong","Các phòng"},{"NgayLap","Ngày lập"},
                    {"SoThangGiaHan","Số tháng gia hạn"},{"NgayKetThucCu","Kết thúc cũ"},{"NgayKetThucMoi","Kết thúc mới"},{"NguoiLap","Người lập"},{"GhiChu","Ghi chú"} };
                foreach (DataGridViewColumn c in grid.Columns)
                {
                    string h;
                    if (heads.TryGetValue(c.DataPropertyName, out h)) c.HeaderText = h;
                }
                if (grid.Columns.Contains("MaPG")) grid.Columns["MaPG"].Visible = false;
            }
            catch (Exception ex) { Theme.Error(Db.Friendly(ex)); }
        }

        private bool LoadInfo()
        {
            if (cboHd.SelectedValue == null) { Theme.Warn("Vui lòng chọn hợp đồng cần gia hạn."); return false; }
            try
            {
                info = GiaHanBll.GetInfo(Convert.ToInt32(cboHd.SelectedValue));
                lblKH.Text = info.TenKH;
                lblPhong.Text = info.Phong;
                lblBatDau.Text = info.NgayBatDau.ToString("dd/MM/yyyy");
                lblGoc.Text = info.SoThangGoc + " tháng";
                lblKetThuc.Text = info.NgayKetThuc.ToString("dd/MM/yyyy");
                lblDaGH.Text = info.DaGiaHan + " tháng";
                lblToiDa.Text = info.ToiDaConLai + " tháng (1/2 thời gian thuê ban đầu = " + (info.SoThangGoc / 2) + " tháng)";
                lblNo.Text = info.CongNo.ToString("N0") + " đ";
                lblNo.ForeColor = info.CongNo > 0 ? Theme.Danger : Theme.Success;
                lblKetQua.Text = "";
                return true;
            }
            catch (Exception ex) { Theme.Error(Db.Friendly(ex)); return false; }
        }

        private bool CheckConditions(bool show)
        {
            if (!LoadInfo()) return false;
            List<string> problems = GiaHanBll.Validate(info, (int)nudThang.Value);
            if (problems.Count == 0)
            {
                lblKetQua.ForeColor = Theme.Success;
                lblKetQua.Text = "✔ Đủ điều kiện gia hạn " + nudThang.Value + " tháng. Ngày kết thúc mới: "
                    + ContractBll.EndDate(info.NgayBatDau, info.SoThangGoc + info.DaGiaHan + (int)nudThang.Value).ToString("dd/MM/yyyy");
                return true;
            }
            lblKetQua.ForeColor = Theme.Danger;
            lblKetQua.Text = "✘ Không đủ điều kiện gia hạn:\n- " + string.Join("\n- ", problems.ToArray());
            return false;
        }

        private void Confirm()
        {
            if (!CheckConditions(false)) return;
            if (!Theme.Confirm("Xác nhận lập phiếu gia hạn " + nudThang.Value + " tháng cho hợp đồng này?")) return;
            try
            {
                GiaHanBll.Create(Convert.ToInt32(cboHd.SelectedValue), (int)nudThang.Value, dtLap.Value.Date, txtGhiChu.Text.Trim());
                Theme.Info("Lập phiếu gia hạn thành công. Lịch thanh toán các tháng gia hạn đã được bổ sung.");
                LoadInfo();
                LoadHistory();
                txtGhiChu.Clear();
            }
            catch (Exception ex) { Theme.Error("Gia hạn thất bại:\n" + Db.Friendly(ex)); }
        }
    }
}

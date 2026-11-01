using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyVanPhong.BLL;
using QuanLyVanPhong.DAL;

namespace QuanLyVanPhong.Presentation
{
    /// <summary>Dashboard: thẻ thống kê + danh sách cần chú ý.</summary>
    public class DashboardForm : EmbeddedForm
    {
        public DashboardForm()
        {
            var root = new TableLayoutPanel();
            root.Dock = DockStyle.Fill;
            root.ColumnCount = 1;
            root.RowCount = 3;
            root.Padding = new Padding(16, 12, 16, 12);
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 120));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var title = new Label();
            title.Text = "TỔNG QUAN";
            title.Font = Theme.Title;
            title.ForeColor = Theme.Primary;
            title.AutoSize = true;
            title.Margin = new Padding(0, 0, 0, 8);
            root.Controls.Add(title, 0, 0);

            var cards = new TableLayoutPanel();
            cards.Dock = DockStyle.Fill;
            cards.ColumnCount = 5;
            cards.RowCount = 1;
            for (int i = 0; i < 5; i++) cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));

            var lists = new TableLayoutPanel();
            lists.Dock = DockStyle.Fill;
            lists.ColumnCount = 2;
            lists.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            lists.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            try
            {
                DataRow d = ReportBll.Dashboard();
                cards.Controls.Add(Card("Tổng số phòng", d["TongPhong"].ToString(), Theme.Primary), 0, 0);
                cards.Controls.Add(Card("Phòng đang thuê", d["DangThue"].ToString(), Theme.Accent), 1, 0);
                cards.Controls.Add(Card("Phòng trống", d["PhongTrong"].ToString(), Theme.Success), 2, 0);
                cards.Controls.Add(Card("Hợp đồng còn hiệu lực", d["HopDongHieuLuc"].ToString(), Theme.Warning), 3, 0);
                cards.Controls.Add(Card("Công nợ (đ)", Convert.ToDecimal(d["CongNo"]).ToString("N0"), Theme.Danger), 4, 0);

                lists.Controls.Add(MakeList("Hợp đồng sắp hết hạn (60 ngày tới)", ReportBll.Expiring(60),
                    "SoHD", "Số HĐ", "TenKH", "Khách hàng", "NgayKetThuc", "Kết thúc", "ConLaiNgay", "Còn (ngày)"), 0, 0);
                lists.Controls.Add(MakeList("Kỳ tiền thuê đến hạn / quá hạn chưa thu", ReportBll.DueRent(),
                    "SoHD", "Số HĐ", "TenKH", "Khách hàng", "KyThu", "Kỳ", "SoTien", "Số tiền", "HanThanhToan", "Hạn"), 1, 0);
            }
            catch (Exception ex)
            {
                Theme.Error("Không tải được dữ liệu tổng quan: " + Db.Friendly(ex));
            }
            root.Controls.Add(cards, 0, 1);
            root.Controls.Add(lists, 0, 2);
            Controls.Add(root);
        }

        private static Control Card(string caption, string value, Color color)
        {
            var p = new Panel();
            p.Dock = DockStyle.Fill;
            p.BackColor = color;
            p.Margin = new Padding(0, 0, 12, 0);
            var v = new Label();
            v.Text = value;
            v.ForeColor = Color.White;
            v.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            v.AutoSize = true;
            v.Location = new Point(16, 18);
            var c = new Label();
            c.Text = caption;
            c.ForeColor = Color.White;
            c.Font = Theme.Bold;
            c.AutoSize = true;
            c.Location = new Point(18, 80);
            p.Controls.Add(v);
            p.Controls.Add(c);
            return p;
        }

        private static Control MakeList(string title, DataTable data, params string[] heads)
        {
            var box = new GroupBox();
            box.Text = title;
            box.Dock = DockStyle.Fill;
            box.Font = Theme.Bold;
            box.ForeColor = Theme.Primary;
            box.Margin = new Padding(0, 12, 12, 0);
            var g = new DataGridView();
            g.Dock = DockStyle.Fill;
            Theme.StyleGrid(g);
            g.DataSource = data;
            Theme.FormatColumns(g);
            foreach (DataGridViewColumn c in g.Columns)
                for (int i = 0; i + 1 < heads.Length; i += 2)
                    if (c.DataPropertyName == heads[i]) c.HeaderText = heads[i + 1];
            box.Controls.Add(g);
            return box;
        }
    }
}

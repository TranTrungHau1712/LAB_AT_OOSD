using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using QuanLyVanPhong.BLL;
using QuanLyVanPhong.DAL;

namespace QuanLyVanPhong.Presentation
{
    /// <summary>15. Thống kê và báo cáo.</summary>
    public class ThongKeForm : EmbeddedForm
    {
        private NumericUpDown nudYear, nudDays;
        private Chart chart;
        private DataGridView gridRevenue, gridFloor, gridDebt, gridExpire;

        public ThongKeForm()
        {
            var tabs = new TabControl();
            tabs.Dock = DockStyle.Fill;
            tabs.Font = Theme.Bold;
            tabs.Padding = new Point(18, 6);
            tabs.TabPages.Add(BuildRevenuePage());
            tabs.TabPages.Add(BuildFloorPage());
            tabs.TabPages.Add(BuildDebtPage());
            tabs.TabPages.Add(BuildExpirePage());
            Controls.Add(tabs);
            LoadRevenue(); LoadFloor(); LoadDebt(); LoadExpire();
        }

        private static DataGridView NewGrid()
        {
            var g = new DataGridView();
            g.Dock = DockStyle.Fill;
            Theme.StyleGrid(g);
            return g;
        }

        private static Panel Bar(params Control[] cs)
        {
            var p = new FlowLayoutPanel();
            p.Dock = DockStyle.Top;
            p.AutoSize = true;
            p.Padding = new Padding(8);
            p.WrapContents = false;
            p.Controls.AddRange(cs);
            return p;
        }

        private static Label Lbl(string t) { var l = new Label(); l.Text = t; l.AutoSize = true; l.Margin = new Padding(0, 10, 8, 0); return l; }

        private TabPage BuildRevenuePage()
        {
            var page = new TabPage("Doanh thu theo tháng");
            page.BackColor = Theme.Back;
            nudYear = new NumericUpDown(); nudYear.Minimum = 2000; nudYear.Maximum = 2100; nudYear.Value = DateTime.Today.Year; nudYear.Width = 80; nudYear.Margin = new Padding(0, 6, 12, 0);
            var btn = Theme.Btn("Xem báo cáo", Theme.Accent, 130);
            btn.Click += delegate { LoadRevenue(); };

            chart = new Chart();
            chart.Dock = DockStyle.Fill;
            chart.BackColor = Color.White;
            var area = new ChartArea("main");
            area.AxisY.LabelStyle.Format = "#,##0";
            area.AxisX.Interval = 1;
            chart.ChartAreas.Add(area);
            chart.Legends.Add(new Legend("lg"));
            chart.Legends[0].Docking = Docking.Bottom;
            chart.Titles.Add("Doanh thu thực thu theo tháng (đồng)");

            gridRevenue = NewGrid();
            gridRevenue.Dock = DockStyle.Bottom;
            gridRevenue.Height = 200;

            page.Controls.Add(chart);
            page.Controls.Add(gridRevenue);
            page.Controls.Add(Bar(Lbl("Năm:"), nudYear, btn));
            return page;
        }

        private TabPage BuildFloorPage()
        {
            var page = new TabPage("Tình trạng phòng theo tầng");
            page.BackColor = Theme.Back;
            gridFloor = NewGrid();
            var btn = Theme.Btn("Làm mới", Theme.Primary);
            btn.Click += delegate { LoadFloor(); };
            page.Controls.Add(gridFloor);
            page.Controls.Add(Bar(btn));
            return page;
        }

        private TabPage BuildDebtPage()
        {
            var page = new TabPage("Công nợ theo hợp đồng");
            page.BackColor = Theme.Back;
            gridDebt = NewGrid();
            var btn = Theme.Btn("Làm mới", Theme.Primary);
            btn.Click += delegate { LoadDebt(); };
            page.Controls.Add(gridDebt);
            page.Controls.Add(Bar(btn));
            return page;
        }

        private TabPage BuildExpirePage()
        {
            var page = new TabPage("Hợp đồng sắp hết hạn");
            page.BackColor = Theme.Back;
            nudDays = new NumericUpDown(); nudDays.Minimum = 1; nudDays.Maximum = 365; nudDays.Value = 60; nudDays.Width = 70; nudDays.Margin = new Padding(0, 6, 12, 0);
            var btn = Theme.Btn("Xem", Theme.Accent);
            btn.Click += delegate { LoadExpire(); };
            gridExpire = NewGrid();
            page.Controls.Add(gridExpire);
            page.Controls.Add(Bar(Lbl("Trong vòng (ngày):"), nudDays, btn));
            return page;
        }

        private static void Heads(DataGridView g, params string[] pairs)
        {
            Theme.FormatColumns(g);
            foreach (DataGridViewColumn c in g.Columns)
                for (int i = 0; i + 1 < pairs.Length; i += 2)
                    if (c.DataPropertyName == pairs[i]) c.HeaderText = pairs[i + 1];
        }

        private void LoadRevenue()
        {
            try
            {
                DataTable dt = ReportBll.Revenue((int)nudYear.Value);
                var full = new DataTable();
                full.Columns.Add("Thang", typeof(int));
                full.Columns.Add("TienThue", typeof(decimal));
                full.Columns.Add("TienDichVu", typeof(decimal));
                full.Columns.Add("TienPhat", typeof(decimal));
                full.Columns.Add("TongThu", typeof(decimal));
                for (int m = 1; m <= 12; m++)
                {
                    DataRow[] f = dt.Select("Thang = " + m);
                    if (f.Length > 0) full.Rows.Add(m, f[0]["TienThue"], f[0]["TienDichVu"], f[0]["TienPhat"], f[0]["TongThu"]);
                    else full.Rows.Add(m, 0m, 0m, 0m, 0m);
                }
                gridRevenue.DataSource = full;
                Heads(gridRevenue, "Thang", "Tháng", "TienThue", "Tiền thuê", "TienDichVu", "Tiền dịch vụ", "TienPhat", "Tiền phạt", "TongThu", "Tổng thu");

                chart.Series.Clear();
                string[] names = { "Tiền thuê", "Tiền dịch vụ", "Tiền phạt" };
                string[] cols = { "TienThue", "TienDichVu", "TienPhat" };
                Color[] colors = { Theme.Primary, Theme.Accent, Theme.Warning };
                for (int s = 0; s < 3; s++)
                {
                    var ser = new Series(names[s]);
                    ser.ChartType = SeriesChartType.StackedColumn;
                    ser.ChartArea = "main";
                    ser.Legend = "lg";
                    ser.Color = colors[s];
                    foreach (DataRow r in full.Rows)
                        ser.Points.AddXY("T" + r["Thang"], Convert.ToDouble(r[cols[s]]));
                    chart.Series.Add(ser);
                }
            }
            catch (Exception ex) { Theme.Error(Db.Friendly(ex)); }
        }

        private void LoadFloor()
        {
            try
            {
                gridFloor.DataSource = ReportBll.RoomsByFloor();
                Heads(gridFloor, "TenTang", "Tầng", "TongPhong", "Tổng phòng", "DangThue", "Đang thuê", "PhongTrong", "Trống", "BaoTri", "Bảo trì", "TyLeLapDay", "Tỷ lệ lấp đầy (%)");
            }
            catch (Exception ex) { Theme.Error(Db.Friendly(ex)); }
        }

        private void LoadDebt()
        {
            try
            {
                gridDebt.DataSource = ReportBll.Debts();
                Heads(gridDebt, "SoHD", "Số HĐ", "TenKH", "Khách hàng", "TenCT", "Công ty", "TrangThai", "Trạng thái HĐ", "CongNo", "Công nợ (đ)");
            }
            catch (Exception ex) { Theme.Error(Db.Friendly(ex)); }
        }

        private void LoadExpire()
        {
            try
            {
                gridExpire.DataSource = ReportBll.Expiring((int)nudDays.Value);
                Heads(gridExpire, "SoHD", "Số HĐ", "TenKH", "Khách hàng", "NgayKetThuc", "Ngày kết thúc", "ConLaiNgay", "Còn lại (ngày)");
            }
            catch (Exception ex) { Theme.Error(Db.Friendly(ex)); }
        }
    }
}

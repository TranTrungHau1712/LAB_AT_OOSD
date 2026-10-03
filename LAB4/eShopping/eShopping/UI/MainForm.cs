using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using eShopping.Models;
using eShopping.Services;

namespace eShopping.UI
{
    /// <summary>Màn hình chính: chọn nhóm sản phẩm -> xem danh sách -> xem chi tiết / thêm vào giỏ.</summary>
    public class MainForm : Form
    {
        private readonly ListBox lstGroups = new ListBox { Dock = DockStyle.Fill, Font = Ui.BaseFont, BorderStyle = BorderStyle.None, IntegralHeight = false };
        private readonly DataGridView grid = Ui.Grid();
        private readonly Label lblUser = new Label { AutoSize = true, ForeColor = Color.White, Font = Ui.BaseFont, Margin = new Padding(3, 12, 10, 3) };
        private readonly NumericUpDown nudQty = new NumericUpDown { Minimum = 1, Maximum = 99, Value = 1, Width = 70, Font = Ui.BaseFont };
        private Button btnLogin, btnCart;

        public MainForm()
        {
            Ui.Setup(this, "e-SHOPPING - Cửa hàng ABC", 1050, 620);
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(900, 500);

            grid.Columns.Add("code", "Mã SP");
            grid.Columns.Add("name", "Tên sản phẩm");
            grid.Columns.Add("maker", "Nhà sản xuất");
            grid.Columns.Add("price", "Giá bán");
            grid.Columns.Add("stock", "Tình trạng");
            grid.Columns["code"].FillWeight = 40;
            grid.Columns["name"].FillWeight = 110;
            grid.Columns["maker"].FillWeight = 60;
            grid.Columns["price"].FillWeight = 60;
            grid.Columns["price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            grid.Columns["stock"].FillWeight = 45;
            grid.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) OnDetail(null, EventArgs.Empty); };

            // ----- thanh tiêu đề (trên) -----
            var top = new Panel { Dock = DockStyle.Top, Height = 62, BackColor = Ui.Primary };
            var title = new Label { Text = "e-SHOPPING", Font = Ui.TitleFont, ForeColor = Color.White, AutoSize = true, Location = new Point(15, 14) };
            var right = new FlowLayoutPanel { Dock = DockStyle.Right, AutoSize = true, FlowDirection = FlowDirection.LeftToRight, Padding = new Padding(0, 8, 8, 0), WrapContents = false };
            btnLogin = Ui.Btn("Đăng nhập", OnLoginLogout, 110);
            btnCart = Ui.Btn("Giỏ hàng (0)", OnCart, 130);
            right.Controls.Add(lblUser);
            right.Controls.Add(btnLogin);
            right.Controls.Add(btnCart);
            top.Controls.Add(title);
            top.Controls.Add(right);

            // ----- danh sách nhóm (trái) -----
            var left = new Panel { Dock = DockStyle.Left, Width = 230, BackColor = Color.WhiteSmoke, Padding = new Padding(0) };
            var lblGroups = new Label { Text = "NHÓM SẢN PHẨM", Dock = DockStyle.Top, Height = 34, Font = Ui.BoldFont, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(10, 0, 0, 0) };
            lstGroups.SelectedIndexChanged += (s, e) => LoadProducts();
            left.Controls.Add(lstGroups);
            left.Controls.Add(lblGroups);

            // ----- thanh thao tác (dưới) -----
            var bottom = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 54, Padding = new Padding(10, 8, 0, 0), BackColor = Color.WhiteSmoke };
            bottom.Controls.Add(Ui.Btn("Xem chi tiết", OnDetail, 130));
            bottom.Controls.Add(Ui.Lbl("Số lượng:"));
            bottom.Controls.Add(nudQty);
            bottom.Controls.Add(Ui.Btn("Thêm vào giỏ", OnAddToCart, 140, true));

            // Fill add trước, các cạnh add sau (top add cuối để chiếm toàn chiều ngang)
            Controls.Add(grid);
            Controls.Add(bottom);
            Controls.Add(left);
            Controls.Add(top);

            Load += (s, e) => { LoadGroups(); RefreshHeader(); };
            Activated += (s, e) => RefreshHeader();
        }

        private void LoadGroups()
        {
            try
            {
                lstGroups.DataSource = AppServices.Catalog.GetGroups();
                lstGroups.DisplayMember = "Name";
            }
            catch (Exception ex) { Ui.Error(this, "Không tải được nhóm sản phẩm: " + ex.Message); }
        }

        private void LoadProducts()
        {
            grid.Rows.Clear();
            var g = lstGroups.SelectedItem as ProductGroup;
            if (g == null) return;
            try
            {
                foreach (var p in AppServices.Catalog.GetProducts(g.Id))
                {
                    int i = grid.Rows.Add(p.Code, p.Name, p.Manufacturer, Ui.Money(p.Price), p.InStock ? "Còn hàng" : "Hết hàng");
                    grid.Rows[i].Tag = p;
                    if (!p.InStock) grid.Rows[i].DefaultCellStyle.ForeColor = Color.Gray;
                }
            }
            catch (Exception ex) { Ui.Error(this, "Không tải được sản phẩm: " + ex.Message); }
        }

        private Product Selected()
        {
            if (grid.CurrentRow == null) { Ui.Warn(this, "Vui lòng chọn một sản phẩm."); return null; }
            return grid.CurrentRow.Tag as Product;
        }

        private void OnDetail(object sender, EventArgs e)
        {
            var p = Selected();
            if (p == null) return;
            using (var f = new ProductDetailForm(p)) f.ShowDialog(this);
            RefreshHeader();
        }

        private void OnAddToCart(object sender, EventArgs e)
        {
            var p = Selected();
            if (p == null) return;
            if (!p.InStock) { Ui.Warn(this, "Sản phẩm đã hết hàng, không thể thêm vào giỏ."); return; }
            Session.Cart.Add(p, (int)nudQty.Value);
            RefreshHeader();
            Ui.Info(this, "Đã thêm \"" + p.Name + "\" vào giỏ hàng.");
        }

        private void OnCart(object sender, EventArgs e)
        {
            using (var f = new CartForm()) f.ShowDialog(this);
            RefreshHeader();
        }

        private void OnLoginLogout(object sender, EventArgs e)
        {
            if (Session.IsLoggedIn)
            {
                Session.CurrentCustomer = null;
            }
            else
            {
                using (var f = new LoginForm()) f.ShowDialog(this);
            }
            RefreshHeader();
        }

        private void RefreshHeader()
        {
            lblUser.Text = Session.IsLoggedIn ? "Xin chào, " + Session.CurrentCustomer.FullName : "Chưa đăng nhập";
            btnLogin.Text = Session.IsLoggedIn ? "Đăng xuất" : "Đăng nhập";
            btnCart.Text = "Giỏ hàng (" + Session.Cart.Count + ")";
        }
    }
}
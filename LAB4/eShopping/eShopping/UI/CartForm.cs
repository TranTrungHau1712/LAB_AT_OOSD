using System;
using System.Drawing;
using System.Windows.Forms;
using eShopping.Models;
using eShopping.Services;

namespace eShopping.UI
{
    /// <summary>Xem / cập nhật giỏ hàng, rồi chọn "Tính tiền".</summary>
    public class CartForm : Form
    {
        private readonly DataGridView grid = Ui.Grid();
        private readonly Label lblTotal = new Label { AutoSize = true, Font = new Font("Segoe UI", 12F, FontStyle.Bold), ForeColor = Ui.Primary, Margin = new Padding(20, 8, 0, 0) };

        public CartForm()
        {
            Ui.Setup(this, "Giỏ hàng", 760, 460);

            grid.ReadOnly = false;
            grid.Columns.Add("code", "Mã SP");
            grid.Columns.Add("name", "Tên sản phẩm");
            grid.Columns.Add("price", "Đơn giá");
            grid.Columns.Add("qty", "Số lượng (sửa trực tiếp)");
            grid.Columns.Add("sub", "Thành tiền");
            foreach (DataGridViewColumn c in grid.Columns) c.ReadOnly = c.Name != "qty";
            grid.Columns["price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            grid.Columns["sub"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            grid.Columns["qty"].DefaultCellStyle.BackColor = Color.LemonChiffon;
            grid.CellEndEdit += OnCellEndEdit;

            var bottom = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 56, Padding = new Padding(8, 8, 0, 0), BackColor = Color.WhiteSmoke };
            bottom.Controls.Add(Ui.Btn("Xóa sản phẩm", OnRemove, 130));
            bottom.Controls.Add(Ui.Btn("Tính tiền", OnCheckout, 130, true));
            bottom.Controls.Add(Ui.Btn("Đóng", (s, e) => Close(), 80));
            bottom.Controls.Add(lblTotal);

            Controls.Add(grid);
            Controls.Add(bottom);
            Load += (s, e) => Reload();
        }

        private void Reload()
        {
            grid.Rows.Clear();
            foreach (var it in Session.Cart.Items)
            {
                int i = grid.Rows.Add(it.Product.Code, it.Product.Name, Ui.Money(it.UnitPrice), it.Quantity, Ui.Money(it.SubTotal));
                grid.Rows[i].Tag = it;
            }
            lblTotal.Text = "Tổng tiền hàng: " + Ui.Money(Session.Cart.Total);
        }

        private void OnCellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || grid.Columns[e.ColumnIndex].Name != "qty") return;
            var it = grid.Rows[e.RowIndex].Tag as CartItem;
            if (it == null) return;

            int q;
            string text = Convert.ToString(grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
            if (!int.TryParse(text, out q) || q < 0 || q > 999)
                Ui.Warn(this, "Số lượng phải là số nguyên từ 1 đến 999 (nhập 0 để xóa khỏi giỏ).");
            else
                Session.Cart.UpdateQuantity(it.Product.Id, q);

            // không đổi dòng trực tiếp trong CellEndEdit -> hoãn lại
            BeginInvoke(new Action(Reload));
        }

        private void OnRemove(object sender, EventArgs e)
        {
            if (grid.CurrentRow == null) { Ui.Warn(this, "Vui lòng chọn sản phẩm cần xóa."); return; }
            var it = grid.CurrentRow.Tag as CartItem;
            if (it == null) return;
            Session.Cart.Remove(it.Product.Id);
            Reload();
        }

        private void OnCheckout(object sender, EventArgs e)
        {
            if (Session.Cart.IsEmpty) { Ui.Warn(this, "Giỏ hàng đang trống."); return; }

            if (!Session.IsLoggedIn)
            {
                Ui.Info(this, "Bạn cần đăng nhập (hoặc đăng ký tài khoản mới) để đặt hàng.");
                using (var login = new LoginForm())
                {
                    if (login.ShowDialog(this) != DialogResult.OK) return;
                }
            }

            using (var f = new CheckoutForm())
            {
                if (f.ShowDialog(this) == DialogResult.OK) Close();   // đặt hàng xong -> giỏ đã được xóa
            }
        }
    }
}
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using eShopping.Models;
using eShopping.Services;

namespace eShopping.UI
{
    public class ProductDetailForm : Form
    {
        private readonly Product _p;
        private readonly NumericUpDown nudQty = new NumericUpDown { Minimum = 1, Maximum = 99, Value = 1, Width = 70, Font = Ui.BaseFont };

        public ProductDetailForm(Product p)
        {
            _p = p;
            Ui.Setup(this, "Chi tiết sản phẩm", 640, 460);

            var pic = new PictureBox { Width = 200, Height = 200, SizeMode = PictureBoxSizeMode.Zoom, BorderStyle = BorderStyle.FixedSingle, Location = new Point(15, 15) };
            if (!string.IsNullOrEmpty(p.ImageUrl) && File.Exists(p.ImageUrl))
            {
                try { pic.Image = Image.FromFile(p.ImageUrl); } catch { }
            }
            var noImg = new Label { Text = pic.Image == null ? "(Chưa có hình)" : "", ForeColor = Color.Gray, AutoSize = true, Location = new Point(70, 105), BackColor = Color.Transparent };

            var info = Ui.Form2Col();
            info.Dock = DockStyle.None;
            info.Location = new Point(225, 5);
            info.Width = 400;
            Ui.AddRow(info, "Mã sản phẩm", Ui.Lbl(p.Code, true));
            Ui.AddRow(info, "Tên", Ui.Lbl(p.Name, true));
            Ui.AddRow(info, "Nhà sản xuất", Ui.Lbl(p.Manufacturer));
            Ui.AddRow(info, "Giá bán", Ui.Lbl(Ui.Money(p.Price), true));
            var stock = Ui.Lbl(p.InStock ? "Còn hàng" : "Hết hàng", true);
            stock.ForeColor = p.InStock ? Color.ForestGreen : Color.Firebrick;
            Ui.AddRow(info, "Tình trạng", stock);

            var txtDesc = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = Ui.BaseFont,
                Location = new Point(15, 225),
                Size = new Size(610, 140),
                BackColor = Color.White,
                Text = "MÔ TẢ:\r\n" + p.Description + "\r\n\r\nTHÔNG SỐ KỸ THUẬT:\r\n" + p.Specs
            };

            var bottom = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 52, Padding = new Padding(12, 8, 0, 0) };
            bottom.Controls.Add(Ui.Lbl("Số lượng:"));
            bottom.Controls.Add(nudQty);
            bottom.Controls.Add(Ui.Btn("Thêm vào giỏ hàng", OnAdd, 160, true));
            bottom.Controls.Add(Ui.Btn("Đóng", (s, e) => Close(), 80));

            Controls.Add(bottom);
            Controls.Add(pic);
            Controls.Add(noImg);
            Controls.Add(info);
            Controls.Add(txtDesc);
            noImg.BringToFront();
        }

        private void OnAdd(object sender, EventArgs e)
        {
            if (!_p.InStock) { Ui.Warn(this, "Sản phẩm đã hết hàng."); return; }
            Session.Cart.Add(_p, (int)nudQty.Value);
            Ui.Info(this, "Đã thêm vào giỏ hàng.");
            DialogResult = DialogResult.OK;
        }
    }
}
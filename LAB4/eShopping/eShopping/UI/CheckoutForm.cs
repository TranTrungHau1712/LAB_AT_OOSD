using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using eShopping.Models;
using eShopping.Services;

namespace eShopping.UI
{
    /// <summary>Đặt hàng: loại phiếu, khu vực giao, người nhận, thẻ tín dụng, xem phí và xác nhận.</summary>
    public class CheckoutForm : Form
    {
        private readonly RadioButton rbNormal = new RadioButton { Text = "Thường", Checked = true, AutoSize = true, Font = Ui.BaseFont };
        private readonly RadioButton rbExpress = new RadioButton { Text = "Chuyển phát nhanh (miễn phí từ 1.000.000 đ)", AutoSize = true, Font = Ui.BaseFont };
        private readonly RadioButton rbSameDay = new RadioButton { Text = "Chuyển phát nhanh trong ngày (miễn phí từ 5.000.000 đ)", AutoSize = true, Font = Ui.BaseFont };
        private readonly ComboBox cboZone = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 280, Font = Ui.BaseFont };

        private readonly TextBox txtRName = Ui.Txt(300), txtRAddr = Ui.Txt(300), txtRPhone = Ui.Txt(300);

        private readonly ComboBox cboCard = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 200, Font = Ui.BaseFont };
        private readonly TextBox txtCardNo = Ui.Txt(300), txtHolder = Ui.Txt(300), txtCsv = Ui.Txt(80);
        private readonly NumericUpDown nudMonth = new NumericUpDown { Minimum = 1, Maximum = 12, Value = 12, Width = 60, Font = Ui.BaseFont };
        private readonly NumericUpDown nudYear = new NumericUpDown { Minimum = 2000, Maximum = 2100, Value = DateTime.Today.Year + 1, Width = 80, Font = Ui.BaseFont };

        private readonly Label lblItems = Ui.Lbl("0 đ"), lblShip = Ui.Lbl("0 đ"), lblCardFee = Ui.Lbl("0 đ"), lblTotal = Ui.Lbl("0 đ", true);
        private readonly Label lblNote = new Label { AutoSize = true, ForeColor = Color.ForestGreen, Font = Ui.BaseFont };

        public CheckoutForm()
        {
            Ui.Setup(this, "Đặt hàng & thanh toán", 660, 800);
            AutoScroll = true;

            // dữ liệu cho combobox
            foreach (CardType t in Enum.GetValues(typeof(CardType))) cboCard.Items.Add(new CardItem(t));
            cboCard.SelectedIndex = 0;
            try
            {
                foreach (var z in AppServices.Orders.GetZones()) cboZone.Items.Add(z);
                if (cboZone.Items.Count > 0) cboZone.SelectedIndex = 0;
            }
            catch (Exception ex) { Ui.Error(this, "Không tải được khu vực giao hàng: " + ex.Message); }

            // ---- 1. Loại phiếu ----
            var gbType = new GroupBox { Text = "1. Loại phiếu đặt hàng", Dock = DockStyle.Top, Height = 120, Font = Ui.BoldFont };
            var flType = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(8, 4, 0, 0) };
            flType.Controls.AddRange(new Control[] { rbNormal, rbExpress, rbSameDay });
            gbType.Controls.Add(flType);

            // ---- 2. Khu vực ----
            var gbZone = new GroupBox { Text = "2. Khu vực giao hàng", Dock = DockStyle.Top, Height = 62, Font = Ui.BoldFont };
            cboZone.Location = new Point(14, 24);
            gbZone.Controls.Add(cboZone);

            // ---- 3. Người nhận ----
            var gbRecv = new GroupBox { Text = "3. Người nhận hàng (có thể khác người mua)", Dock = DockStyle.Top, Height = 205, Font = Ui.BoldFont };
            var tRecv = Ui.Form2Col();
            Ui.AddRow(tRecv, "Họ tên", txtRName);
            Ui.AddRow(tRecv, "Địa chỉ", txtRAddr);
            Ui.AddRow(tRecv, "Điện thoại", txtRPhone);
            var btnSelf = Ui.Btn("Dùng thông tin của tôi", OnUseMyInfo, 190);
            btnSelf.Font = Ui.BaseFont;
            tRecv.Controls.Add(btnSelf, 1, tRecv.RowCount);
            tRecv.RowCount += 1;
            gbRecv.Controls.Add(tRecv);

            // ---- 4. Thẻ ----
            var gbCard = new GroupBox { Text = "4. Thanh toán bằng thẻ tín dụng", Dock = DockStyle.Top, Height = 255, Font = Ui.BoldFont };
            var tCard = Ui.Form2Col();
            Ui.AddRow(tCard, "Loại thẻ", cboCard);
            Ui.AddRow(tCard, "Số thẻ", txtCardNo);
            var flExp = new FlowLayoutPanel { AutoSize = true, WrapContents = false };
            flExp.Controls.Add(nudMonth);
            flExp.Controls.Add(Ui.Lbl("/"));
            flExp.Controls.Add(nudYear);
            Ui.AddRow(tCard, "Hết hạn (tháng/năm)", flExp);
            Ui.AddRow(tCard, "Họ tên chủ thẻ", txtHolder);
            Ui.AddRow(tCard, "CSV", txtCsv);
            gbCard.Controls.Add(tCard);

            // ---- 5. Tổng kết ----
            var gbSum = new GroupBox { Text = "5. Chi phí", Dock = DockStyle.Top, Height = 175, Font = Ui.BoldFont };
            var tSum = Ui.Form2Col();
            Ui.AddRow(tSum, "Tiền hàng:", lblItems);
            Ui.AddRow(tSum, "Phí giao hàng:", lblShip);
            Ui.AddRow(tSum, "Lệ phí thẻ:", lblCardFee);
            Ui.AddRow(tSum, "TỔNG CỘNG:", lblTotal);
            gbSum.Controls.Add(tSum);
            lblNote.Location = new Point(300, 30);
            gbSum.Controls.Add(lblNote);

            var bottom = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 56, Padding = new Padding(8, 8, 0, 0) };
            bottom.Controls.Add(Ui.Btn("Xác nhận đặt hàng", OnConfirm, 180, true));
            bottom.Controls.Add(Ui.Btn("Quay lại", (s, e) => DialogResult = DialogResult.Cancel, 100));

            // add ngược thứ tự hiển thị (control add sau nằm trên)
            Controls.Add(bottom);
            Controls.Add(gbSum);
            Controls.Add(gbCard);
            Controls.Add(gbRecv);
            Controls.Add(gbZone);
            Controls.Add(gbType);

            // tính lại phí mỗi khi người dùng đổi lựa chọn
            rbNormal.CheckedChanged += (s, e) => Recalc();
            rbExpress.CheckedChanged += (s, e) => Recalc();
            rbSameDay.CheckedChanged += (s, e) => Recalc();
            cboZone.SelectedIndexChanged += (s, e) => Recalc();
            cboCard.SelectedIndexChanged += (s, e) => Recalc();
            Load += (s, e) => Recalc();
        }

        private sealed class CardItem
        {
            public CardType Type { get; }
            public CardItem(CardType t) { Type = t; }
            public override string ToString() => EnumText.Of(Type);
        }

        private OrderType SelectedType()
            => rbSameDay.Checked ? OrderType.SameDay : rbExpress.Checked ? OrderType.Express : OrderType.Normal;

        private CardType SelectedCard() => ((CardItem)cboCard.SelectedItem).Type;

        private void OnUseMyInfo(object sender, EventArgs e)
        {
            var c = Session.CurrentCustomer;
            if (c == null) return;
            txtRName.Text = c.FullName;
            txtRAddr.Text = c.Address;
            txtRPhone.Text = c.Phone;
        }

        private void Recalc()
        {
            try
            {
                var f = AppServices.Orders.CalculateFees(Session.Cart.Total, cboZone.SelectedItem as ShippingZone, SelectedType(), SelectedCard());
                lblItems.Text = Ui.Money(f.ItemsTotal);
                lblShip.Text = f.ShippingFree ? "Miễn phí" : Ui.Money(f.ShippingFee);
                lblCardFee.Text = Ui.Money(f.CardFee);
                lblTotal.Text = Ui.Money(f.Total);

                lblNote.Text = f.ShippingFree ? "Được miễn phí giao hàng" : "";
            }
            catch (Exception ex) { Ui.Error(this, "Lỗi tính phí: " + ex.Message); }
        }

        private void OnConfirm(object sender, EventArgs e)
        {
            var req = new OrderRequest
            {
                Customer = Session.CurrentCustomer,
                Cart = Session.Cart,
                OrderType = SelectedType(),
                Zone = cboZone.SelectedItem as ShippingZone,
                Receiver = new Receiver { FullName = txtRName.Text.Trim(), Address = txtRAddr.Text.Trim(), Phone = txtRPhone.Text.Trim() },
                Card = new CreditCardInfo
                {
                    Type = SelectedCard(),
                    Number = txtCardNo.Text.Replace(" ", ""),
                    ExpMonth = (int)nudMonth.Value,
                    ExpYear = (int)nudYear.Value,
                    Holder = txtHolder.Text.Trim(),
                    Csv = txtCsv.Text.Trim()
                }
            };

            Cursor = Cursors.WaitCursor;
            OrderResult res;
            try { res = AppServices.Orders.Checkout(req); }
            catch (Exception ex) { res = new OrderResult { Success = false, Message = "Lỗi hệ thống: " + ex.Message }; }
            finally { Cursor = Cursors.Default; }

            if (!res.Success) { Ui.Warn(this, res.Message); return; }

            string mail = !res.EmailAttempted ? "Khách chưa cung cấp email nên không gửi xác nhận."
                         : res.EmailSent ? "Đã gửi email xác nhận."
                         : "Gửi email xác nhận thất bại (đơn hàng vẫn hợp lệ).";
            Ui.Info(this, "Đặt hàng thành công!\nMã đơn: " + res.OrderId + "\nTổng thanh toán: " + Ui.Money(res.Total) + "\n" + mail);

            Session.Cart.Clear();
            DialogResult = DialogResult.OK;
        }
    }
}
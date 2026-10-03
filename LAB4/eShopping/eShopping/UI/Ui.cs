using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace eShopping.UI
{
    /// <summary>Bộ hàm dựng control bằng code để không phải kéo thả trong Designer.</summary>
    internal static class Ui
    {
        public static readonly CultureInfo Vn = new CultureInfo("vi-VN");
        public static readonly Font BaseFont = new Font("Segoe UI", 10F);
        public static readonly Font BoldFont = new Font("Segoe UI", 10F, FontStyle.Bold);
        public static readonly Font TitleFont = new Font("Segoe UI", 16F, FontStyle.Bold);
        public static readonly Color Primary = Color.FromArgb(106, 27, 154);   // tím cho hợp Visual Studio :)
        public static readonly Color PrimaryDark = Color.FromArgb(74, 20, 140);

        public static string Money(decimal v) => v.ToString("N0", Vn) + " đ";

        public static Label Lbl(string text, bool bold = false)
            => new Label
            {
                Text = text,
                AutoSize = true,
                Font = bold ? BoldFont : BaseFont,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(3, 6, 8, 3)
            };

        public static TextBox Txt(int width = 260, bool password = false)
            => new TextBox { Width = width, Font = BaseFont, UseSystemPasswordChar = password, Anchor = AnchorStyles.Left };

        public static Button Btn(string text, EventHandler click, int width = 130, bool primary = false)
        {
            var b = new Button
            {
                Text = text,
                Width = width,
                Height = 34,
                Font = BaseFont,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(4)
            };
            if (primary)
            {
                b.BackColor = Primary;
                b.ForeColor = Color.White;
                b.FlatAppearance.BorderColor = PrimaryDark;
            }
            else
            {
                b.BackColor = Color.White;
                b.FlatAppearance.BorderColor = Color.Gray;
            }
            if (click != null) b.Click += click;
            return b;
        }

        /// <summary>Bảng 2 cột (nhãn - ô nhập) tự co giãn theo nội dung.</summary>
        public static TableLayoutPanel Form2Col()
        {
            var t = new TableLayoutPanel
            {
                ColumnCount = 2,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Top,
                Padding = new Padding(10)
            };
            t.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            return t;
        }

        public static void AddRow(TableLayoutPanel t, string label, Control c)
        {
            int r = t.RowCount;
            t.RowCount = r + 1;
            t.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            t.Controls.Add(Lbl(label), 0, r);
            t.Controls.Add(c, 1, r);
        }

        public static DataGridView Grid()
        {
            var g = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = BaseFont,
                EnableHeadersVisualStyles = false
            };
            g.ColumnHeadersDefaultCellStyle.BackColor = Primary;
            g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            g.ColumnHeadersDefaultCellStyle.Font = BoldFont;
            g.ColumnHeadersHeight = 32;
            g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            return g;
        }

        public static void Setup(Form f, string title, int w, int h)
        {
            f.Text = title;
            f.Font = BaseFont;
            f.ClientSize = new Size(w, h);
            f.StartPosition = FormStartPosition.CenterParent;
            f.BackColor = Color.White;
        }

        public static void Info(IWin32Window o, string m) => MessageBox.Show(o, m, "e-Shopping", MessageBoxButtons.OK, MessageBoxIcon.Information);
        public static void Warn(IWin32Window o, string m) => MessageBox.Show(o, m, "e-Shopping", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        public static void Error(IWin32Window o, string m) => MessageBox.Show(o, m, "e-Shopping", MessageBoxButtons.OK, MessageBoxIcon.Error);
        public static bool Confirm(IWin32Window o, string m)
            => MessageBox.Show(o, m, "e-Shopping", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
    }
}
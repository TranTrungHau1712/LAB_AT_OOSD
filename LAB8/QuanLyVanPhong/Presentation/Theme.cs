using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyVanPhong.Presentation
{
    public static class Theme
    {
        public static readonly Color Primary = Color.FromArgb(31, 58, 95);
        public static readonly Color PrimaryDark = Color.FromArgb(21, 40, 68);
        public static readonly Color Accent = Color.FromArgb(46, 134, 222);
        public static readonly Color Success = Color.FromArgb(39, 174, 96);
        public static readonly Color Danger = Color.FromArgb(214, 69, 65);
        public static readonly Color Warning = Color.FromArgb(243, 156, 18);
        public static readonly Color Back = Color.FromArgb(244, 246, 250);
        public static readonly Color TextColor = Color.FromArgb(44, 62, 80);
        public static readonly Font Base = new Font("Segoe UI", 10F);
        public static readonly Font Bold = new Font("Segoe UI", 10F, FontStyle.Bold);
        public static readonly Font Title = new Font("Segoe UI", 16F, FontStyle.Bold);

        public static Button Btn(string text, Color color, int width = 100)
        {
            var b = new Button();
            b.Text = text;
            b.Width = width;
            b.Height = 34;
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.BackColor = color;
            b.ForeColor = Color.White;
            b.Font = Bold;
            b.Cursor = Cursors.Hand;
            b.Margin = new Padding(0, 0, 8, 0);
            return b;
        }

        public static void StyleGrid(DataGridView g)
        {
            g.ReadOnly = true;
            g.AllowUserToAddRows = false;
            g.AllowUserToDeleteRows = false;
            g.AllowUserToResizeRows = false;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.MultiSelect = false;
            g.RowHeadersVisible = false;
            g.BackgroundColor = Color.White;
            g.BorderStyle = BorderStyle.None;
            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersDefaultCellStyle.BackColor = Primary;
            g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            g.ColumnHeadersDefaultCellStyle.Font = Bold;
            g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            g.ColumnHeadersHeight = 36;
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 252);
            g.DefaultCellStyle.SelectionBackColor = Accent;
            g.DefaultCellStyle.SelectionForeColor = Color.White;
            g.DefaultCellStyle.Font = Base;
            g.RowTemplate.Height = 28;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            g.GridColor = Color.FromArgb(225, 230, 238);
        }

        public static void FormatColumns(DataGridView g)
        {
            foreach (DataGridViewColumn c in g.Columns)
            {
                if (c.ValueType == typeof(DateTime)) c.DefaultCellStyle.Format = "dd/MM/yyyy";
                else if (c.ValueType == typeof(decimal) || c.ValueType == typeof(double))
                {
                    c.DefaultCellStyle.Format = "#,##0.##";
                    c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }
        }

        public static void Info(string m) { MessageBox.Show(m, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        public static void Warn(string m) { MessageBox.Show(m, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        public static void Error(string m) { MessageBox.Show(m, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        public static bool Confirm(string m)
        {
            return MessageBox.Show(m, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }
    }

    /// <summary>Form nhúng vào vùng nội dung của MainForm.</summary>
    public class EmbeddedForm : Form
    {
        public EmbeddedForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            TopLevel = false;
            Dock = DockStyle.Fill;
            BackColor = Theme.Back;
            Font = Theme.Base;
        }
    }
}

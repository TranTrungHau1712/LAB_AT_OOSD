using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace NhanSu
{
    public static class UI
    {
        public static Button Btn(string text, EventHandler click, int width = 110)
        {
            var b = new Button { Text = text, Width = width, Height = 30, Margin = new Padding(4) };
            b.Click += click;
            return b;
        }

        public static Label Lbl(string text, bool bold = false)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, bold ? FontStyle.Bold : FontStyle.Regular),
                Margin = new Padding(0, 0, 0, 2)
            };
        }

        public static DataGridView Grid()
        {
            var g = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
                BackgroundColor = Color.White
            };
            g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(217, 226, 243);
            return g;
        }

        /// <summary>Đặt tiêu đề tiếng Việt và định dạng số, ngày cho các cột của lưới.</summary>
        public static void FormatColumns(DataGridView g)
        {
            foreach (DataGridViewColumn c in g.Columns)
            {
                c.HeaderText = Labels.Of(c.DataPropertyName);
                if (c.ValueType == typeof(DateTime)) c.DefaultCellStyle.Format = "dd/MM/yyyy";
                else if (c.ValueType == typeof(decimal)) c.DefaultCellStyle.Format = "N0";
            }
        }

        public static void Err(Exception ex)
        {
            MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void Warn(string msg)
        {
            MessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static void Info(string msg)
        {
            MessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static bool Confirm(string msg)
        {
            return MessageBox.Show(msg, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }
    }
}

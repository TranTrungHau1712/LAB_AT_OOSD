using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBenhAn.Services;

namespace QuanLyBenhAn.Forms
{
    /// <summary>Hàm dựng giao diện bằng code (không dùng Designer).</summary>
    internal static class Ui
    {
        public static readonly Font Normal = new Font("Segoe UI", 10f);
        public static readonly Font Bold = new Font("Segoe UI", 10f, FontStyle.Bold);

        public static Label Lbl(string text, bool bold = false)
        {
            return new Label { Text = text, AutoSize = true, Font = bold ? Bold : Normal, Margin = new Padding(3, 8, 3, 3), Anchor = AnchorStyles.Left };
        }

        public static TextBox Txt(int width = 220, bool multiline = false, int height = 0)
        {
            var t = new TextBox { Width = width, Font = Normal, Margin = new Padding(3, 4, 3, 4) };
            if (multiline) { t.Multiline = true; t.ScrollBars = ScrollBars.Vertical; t.Height = height > 0 ? height : 60; }
            return t;
        }

        public static Button Btn(string text, EventHandler onClick, int width = 120)
        {
            var b = new Button { Text = text, Width = width, Height = 32, Font = Normal, Margin = new Padding(3, 4, 3, 4), UseVisualStyleBackColor = true };
            if (onClick != null) b.Click += onClick;
            return b;
        }

        public static ComboBox ComboEmpty(string display, string value, int width = 220)
        {
            return new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = width, Font = Normal, DisplayMember = display, ValueMember = value, Margin = new Padding(3, 4, 3, 4) };
        }

        public static ComboBox Combo(DataTable dt, string display, string value, int width = 220)
        {
            var c = ComboEmpty(display, value, width);
            if (dt != null) c.DataSource = dt;
            return c;
        }

        public static ComboBox ComboList(string[] items, int width = 220)
        {
            var c = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = width, Font = Normal, Margin = new Padding(3, 4, 3, 4) };
            c.Items.AddRange(items);
            if (items.Length > 0) c.SelectedIndex = 0;
            return c;
        }

        public static DateTimePicker DateTimeBox(int width = 220)
        {
            return new DateTimePicker { Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm", Width = width, Font = Normal, Margin = new Padding(3, 4, 3, 4) };
        }

        public static DataGridView Grid(bool editable = false)
        {
            var g = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                Font = Normal,
                ReadOnly = !editable,
                AllowUserToAddRows = editable,
                AllowUserToDeleteRows = editable,
                AllowUserToResizeRows = false,
                MultiSelect = false,
                RowHeadersVisible = editable,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = editable ? DataGridViewSelectionMode.CellSelect : DataGridViewSelectionMode.FullRowSelect,
                EditMode = editable ? DataGridViewEditMode.EditOnEnter : DataGridViewEditMode.EditProgrammatically,
                BorderStyle = BorderStyle.FixedSingle,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
            };
            g.DataError += (s, e) => { e.ThrowException = false; };
            return g;
        }

        public static void Headers(DataGridView g, params string[] pairs)
        {
            for (int i = 0; i + 1 < pairs.Length; i += 2)
                if (g.Columns.Contains(pairs[i])) g.Columns[pairs[i]].HeaderText = pairs[i + 1];
        }

        public static void Format(DataGridView g, string col, string fmt)
        {
            if (g.Columns.Contains(col)) g.Columns[col].DefaultCellStyle.Format = fmt;
        }

        /// <summary>Bảng 2 cột nhãn - ô nhập: Fields("Họ tên", txtTen, "Địa chỉ", txtDiaChi...).</summary>
        public static TableLayoutPanel Fields(params object[] pairs)
        {
            var t = new TableLayoutPanel { AutoSize = true, ColumnCount = 2, Dock = DockStyle.Top, Padding = new Padding(4) };
            t.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            int r = 0;
            for (int i = 0; i + 1 < pairs.Length; i += 2)
            {
                t.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                t.Controls.Add(Lbl((string)pairs[i]), 0, r);
                var c = (Control)pairs[i + 1];
                c.Anchor = AnchorStyles.Left;
                t.Controls.Add(c, 1, r);
                r++;
            }
            t.RowCount = r;
            return t;
        }

        public static FlowLayoutPanel Row(params Control[] controls)
        {
            var f = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Fill, WrapContents = true, Padding = new Padding(2) };
            f.Controls.AddRange(controls);
            return f;
        }

        /// <summary>Khung chính: hàng trên (tự co), vùng giữa, tùy chọn cột phải.</summary>
        public static TableLayoutPanel Layout(Control top, Control center, Control right, int rightWidth)
        {
            int cols = right == null ? 1 : 2;
            var t = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = cols, RowCount = 2 };
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            if (right != null) t.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, rightWidth));
            t.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            t.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            top.Dock = DockStyle.Fill;
            t.Controls.Add(top, 0, 0);
            t.SetColumnSpan(top, cols);
            center.Dock = DockStyle.Fill;
            t.Controls.Add(center, 0, 1);
            if (right != null) { right.Dock = DockStyle.Fill; t.Controls.Add(right, 1, 1); }
            return t;
        }

        public static string NullIfEmpty(string s)
        {
            return string.IsNullOrWhiteSpace(s) ? null : s.Trim();
        }

        public static string Date(object v)
        {
            return v == null || v == DBNull.Value ? "" : ((DateTime)v).ToString("dd/MM/yyyy");
        }

        public static string Str(object v)
        {
            return v == null || v == DBNull.Value ? "" : v.ToString();
        }

        public static void Info(string msg) { MessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        public static void Warn(string msg) { MessageBox.Show(msg, "Chưa hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        public static bool Confirm(string msg)
        {
            return MessageBox.Show(msg, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        /// <summary>Chạy hành động và hiển thị lỗi nghiệp vụ / lỗi CSDL thân thiện.</summary>
        public static void Guard(Action action)
        {
            try { action(); }
            catch (BusinessException ex) { Warn(ex.Message); }
            catch (SqlException ex)
            {
                if (ex.Number == 547) Warn("Thao tác vi phạm ràng buộc dữ liệu (khóa ngoại hoặc điều kiện kiểm tra).\n" + ex.Message);
                else if (ex.Number == 2627 || ex.Number == 2601) Warn("Mã bị trùng, hãy dùng mã khác.\n" + ex.Message);
                else MessageBox.Show("Lỗi CSDL: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
    }
}

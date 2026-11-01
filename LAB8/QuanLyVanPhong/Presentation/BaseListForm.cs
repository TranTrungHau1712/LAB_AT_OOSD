using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using QuanLyVanPhong.DAL;

namespace QuanLyVanPhong.Presentation
{
    public enum FieldType { Text, Multiline, Integer, Decimal, Date, Combo, Check, Image }

    /// <summary>Mô tả một trường nhập liệu / một cột hiển thị của form quản lý.</summary>
    public class FieldDef
    {
        public string Name, Label;
        public FieldType Type;
        public bool Required, ReadOnlyOnEdit, ReadOnlyAlways, GridOnly, GridHidden, FullRow, AllowNull;
        public int MaxLength, Decimals;
        public decimal Min = 0, Max = 999999999999m;
        public DataTable ComboSource;
        public string ValueMember, DisplayMember;
        public string[] Items;

        public FieldDef(string name, string label, FieldType type = FieldType.Text)
        {
            Name = name; Label = label; Type = type;
        }
        public FieldDef Req() { Required = true; return this; }
        public FieldDef Lock() { ReadOnlyOnEdit = true; return this; }
        public FieldDef Hide() { GridHidden = true; return this; }
        public FieldDef Full() { FullRow = true; return this; }
        public FieldDef GridCol() { GridOnly = true; return this; }
        public FieldDef Len(int n) { MaxLength = n; return this; }
        public FieldDef Dec(int d) { Decimals = d; return this; }
        public FieldDef Range(decimal min, decimal max) { Min = min; Max = max; return this; }
        public FieldDef Lookup(DataTable dt, string value, string display, bool allowNull = false)
        {
            ComboSource = dt; ValueMember = value; DisplayMember = display; AllowNull = allowNull; return this;
        }
        public FieldDef Choices(params string[] items) { Items = items; return this; }
    }

    /// <summary>Ô chọn ảnh (lưu đường dẫn tương đối trong thư mục Images).</summary>
    public class ImagePicker : Panel
    {
        private readonly PictureBox pic = new PictureBox();
        private readonly Button btn = new Button();
        private string path;

        public ImagePicker()
        {
            Height = 110;
            pic.Size = new Size(96, 100);
            pic.Location = new Point(0, 4);
            pic.SizeMode = PictureBoxSizeMode.Zoom;
            pic.BorderStyle = BorderStyle.FixedSingle;
            pic.BackColor = Color.White;
            btn.Text = "Chọn ảnh...";
            btn.Size = new Size(110, 30);
            btn.Location = new Point(106, 4);
            btn.Click += delegate { Choose(); };
            Controls.Add(pic);
            Controls.Add(btn);
        }

        public string ImagePath
        {
            get { return path; }
            set
            {
                path = string.IsNullOrEmpty(value) ? null : value;
                if (pic.Image != null) { pic.Image.Dispose(); pic.Image = null; }
                if (path == null) return;
                string full = Path.IsPathRooted(path) ? path : Path.Combine(Application.StartupPath, path);
                if (!File.Exists(full)) return;
                try
                {
                    byte[] bytes = File.ReadAllBytes(full);
                    using (var ms = new MemoryStream(bytes)) pic.Image = new Bitmap(ms);
                }
                catch { }
            }
        }

        public void SetEnabled(bool on) { btn.Enabled = on; }

        private void Choose()
        {
            using (var d = new OpenFileDialog())
            {
                d.Filter = "Ảnh|*.jpg;*.jpeg;*.png;*.bmp";
                if (d.ShowDialog() != DialogResult.OK) return;
                string dir = Path.Combine(Application.StartupPath, "Images");
                Directory.CreateDirectory(dir);
                string name = Guid.NewGuid().ToString("N") + Path.GetExtension(d.FileName);
                File.Copy(d.FileName, Path.Combine(dir, name), true);
                ImagePath = Path.Combine("Images", name);
            }
        }
    }

    /// <summary>
    /// Form quản lý dùng chung: tìm kiếm + DataGridView + khung nhập liệu + nút Thêm/Sửa/Xóa/Lưu/Hủy.
    /// Giao diện dựng hoàn toàn bằng code (không dùng Designer).
    /// </summary>
    public abstract class BaseListForm : EmbeddedForm
    {
        protected readonly List<FieldDef> Fields = new List<FieldDef>();
        protected DataGridView Grid;
        protected TextBox TxtSearch;
        protected FlowLayoutPanel SearchBar, ButtonBar;
        protected Button BtnAdd, BtnEdit, BtnDelete, BtnSave, BtnCancel;

        private readonly Dictionary<string, Control> inputs = new Dictionary<string, Control>();
        private enum Mode { View, Add, Edit }
        private Mode mode = Mode.View;

        protected abstract string Title { get; }
        protected abstract string IdColumn { get; }
        protected abstract void DefineFields();
        protected abstract DataTable LoadData(string keyword);
        protected abstract void DoInsert(Dictionary<string, object> v);
        protected abstract void DoUpdate(object id, Dictionary<string, object> v);
        protected abstract void DoDelete(object id);
        protected virtual string DeleteMessage { get { return "Bạn có chắc chắn muốn xóa bản ghi đã chọn?"; } }
        protected virtual void OnNew() { }

        /// <summary>Subclass gọi ở cuối constructor.</summary>
        protected void Init()
        {
            Text = Title;
            DefineFields();
            BuildUi();
            Reload();
            ApplyMode(Mode.View);
        }

        protected DataRow CurrentRow
        {
            get
            {
                if (Grid == null || Grid.CurrentRow == null) return null;
                var v = Grid.CurrentRow.DataBoundItem as DataRowView;
                return v == null ? null : v.Row;
            }
        }

        protected Control Input(string name) { return inputs[name]; }

        protected void SetFieldValue(string name, object value)
        {
            SetValue(Fields.First(f => f.Name == name), value);
        }

        protected object GetFieldValue(string name)
        {
            return GetValue(Fields.First(f => f.Name == name));
        }

        protected Button AddExtraButton(string text, Color color, EventHandler handler, int width = 150)
        {
            var b = Theme.Btn(text, color, width);
            b.Click += handler;
            ButtonBar.Controls.Add(b);
            return b;
        }

        // ------------------------------------------------------------------ UI
        private void BuildUi()
        {
            var root = new TableLayoutPanel();
            root.Dock = DockStyle.Fill;
            root.ColumnCount = 1;
            root.RowCount = 5;
            root.Padding = new Padding(16, 12, 16, 12);
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var title = new Label();
            title.Text = Title.ToUpper();
            title.Font = Theme.Title;
            title.ForeColor = Theme.Primary;
            title.AutoSize = true;
            title.Margin = new Padding(0, 0, 0, 8);
            root.Controls.Add(title, 0, 0);

            SearchBar = new FlowLayoutPanel();
            SearchBar.AutoSize = true;
            SearchBar.Dock = DockStyle.Fill;
            SearchBar.WrapContents = false;
            SearchBar.Margin = new Padding(0, 0, 0, 8);
            TxtSearch = new TextBox();
            TxtSearch.Width = 280;
            TxtSearch.Margin = new Padding(0, 6, 8, 0);
            TxtSearch.KeyDown += delegate (object s, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; Reload(); }
            };
            var btnSearch = Theme.Btn("Tìm kiếm", Theme.Accent);
            btnSearch.Click += delegate { Reload(); };
            var btnRefresh = Theme.Btn("Làm mới", Theme.Primary);
            btnRefresh.Click += delegate { TxtSearch.Clear(); Reload(); };
            var lblKw = new Label();
            lblKw.Text = "Từ khóa:";
            lblKw.AutoSize = true;
            lblKw.Margin = new Padding(0, 10, 8, 0);
            SearchBar.Controls.Add(lblKw);
            SearchBar.Controls.Add(TxtSearch);
            SearchBar.Controls.Add(btnSearch);
            SearchBar.Controls.Add(btnRefresh);
            root.Controls.Add(SearchBar, 0, 1);

            Grid = new DataGridView();
            Grid.Dock = DockStyle.Fill;
            Theme.StyleGrid(Grid);
            Grid.SelectionChanged += delegate
            {
                if (mode == Mode.View) FillInputsFromCurrent();
                ApplyButtons();
            };
            root.Controls.Add(Grid, 0, 2);

            var group = new GroupBox();
            group.Text = "Thông tin chi tiết";
            group.Dock = DockStyle.Fill;
            group.AutoSize = true;
            group.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            group.Font = Theme.Bold;
            group.ForeColor = Theme.Primary;
            group.Padding = new Padding(10);

            var tbl = new TableLayoutPanel();
            tbl.Dock = DockStyle.Fill;
            tbl.AutoSize = true;
            tbl.ColumnCount = 4;
            tbl.Font = Theme.Base;
            tbl.ForeColor = Theme.TextColor;
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            int col = 0, row = 0;
            foreach (var f in Fields)
            {
                if (f.GridOnly) continue;
                Control input = CreateInput(f);
                inputs[f.Name] = input;
                var label = new Label();
                label.Text = f.Label + (f.Required ? " *" : "");
                label.AutoSize = true;
                label.Anchor = AnchorStyles.Left;
                label.Margin = new Padding(0, 8, 8, 8);
                bool full = f.FullRow || f.Type == FieldType.Multiline || f.Type == FieldType.Image;
                if (full && col != 0) { col = 0; row++; }
                while (tbl.RowStyles.Count <= row) tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                tbl.RowCount = tbl.RowStyles.Count;
                input.Dock = DockStyle.Fill;
                input.Margin = new Padding(0, 4, 16, 4);
                tbl.Controls.Add(label, col, row);
                tbl.Controls.Add(input, col + 1, row);
                if (full) { tbl.SetColumnSpan(input, 3); col = 0; row++; }
                else { col += 2; if (col >= 4) { col = 0; row++; } }
            }
            group.Controls.Add(tbl);
            root.Controls.Add(group, 0, 3);

            ButtonBar = new FlowLayoutPanel();
            ButtonBar.AutoSize = true;
            ButtonBar.Dock = DockStyle.Fill;
            ButtonBar.WrapContents = false;
            ButtonBar.Margin = new Padding(0, 8, 0, 0);
            BtnAdd = Theme.Btn("Thêm", Theme.Success);
            BtnEdit = Theme.Btn("Sửa", Theme.Accent);
            BtnDelete = Theme.Btn("Xóa", Theme.Danger);
            BtnSave = Theme.Btn("Lưu", Theme.Primary);
            BtnCancel = Theme.Btn("Hủy", Color.Gray);
            BtnAdd.Click += delegate { ApplyMode(Mode.Add); };
            BtnEdit.Click += delegate { if (CurrentRow != null) ApplyMode(Mode.Edit); };
            BtnDelete.Click += delegate { DeleteCurrent(); };
            BtnSave.Click += delegate { Save(); };
            BtnCancel.Click += delegate { ApplyMode(Mode.View); };
            ButtonBar.Controls.AddRange(new Control[] { BtnAdd, BtnEdit, BtnDelete, BtnSave, BtnCancel });
            root.Controls.Add(ButtonBar, 0, 4);

            Controls.Add(root);
        }

        private Control CreateInput(FieldDef f)
        {
            switch (f.Type)
            {
                case FieldType.Multiline:
                    var tm = new TextBox();
                    tm.Multiline = true;
                    tm.Height = 56;
                    tm.MinimumSize = new Size(0, 56);
                    tm.ScrollBars = ScrollBars.Vertical;
                    if (f.MaxLength > 0) tm.MaxLength = f.MaxLength;
                    return tm;
                case FieldType.Integer:
                    var ni = new NumericUpDown();
                    ni.Minimum = f.Min; ni.Maximum = f.Max; ni.DecimalPlaces = 0;
                    return ni;
                case FieldType.Decimal:
                    var nd = new NumericUpDown();
                    nd.Minimum = f.Min; nd.Maximum = f.Max; nd.DecimalPlaces = f.Decimals;
                    nd.ThousandsSeparator = true;
                    return nd;
                case FieldType.Date:
                    var dp = new DateTimePicker();
                    dp.Format = DateTimePickerFormat.Custom;
                    dp.CustomFormat = "dd/MM/yyyy";
                    return dp;
                case FieldType.Combo:
                    var cb = new ComboBox();
                    cb.DropDownStyle = ComboBoxStyle.DropDownList;
                    if (f.Items != null) cb.Items.AddRange(f.Items);
                    else if (f.ComboSource != null)
                    {
                        cb.DisplayMember = f.DisplayMember;
                        cb.ValueMember = f.ValueMember;
                        cb.DataSource = f.AllowNull ? WithBlank(f.ComboSource, f.ValueMember, f.DisplayMember) : f.ComboSource;
                    }
                    cb.SelectedIndex = -1;
                    return cb;
                case FieldType.Check:
                    var ck = new CheckBox();
                    ck.Text = "Có";
                    return ck;
                case FieldType.Image:
                    return new ImagePicker();
                default:
                    var tb = new TextBox();
                    if (f.MaxLength > 0) tb.MaxLength = f.MaxLength;
                    return tb;
            }
        }

        private static DataTable WithBlank(DataTable src, string valueMember, string displayMember)
        {
            DataTable t = src.Clone();
            DataRow r = t.NewRow();
            r[displayMember] = "(Không chọn)";
            t.Rows.Add(r);
            foreach (DataRow x in src.Rows) t.ImportRow(x);
            return t;
        }

        // --------------------------------------------------------- giá trị
        private object GetValue(FieldDef f)
        {
            Control c = inputs[f.Name];
            switch (f.Type)
            {
                case FieldType.Integer: return (int)((NumericUpDown)c).Value;
                case FieldType.Decimal: return ((NumericUpDown)c).Value;
                case FieldType.Date: return ((DateTimePicker)c).Value.Date;
                case FieldType.Check: return ((CheckBox)c).Checked;
                case FieldType.Image: return ((ImagePicker)c).ImagePath;
                case FieldType.Combo:
                    var cb = (ComboBox)c;
                    if (cb.SelectedIndex < 0) return null;
                    if (f.Items != null) return cb.SelectedItem.ToString();
                    object v = cb.SelectedValue;
                    return (v == null || v == DBNull.Value) ? null : v;
                default:
                    string s = c.Text.Trim();
                    return s.Length == 0 ? null : s;
            }
        }

        private void SetValue(FieldDef f, object v)
        {
            Control c = inputs[f.Name];
            bool isNull = v == null || v == DBNull.Value;
            switch (f.Type)
            {
                case FieldType.Integer:
                case FieldType.Decimal:
                    var n = (NumericUpDown)c;
                    decimal d = isNull ? n.Minimum : Convert.ToDecimal(v);
                    n.Value = Math.Min(n.Maximum, Math.Max(n.Minimum, d));
                    break;
                case FieldType.Date:
                    var dp = (DateTimePicker)c;
                    DateTime dt = isNull ? DateTime.Today : Convert.ToDateTime(v);
                    dp.Value = dt < dp.MinDate ? dp.MinDate : dt;
                    break;
                case FieldType.Check:
                    ((CheckBox)c).Checked = !isNull && Convert.ToBoolean(v);
                    break;
                case FieldType.Image:
                    ((ImagePicker)c).ImagePath = isNull ? null : v.ToString();
                    break;
                case FieldType.Combo:
                    var cb = (ComboBox)c;
                    if (isNull) { cb.SelectedIndex = (f.AllowNull && f.Items == null && cb.Items.Count > 0) ? 0 : -1; }
                    else if (f.Items != null) cb.SelectedItem = v.ToString();
                    else cb.SelectedValue = v;
                    break;
                default:
                    c.Text = isNull ? "" : v.ToString();
                    break;
            }
        }

        private void ClearInputs()
        {
            foreach (var f in Fields) if (!f.GridOnly) SetValue(f, null);
        }

        private void FillInputsFromCurrent()
        {
            DataRow r = CurrentRow;
            foreach (var f in Fields)
            {
                if (f.GridOnly) continue;
                SetValue(f, (r != null && r.Table.Columns.Contains(f.Name)) ? r[f.Name] : null);
            }
        }

        private Dictionary<string, object> ReadValues()
        {
            var d = new Dictionary<string, object>();
            foreach (var f in Fields) if (!f.GridOnly) d[f.Name] = GetValue(f);
            return d;
        }

        private string Validate()
        {
            foreach (var f in Fields)
            {
                if (f.GridOnly || !f.Required) continue;
                if (GetValue(f) == null)
                {
                    inputs[f.Name].Focus();
                    return "Vui lòng " + (f.Type == FieldType.Combo ? "chọn " : "nhập ") + f.Label + ".";
                }
            }
            return null;
        }

        // ---------------------------------------------------------- hành vi
        protected void Reload()
        {
            try
            {
                DataTable dt = LoadData(TxtSearch == null ? "" : TxtSearch.Text.Trim());
                Grid.DataSource = dt;
                FormatGrid();
            }
            catch (Exception ex)
            {
                Theme.Error("Không tải được dữ liệu: " + Db.Friendly(ex));
            }
            if (mode == Mode.View)
            {
                if (Grid.Rows.Count == 0) ClearInputs(); else FillInputsFromCurrent();
            }
            ApplyButtons();
        }

        private void FormatGrid()
        {
            Theme.FormatColumns(Grid);
            foreach (DataGridViewColumn c in Grid.Columns)
            {
                FieldDef f = Fields.FirstOrDefault(x => x.Name == c.DataPropertyName);
                if (f == null) continue;
                c.HeaderText = f.Label;
                if (f.GridHidden || f.Type == FieldType.Image) c.Visible = false;
            }
        }

        private void ApplyMode(Mode m)
        {
            mode = m;
            foreach (var f in Fields)
            {
                if (f.GridOnly) continue;
                bool on = !f.ReadOnlyAlways && ((m == Mode.Add) || (m == Mode.Edit && !f.ReadOnlyOnEdit));
                Control c = inputs[f.Name];
                var ip = c as ImagePicker;
                if (ip != null) ip.SetEnabled(on); else c.Enabled = on;
            }
            Grid.Enabled = m == Mode.View;
            if (m == Mode.Add) { ClearInputs(); OnNew(); }
            else if (m == Mode.View) { if (Grid.Rows.Count == 0) ClearInputs(); else FillInputsFromCurrent(); }
            ApplyButtons();
        }

        private void ApplyButtons()
        {
            if (BtnAdd == null) return;
            bool view = mode == Mode.View;
            bool has = CurrentRow != null && CurrentRow.Table.Columns.Contains(IdColumn);
            BtnAdd.Enabled = view;
            BtnEdit.Enabled = view && has;
            BtnDelete.Enabled = view && has;
            BtnSave.Enabled = !view;
            BtnCancel.Enabled = !view;
        }

        private void Save()
        {
            try
            {
                string err = Validate();
                if (err != null) { Theme.Warn(err); return; }
                Dictionary<string, object> v = ReadValues();
                bool adding = mode == Mode.Add;
                if (adding) DoInsert(v); else DoUpdate(CurrentRow[IdColumn], v);
                Theme.Info(adding ? "Thêm dữ liệu thành công." : "Cập nhật dữ liệu thành công.");
                ApplyMode(Mode.View);
                Reload();
            }
            catch (Exception ex)
            {
                Theme.Error("Lưu thất bại: " + Db.Friendly(ex));
            }
        }

        private void DeleteCurrent()
        {
            DataRow r = CurrentRow;
            if (r == null) return;
            if (!Theme.Confirm(DeleteMessage)) return;
            try
            {
                DoDelete(r[IdColumn]);
                Theme.Info("Xóa dữ liệu thành công.");
                Reload();
            }
            catch (Exception ex)
            {
                Theme.Error("Xóa thất bại: " + Db.Friendly(ex));
            }
        }
    }
}

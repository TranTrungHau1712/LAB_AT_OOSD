using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace NhanSu
{
    /// <summary>Màn hình CRUD dùng chung, toàn bộ giao diện dựng bằng code (không dùng Designer).</summary>
    public class EntityForm : Form
    {
        readonly EntityDef def;
        readonly DataGridView grid = UI.Grid();
        readonly FlowLayoutPanel fields = new FlowLayoutPanel();
        readonly TextBox txtSearch = new TextBox { Width = 220 };
        readonly Dictionary<string, Control> inputs = new Dictionary<string, Control>();
        readonly Dictionary<string, List<string>> tableCols = new Dictionary<string, List<string>>();
        DataTable schema;
        DataTable data;
        DataView view;

        public EntityForm(EntityDef d)
        {
            def = d;
            Text = d.Title;
            Size = new Size(1150, 720);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F);
            BuildLayout();
            try
            {
                string sql = BuildSelect();
                schema = Db.Schema(sql);
                BuildInputs();
                LoadData();
            }
            catch (Exception ex) { UI.Err(ex); }
        }

        // ---------- bố cục ----------
        void BuildLayout()
        {
            var bottom = new Panel { Dock = DockStyle.Bottom, Height = 250, Padding = new Padding(8), BorderStyle = BorderStyle.FixedSingle };
            fields.Dock = DockStyle.Fill;
            fields.AutoScroll = true;
            fields.FlowDirection = FlowDirection.LeftToRight;
            fields.WrapContents = true;
            bottom.Controls.Add(fields);

            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(6, 6, 0, 0) };
            top.Controls.Add(UI.Btn("Nhập mới", (s, e) => ClearInputs(), 90));
            top.Controls.Add(UI.Btn("Thêm", (s, e) => OnInsert(), 80));
            top.Controls.Add(UI.Btn("Sửa", (s, e) => OnUpdate(), 80));
            top.Controls.Add(UI.Btn("Xóa", (s, e) => OnDelete(), 80));
            top.Controls.Add(UI.Btn("Làm mới", (s, e) => LoadData(), 90));
            var lbl = UI.Lbl("Tìm kiếm:");
            lbl.Margin = new Padding(20, 8, 4, 0);
            top.Controls.Add(lbl);
            txtSearch.Margin = new Padding(0, 5, 0, 0);
            txtSearch.TextChanged += (s, e) => ApplyFilter();
            top.Controls.Add(txtSearch);

            // Control điền (Fill) phải được thêm đầu tiên
            Controls.Add(grid);
            Controls.Add(bottom);
            Controls.Add(top);
            grid.SelectionChanged += (s, e) => OnGridSelect();
        }

        List<string> Cols(string table)
        {
            List<string> l;
            if (tableCols.TryGetValue(table, out l)) return l;
            var dt = Db.Query("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME=@t ORDER BY ORDINAL_POSITION",
                Db.P("@t", table));
            l = dt.Rows.Cast<DataRow>().Select(r => r[0].ToString()).ToList();
            tableCols[table] = l;
            return l;
        }

        string BuildSelect()
        {
            var cols = new List<string>();
            for (int i = 0; i < def.Tables.Length; i++)
                foreach (var c in Cols(def.Tables[i]))
                {
                    if (i > 0 && def.Keys.Contains(c)) continue;
                    cols.Add("t" + i + "." + c);
                }
            string from = def.Tables[0] + " t0";
            for (int i = 1; i < def.Tables.Length; i++)
            {
                int n = i;
                from += " JOIN " + def.Tables[i] + " t" + i + " ON " +
                        string.Join(" AND ", def.Keys.Select(k => "t" + n + "." + k + "=t0." + k).ToArray());
            }
            string sql = "SELECT " + string.Join(", ", cols.ToArray()) + " FROM " + from;
            if (!string.IsNullOrEmpty(def.Where)) sql += " WHERE " + def.Where;
            sql += " ORDER BY " + string.Join(", ", def.Keys.Select(k => "t0." + k).ToArray());
            return sql;
        }

        // ---------- ô nhập ----------
        void BuildInputs()
        {
            foreach (DataColumn c in schema.Columns)
            {
                if (def.Hidden.Contains(c.ColumnName)) continue;
                var ctl = MakeInput(c);
                inputs[c.ColumnName] = ctl;
                var p = new Panel { Width = 250, Height = 54, Margin = new Padding(4) };
                var lb = UI.Lbl(Labels.Of(c.ColumnName) + (c.AllowDBNull ? "" : " *"));
                lb.Location = new Point(0, 0);
                ctl.Location = new Point(0, 20);
                ctl.Width = 240;
                p.Controls.Add(lb);
                p.Controls.Add(ctl);
                fields.Controls.Add(p);
            }
            ClearInputs();
        }

        Control MakeInput(DataColumn c)
        {
            string name = c.ColumnName;
            Lookup lk;
            string[] choices;
            if (def.Lookups.TryGetValue(name, out lk))
            {
                string where = string.IsNullOrEmpty(lk.Where) ? "" : " WHERE " + lk.Where;
                var lt = Db.Query("SELECT CAST(" + lk.Value + " AS NVARCHAR(50)) AS V, CAST(" + lk.Value +
                                  " AS NVARCHAR(50)) + N' - ' + ISNULL(CAST(" + lk.Display + " AS NVARCHAR(200)), N'') AS T FROM " +
                                  lk.Table + where + " ORDER BY 1");
                if (c.AllowDBNull)
                {
                    var nr = lt.NewRow();
                    nr["V"] = DBNull.Value;
                    nr["T"] = "";
                    lt.Rows.InsertAt(nr, 0);
                }
                return new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, ValueMember = "V", DisplayMember = "T", DataSource = lt };
            }
            if (def.Choices.TryGetValue(name, out choices))
            {
                var cb = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
                cb.Items.AddRange(choices);
                return cb;
            }
            if (c.DataType == typeof(DateTime))
                return new DateTimePicker { Format = DateTimePickerFormat.Short, ShowCheckBox = c.AllowDBNull };
            if (c.DataType == typeof(bool))
                return new CheckBox { Text = "Có" };
            var tb = new TextBox();
            if (c.DataType == typeof(string) && c.MaxLength > 200)
            {
                tb.Multiline = true;
                tb.Height = 60;
                tb.ScrollBars = ScrollBars.Vertical;
            }
            return tb;
        }

        object GetValue(DataColumn c)
        {
            Control ctl = inputs[c.ColumnName];
            var cb = ctl as ComboBox;
            if (cb != null)
            {
                object v = cb.DataSource != null ? cb.SelectedValue : cb.SelectedItem;
                return (v == null || v is DBNull) ? (object)DBNull.Value : v.ToString();
            }
            var dp = ctl as DateTimePicker;
            if (dp != null) return (dp.ShowCheckBox && !dp.Checked) ? (object)DBNull.Value : dp.Value.Date;
            var ck = ctl as CheckBox;
            if (ck != null) return ck.Checked;
            string s = ctl.Text.Trim();
            if (s.Length == 0) return DBNull.Value;
            try
            {
                if (c.DataType == typeof(int)) return int.Parse(s, CultureInfo.InvariantCulture);
                if (c.DataType == typeof(decimal)) return decimal.Parse(s.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture);
                if (c.DataType == typeof(TimeSpan)) return TimeSpan.Parse(s, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                throw new InvalidOperationException("Giá trị '" + s + "' của \"" + Labels.Of(c.ColumnName) + "\" không đúng định dạng.");
            }
            return s;
        }

        void SetValue(DataColumn c, object val)
        {
            Control ctl = inputs[c.ColumnName];
            bool isNull = val == null || val is DBNull;
            var cb = ctl as ComboBox;
            if (cb != null)
            {
                if (cb.DataSource != null)
                {
                    if (isNull) cb.SelectedIndex = c.AllowDBNull ? 0 : -1;
                    else cb.SelectedValue = val.ToString();
                }
                else cb.SelectedItem = isNull ? null : val.ToString();
                return;
            }
            var dp = ctl as DateTimePicker;
            if (dp != null)
            {
                if (isNull) { if (dp.ShowCheckBox) dp.Checked = false; }
                else { dp.Value = (DateTime)val; if (dp.ShowCheckBox) dp.Checked = true; }
                return;
            }
            var ck = ctl as CheckBox;
            if (ck != null) { ck.Checked = !isNull && (bool)val; return; }
            if (isNull) ctl.Text = "";
            else if (val is decimal) ctl.Text = ((decimal)val).ToString("0.##", CultureInfo.InvariantCulture);
            else ctl.Text = val.ToString();
        }

        void ClearInputs()
        {
            foreach (DataColumn c in schema.Columns)
            {
                if (!inputs.ContainsKey(c.ColumnName)) continue;
                Control ctl = inputs[c.ColumnName];
                var cb = ctl as ComboBox;
                if (cb != null) cb.SelectedIndex = (cb.DataSource == null && cb.Items.Count > 0) || (cb.DataSource != null && c.AllowDBNull) ? 0 : -1;
                else if (ctl is DateTimePicker)
                {
                    var dp = (DateTimePicker)ctl;
                    dp.Value = DateTime.Today;
                    if (dp.ShowCheckBox) dp.Checked = false;
                }
                else if (ctl is CheckBox) ((CheckBox)ctl).Checked = false;
                else ctl.Text = "";
                ctl.Enabled = true;
            }
            grid.ClearSelection();
        }

        // ---------- dữ liệu ----------
        void LoadData()
        {
            try
            {
                data = Db.Query(BuildSelect());
                view = new DataView(data);
                grid.DataSource = view;
                UI.FormatColumns(grid);
                foreach (var h in def.Hidden)
                    if (grid.Columns.Contains(h)) grid.Columns[h].Visible = false;
                ApplyFilter();
                ClearInputs();
            }
            catch (Exception ex) { UI.Err(ex); }
        }

        void ApplyFilter()
        {
            if (view == null) return;
            string t = txtSearch.Text.Trim().Replace("'", "''").Replace("[", "[[]").Replace("%", "[%]").Replace("*", "[*]");
            if (t.Length == 0) { view.RowFilter = ""; return; }
            var parts = data.Columns.Cast<DataColumn>()
                .Where(c => !def.Hidden.Contains(c.ColumnName))
                .Select(c => "Convert([" + c.ColumnName + "], 'System.String') LIKE '%" + t + "%'").ToArray();
            try { view.RowFilter = string.Join(" OR ", parts); }
            catch (Exception) { view.RowFilter = ""; }
        }

        void OnGridSelect()
        {
            if (grid.CurrentRow == null || !grid.CurrentRow.Selected || schema == null) return;
            var drv = grid.CurrentRow.DataBoundItem as DataRowView;
            if (drv == null) return;
            foreach (DataColumn c in schema.Columns)
            {
                if (!inputs.ContainsKey(c.ColumnName)) continue;
                SetValue(c, drv.Row[c.ColumnName]);
                if (def.Keys.Contains(c.ColumnName)) inputs[c.ColumnName].Enabled = false;
            }
        }

        Dictionary<string, object> Collect()
        {
            var vals = new Dictionary<string, object>();
            foreach (DataColumn c in schema.Columns)
            {
                if (!inputs.ContainsKey(c.ColumnName)) continue;
                object v = GetValue(c);
                if (v is DBNull && !c.AllowDBNull)
                {
                    inputs[c.ColumnName].Focus();
                    throw new InvalidOperationException("Vui lòng nhập: " + Labels.Of(c.ColumnName));
                }
                vals[c.ColumnName] = v;
            }
            foreach (var kv in def.Fixed) vals[kv.Key] = kv.Value;
            return vals;
        }

        void OnInsert()
        {
            try
            {
                if (def.Keys.Any(k => inputs.ContainsKey(k) && !inputs[k].Enabled))
                {
                    UI.Warn("Đang chọn một dòng có sẵn. Bấm \"Nhập mới\" trước khi thêm.");
                    return;
                }
                var v = Collect();
                Db.Tran((cn, tx) =>
                {
                    foreach (var t in def.Tables)
                    {
                        var cols = Cols(t).Where(c => v.ContainsKey(c)).ToList();
                        string sql = "INSERT INTO " + t + "(" + string.Join(",", cols.ToArray()) + ") VALUES(" +
                                     string.Join(",", cols.Select((c, i) => "@p" + i).ToArray()) + ")";
                        using (var cmd = new SqlCommand(sql, cn, tx))
                        {
                            for (int i = 0; i < cols.Count; i++) cmd.Parameters.AddWithValue("@p" + i, v[cols[i]]);
                            cmd.ExecuteNonQuery();
                        }
                    }
                });
                LoadData();
                ClearInputs();
            }
            catch (Exception ex) { UI.Err(ex); }
        }

        void OnUpdate()
        {
            try
            {
                if (grid.CurrentRow == null || !grid.CurrentRow.Selected) { UI.Warn("Chọn một dòng cần sửa."); return; }
                var v = Collect();
                Db.Tran((cn, tx) =>
                {
                    foreach (var t in def.Tables)
                    {
                        var set = Cols(t).Where(c => v.ContainsKey(c) && !def.Keys.Contains(c)).ToList();
                        if (set.Count == 0) continue;
                        string sql = "UPDATE " + t + " SET " + string.Join(",", set.Select((c, i) => c + "=@p" + i).ToArray()) +
                                     " WHERE " + string.Join(" AND ", def.Keys.Select(k => k + "=@k" + k).ToArray());
                        using (var cmd = new SqlCommand(sql, cn, tx))
                        {
                            for (int i = 0; i < set.Count; i++) cmd.Parameters.AddWithValue("@p" + i, v[set[i]]);
                            foreach (var k in def.Keys) cmd.Parameters.AddWithValue("@k" + k, v[k]);
                            cmd.ExecuteNonQuery();
                        }
                    }
                });
                LoadData();
            }
            catch (Exception ex) { UI.Err(ex); }
        }

        void OnDelete()
        {
            try
            {
                if (grid.CurrentRow == null || !grid.CurrentRow.Selected) { UI.Warn("Chọn một dòng cần xóa."); return; }
                if (!UI.Confirm("Xóa dòng đã chọn?")) return;
                var drv = (DataRowView)grid.CurrentRow.DataBoundItem;
                Db.Tran((cn, tx) =>
                {
                    foreach (var t in def.Tables.Reverse())
                    {
                        string sql = "DELETE FROM " + t + " WHERE " + string.Join(" AND ", def.Keys.Select(k => k + "=@k" + k).ToArray());
                        using (var cmd = new SqlCommand(sql, cn, tx))
                        {
                            foreach (var k in def.Keys) cmd.Parameters.AddWithValue("@k" + k, drv.Row[k]);
                            cmd.ExecuteNonQuery();
                        }
                    }
                });
                LoadData();
                ClearInputs();
            }
            catch (Exception ex) { UI.Err(ex); }
        }
    }
}

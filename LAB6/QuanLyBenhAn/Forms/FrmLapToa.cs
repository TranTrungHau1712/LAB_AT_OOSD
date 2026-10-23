using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBenhAn.Services;

namespace QuanLyBenhAn.Forms
{
    /// <summary>Bác sĩ lập (hoặc sửa) toa thuốc cho phiếu khám.</summary>
    public class FrmLapToa : Form
    {
        private readonly TextBox txtPK = Ui.Txt(120), txtKw = Ui.Txt(140), txtLieu = Ui.Txt(180), txtCach = Ui.Txt(220);
        private readonly TextBox txtLoiDan = Ui.Txt(500, true, 50);
        private readonly Label lblInfo = Ui.Lbl("(nhập mã phiếu khám rồi bấm Tìm phiếu)", true);
        private readonly Label lblTDP = Ui.Lbl("");
        private readonly ComboBox cboThuoc = Ui.ComboEmpty("ten", "maThuoc", 260);
        private readonly NumericUpDown numSL = new NumericUpDown { Minimum = 1, Maximum = 1000, Value = 1, Width = 70, Font = Ui.Normal };
        private readonly DataGridView grid = Ui.Grid(true);
        private bool binding;

        public FrmLapToa()
        {
            Text = "Lập toa thuốc";
            Size = new Size(1100, 700);
            BuildUi();
            cboThuoc.SelectedIndexChanged += (s, e) => { if (!binding) ShowTDP(); };
            Load += (s, e) => Ui.Guard(() => TimThuoc());
        }

        private void BuildUi()
        {
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "maThuoc", HeaderText = "Mã thuốc", ReadOnly = true, FillWeight = 15 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "tenThuoc", HeaderText = "Tên thuốc", ReadOnly = true, FillWeight = 35 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "soLuong", HeaderText = "Số lượng", FillWeight = 12 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "lieuDung", HeaderText = "Liều dùng", FillWeight = 25 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "cachDung", HeaderText = "Cách dùng", FillWeight = 30 });

            var top = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, AutoSize = true, WrapContents = false, Dock = DockStyle.Fill };
            top.Controls.Add(Ui.Row(Ui.Lbl("Mã phiếu khám *"), txtPK, Ui.Btn("Tìm phiếu", (s, e) => Ui.Guard(TimPhieu), 100), lblInfo));
            top.Controls.Add(Ui.Row(Ui.Lbl("Tìm thuốc"), txtKw, Ui.Btn("Tìm", (s, e) => Ui.Guard(() => TimThuoc()), 60), cboThuoc,
                Ui.Lbl("SL"), numSL, Ui.Lbl("Liều dùng *"), txtLieu, Ui.Lbl("Cách dùng"), txtCach));
            top.Controls.Add(Ui.Row(Ui.Btn("Thêm vào toa", (s, e) => Ui.Guard(ThemThuoc), 120), Ui.Btn("Bỏ dòng chọn", (s, e) => BoThuoc(), 120), lblTDP));
            txtKw.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) Ui.Guard(() => TimThuoc()); };

            var bottom = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Fill, WrapContents = true };
            bottom.Controls.Add(Ui.Lbl("Lời dặn"));
            bottom.Controls.Add(txtLoiDan);
            bottom.Controls.Add(Ui.Btn("Lưu toa thuốc", (s, e) => Ui.Guard(Luu), 140));

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.Controls.Add(top, 0, 0);
            root.Controls.Add(grid, 0, 1);
            root.Controls.Add(bottom, 0, 2);
            Controls.Add(root);
        }

        private void TimThuoc()
        {
            binding = true;
            cboThuoc.DataSource = DanhMucService.Thuoc(txtKw.Text);
            binding = false;
            ShowTDP();
        }

        private void ShowTDP()
        {
            var drv = cboThuoc.SelectedItem as DataRowView;
            if (drv == null) { lblTDP.Text = ""; return; }
            string tdp = Ui.Str(drv["tacDungPhu"]);
            lblTDP.Text = "Tác dụng phụ: " + (tdp == "" ? "(chưa ghi nhận)" : tdp);
        }

        private void TimPhieu()
        {
            var info = ToaThuocService.PhieuInfo(txtPK.Text);
            if (info == null) throw new BusinessException("Không tìm thấy phiếu khám " + txtPK.Text.Trim() + ".");
            string ma = Ui.Str(info["maPK"]);
            txtPK.Text = ma;
            lblInfo.Text = Ui.Str(info["hoTen"]) + " | " + ((DateTime)info["ngayGioKham"]).ToString("dd/MM/yyyy HH:mm") +
                           " | " + Ui.Str(info["loaiKham"]) + " | " + Ui.Str(info["chanDoan"]);
            grid.Rows.Clear();
            foreach (DataRow r in ToaThuocService.GetChiTiet(ma).Rows)
                grid.Rows.Add(r["maThuoc"], r["tenThuoc"], r["soLuong"], r["lieuDung"], Ui.Str(r["cachDung"]));
            txtLoiDan.Text = ToaThuocService.GetLoiDan(ma) ?? "";
        }

        private void ThemThuoc()
        {
            var drv = cboThuoc.SelectedItem as DataRowView;
            if (drv == null) throw new BusinessException("Hãy chọn thuốc.");
            if (string.IsNullOrWhiteSpace(txtLieu.Text)) throw new BusinessException("Hãy nhập liều dùng.");
            string ma = drv["maThuoc"].ToString();
            foreach (DataGridViewRow r in grid.Rows)
                if (Ui.Str(r.Cells["maThuoc"].Value) == ma) throw new BusinessException("Thuốc này đã có trong toa.");
            grid.Rows.Add(ma, drv["tenThuoc"].ToString(), (int)numSL.Value, txtLieu.Text.Trim(), txtCach.Text.Trim());
        }

        private void BoThuoc()
        {
            if (grid.CurrentRow != null) grid.Rows.Remove(grid.CurrentRow);
        }

        private void Luu()
        {
            if (string.IsNullOrWhiteSpace(txtPK.Text)) throw new BusinessException("Hãy nhập mã phiếu khám.");
            grid.EndEdit();
            var ds = new List<ChiTietToaInput>();
            foreach (DataGridViewRow r in grid.Rows)
            {
                int sl;
                if (!int.TryParse(Ui.Str(r.Cells["soLuong"].Value), out sl)) throw new BusinessException("Số lượng thuốc phải là số nguyên.");
                ds.Add(new ChiTietToaInput
                {
                    MaThuoc = Ui.Str(r.Cells["maThuoc"].Value),
                    SoLuong = sl,
                    LieuDung = Ui.Str(r.Cells["lieuDung"].Value),
                    CachDung = Ui.NullIfEmpty(Ui.Str(r.Cells["cachDung"].Value))
                });
            }
            string maToa = ToaThuocService.Save(txtPK.Text.Trim(), Ui.NullIfEmpty(txtLoiDan.Text), ds);
            Ui.Info("Đã lưu toa thuốc " + maToa + " (" + ds.Count + " loại thuốc).");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBenhAn.Services;

namespace QuanLyBenhAn.Forms
{
    /// <summary>Quản lý danh mục: khoa, bác sĩ, nhân viên CSKH, thuốc, xét nghiệm (nhập trực tiếp trên lưới).</summary>
    public class FrmDanhMuc : Form
    {
        private static readonly Dictionary<string, string> Ten = new Dictionary<string, string>
        {
            { "Khoa", "Khoa" }, { "BacSi", "Bác sĩ" }, { "NhanVienCSKH", "Nhân viên CSKH" }, { "Thuoc", "Thuốc" }, { "XetNghiem", "Xét nghiệm" }
        };

        public FrmDanhMuc()
        {
            Text = "Quản lý danh mục";
            Size = new Size(1100, 640);
            var tabs = new TabControl { Dock = DockStyle.Fill, Font = Ui.Normal };
            foreach (string tb in DanhMucService.Bang)
            {
                string table = tb;
                var page = new TabPage(Ten[table]);
                var grid = Ui.Grid(true);
                Action load = () => grid.DataSource = DanhMucService.Load(table);
                var top = Ui.Row(
                    Ui.Btn("Lưu thay đổi", (s, e) => Ui.Guard(() =>
                    {
                        grid.EndEdit();
                        DanhMucService.Save(table, (DataTable)grid.DataSource);
                        load();
                        Ui.Info("Đã lưu danh mục " + Ten[table] + ".");
                    }), 130),
                    Ui.Btn("Tải lại", (s, e) => Ui.Guard(() => load()), 90),
                    Ui.Lbl("Nhập dòng mới ở cuối lưới; chọn tiêu đề dòng rồi nhấn Delete để xóa."));
                page.Controls.Add(Ui.Layout(top, grid, null, 0));
                tabs.TabPages.Add(page);
                Load += (s, e) => Ui.Guard(() => load());
            }
            Controls.Add(tabs);
        }
    }
}

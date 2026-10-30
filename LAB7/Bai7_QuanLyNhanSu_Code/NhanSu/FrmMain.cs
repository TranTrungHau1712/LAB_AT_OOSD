using System;
using System.Drawing;
using System.Windows.Forms;

namespace NhanSu
{
    public class FrmMain : Form
    {
        public FrmMain()
        {
            Text = "Quản lý nhân sự - Công ty sơn Đông Á";
            WindowState = FormWindowState.Maximized;
            IsMdiContainer = true;
            Font = new Font("Segoe UI", 9F);

            var ms = new MenuStrip();
            ms.Items.Add(MakeMenu("Danh mục",
                Item("Phòng ban", () => Entities.PhongBan()),
                Item("Chức vụ", () => Entities.ChucVu()),
                Item("Nhân viên", () => Entities.NhanVien()),
                Item("Ca làm việc", () => Entities.CaLamViec())));
            ms.Items.Add(MakeMenu("Tuyển dụng - Hợp đồng",
                 FormItem("Thử việc và ký hợp đồng chính thức", () => new FrmThuViec()),
                Item("Quyết định tuyển dụng (thử việc)", () => Entities.QDTuyenDung()),
                new ToolStripSeparator(),
                Item("Hợp đồng dài hạn", () => Entities.HopDongDaiHan()),
                Item("Hợp đồng ngắn hạn", () => Entities.HopDongNganHan()),
                Item("Hồ sơ BHYT, BHXH", () => Entities.HoSoBaoHiem())));
            ms.Items.Add(MakeMenu("Quyết định",
                Item("Quyết định chức vụ, phụ cấp chức vụ", () => Entities.QDChucVu()),
                Item("Quyết định lương", () => Entities.QDLuong()),
                Item("Quyết định khen thưởng", () => Entities.QDKhenThuong()),
                Item("Quyết định kỷ luật", () => Entities.QDKyLuat()),
                new ToolStripSeparator(),
                 FormItem("Lập quyết định khen thưởng và ký duyệt", () => new FrmKhenThuong())));
            ms.Items.Add(MakeMenu("Chấm công - Phụ trội",
                Item("Kế hoạch phân công tuần", () => Entities.KeHoachPhanCong()),
                Item("Chi tiết phân công", () => Entities.ChiTietPhanCong()),
                new ToolStripSeparator(),
                Item("Bảng chấm công", () => Entities.BangChamCong()),
                Item("Chi tiết chấm công", () => Entities.ChiTietChamCong()),
                new ToolStripSeparator(),
                Item("Phiếu ghi nhận giờ phụ trội", () => Entities.PhieuPhuTroi()),
                Item("Chi tiết phiếu phụ trội", () => Entities.ChiTietPhieuPhuTroi())));
            ms.Items.Add(MakeMenu("Lương - Thu chi",
                 FormItem("Tính lương hằng tháng", () => new FrmTinhLuong()),
                new ToolStripSeparator(),
                Item("Phiếu chi khen thưởng", () => Entities.PhieuChiKhenThuong()),
                Item("Phiếu thu nộp phạt", () => Entities.PhieuThuNopPhat())));
            var thoat = new ToolStripMenuItem("Thoát");
            thoat.Click += (s, e) => Close();
            ms.Items.Add(thoat);

            MainMenuStrip = ms;
            Controls.Add(ms);
        }

        static ToolStripMenuItem MakeMenu(string text, params ToolStripItem[] items)
        {
            var m = new ToolStripMenuItem(text);
            m.DropDownItems.AddRange(items);
            return m;
        }

        ToolStripMenuItem Item(string text, Func<EntityDef> make)
        {
            return FormItem(text, () => new EntityForm(make()));
        }

        ToolStripMenuItem FormItem(string text, Func<System.Windows.Forms.Form> make)
        {
            var it = new ToolStripMenuItem(text);
            it.Click += (s, e) =>
            {
                var f = make();
                f.MdiParent = this;
                f.WindowState = FormWindowState.Maximized;
                f.Show();
            };
            return it;
        }
    }
}

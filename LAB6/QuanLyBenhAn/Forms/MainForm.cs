using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyBenhAn.Forms
{
    public class MainForm : Form
    {
        public MainForm()
        {
            Text = "Phần mềm quản lý hồ sơ bệnh án - Bệnh viện đa khoa quốc tế";
            IsMdiContainer = true;
            WindowState = FormWindowState.Maximized;
            Font = Ui.Normal;

            var menu = new MenuStrip { Font = Ui.Normal };
            menu.Items.Add(Group("Tiếp nhận",
                Item("Bệnh nhân - Đăng ký và cấp hồ sơ", (s, e) => Open<FrmBenhNhan>()),
                Item("Lịch hẹn khám", (s, e) => Open<FrmLichHen>())));
            menu.Items.Add(Group("Hồ sơ bệnh án",
                Item("Tra cứu hồ sơ và lịch sử khám", (s, e) => Open<FrmTraCuuHoSo>())));
            menu.Items.Add(Group("Khám bệnh",
                Item("Lập phiếu khám", (s, e) => Open<FrmPhieuKham>()),
                Item("Kết quả xét nghiệm", (s, e) => Open<FrmKetQuaXN>()),
                Item("Lập toa thuốc", (s, e) => Open<FrmLapToa>())));
            menu.Items.Add(Group("Thanh toán",
                Item("Viện phí", (s, e) => Open<FrmHoaDon>())));
            menu.Items.Add(Group("Danh mục",
                Item("Khoa, bác sĩ, nhân viên, thuốc, xét nghiệm", (s, e) => Open<FrmDanhMuc>())));
            menu.Items.Add(Group("Hệ thống",
                Item("Thoát", (s, e) => Close())));

            var status = new StatusStrip();
            status.Items.Add(new ToolStripStatusLabel("Chọn chức năng ở thanh menu phía trên."));
            Controls.Add(status);
            Controls.Add(menu);
            MainMenuStrip = menu;
        }

        private static ToolStripMenuItem Group(string text, params ToolStripItem[] items)
        {
            var g = new ToolStripMenuItem(text);
            g.DropDownItems.AddRange(items);
            return g;
        }

        private static ToolStripMenuItem Item(string text, EventHandler h)
        {
            var i = new ToolStripMenuItem(text);
            i.Click += h;
            return i;
        }

        private void Open<T>() where T : Form, new()
        {
            foreach (Form f in MdiChildren)
                if (f is T) { f.Activate(); return; }
            var frm = new T { MdiParent = this };
            frm.Show();
        }
    }
}

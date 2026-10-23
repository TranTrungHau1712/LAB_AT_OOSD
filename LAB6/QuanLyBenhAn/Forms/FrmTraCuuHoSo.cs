using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using QuanLyBenhAn.Services;

namespace QuanLyBenhAn.Forms
{
    /// <summary>Tra cứu hồ sơ bệnh án theo mã hồ sơ / thông tin bệnh nhân, xem lịch sử khám.</summary>
    public class FrmTraCuuHoSo : Form
    {
        private readonly TextBox txtSearch = Ui.Txt(240);
        private readonly DataGridView gridHS = Ui.Grid(), gridPK = Ui.Grid();
        private readonly TextBox txtCover = new TextBox { Multiline = true, ReadOnly = true, Dock = DockStyle.Fill, Font = Ui.Normal, ScrollBars = ScrollBars.Vertical };
        private readonly TextBox txtDetail = new TextBox { Multiline = true, ReadOnly = true, Dock = DockStyle.Fill, Font = Ui.Normal, ScrollBars = ScrollBars.Vertical };
        private readonly PictureBox pic = new PictureBox { Width = 105, Height = 140, SizeMode = PictureBoxSizeMode.Zoom, BorderStyle = BorderStyle.FixedSingle, Dock = DockStyle.Top };
        private bool loading;

        public FrmTraCuuHoSo()
        {
            Text = "Tra cứu hồ sơ bệnh án";
            Size = new Size(1250, 740);
            BuildUi();
            gridHS.SelectionChanged += (s, e) => { if (!loading) Ui.Guard(ShowHoSo); };
            gridPK.SelectionChanged += (s, e) => { if (!loading) Ui.Guard(ShowDetail); };
            Load += (s, e) => Ui.Guard(Search);
        }

        private void BuildUi()
        {
            var top = Ui.Row(Ui.Lbl("Mã hồ sơ / mã BN / tên / SĐT:"), txtSearch, Ui.Btn("Tra cứu", (s, e) => Ui.Guard(Search), 90));
            txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) Ui.Guard(Search); };

            var cover = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
            cover.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
            cover.RowStyles.Add(new RowStyle(SizeType.Absolute, 146));
            cover.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            cover.Controls.Add(Ui.Lbl("TRANG BÌA HỒ SƠ", true), 0, 0);
            cover.Controls.Add(pic, 0, 1);
            cover.Controls.Add(txtCover, 0, 2);

            var lower = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1 };
            lower.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 280));
            lower.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
            lower.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
            lower.Controls.Add(cover, 0, 0);
            lower.Controls.Add(gridPK, 1, 0);
            lower.Controls.Add(txtDetail, 2, 0);

            var center = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
            center.RowStyles.Add(new RowStyle(SizeType.Absolute, 190));
            center.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            center.Controls.Add(gridHS, 0, 0);
            center.Controls.Add(lower, 0, 1);

            Controls.Add(Ui.Layout(top, center, null, 0));
        }

        private void Search()
        {
            loading = true;
            gridHS.DataSource = HoSoService.Search(txtSearch.Text);
            Ui.Headers(gridHS, "maHS", "Mã hồ sơ", "maBN", "Mã BN", "hoTen", "Họ tên", "ngaySinh", "Ngày sinh", "dienThoai", "Điện thoại",
                "diaChi", "Địa chỉ", "ngayLap", "Ngày lập", "ngayHetHan", "Hết hạn", "trangThai", "Trạng thái");
            Ui.Format(gridHS, "ngaySinh", "dd/MM/yyyy"); Ui.Format(gridHS, "ngayLap", "dd/MM/yyyy"); Ui.Format(gridHS, "ngayHetHan", "dd/MM/yyyy");
            loading = false;
            ShowHoSo();
        }

        private void ShowHoSo()
        {
            var r = gridHS.CurrentRow;
            loading = true;
            if (pic.Image != null) { pic.Image.Dispose(); pic.Image = null; }
            if (r == null)
            {
                txtCover.Clear(); gridPK.DataSource = null; txtDetail.Clear();
                loading = false;
                return;
            }
            string maHS = Ui.Str(r.Cells["maHS"].Value);
            var sb = new StringBuilder();
            sb.AppendLine("Mã hồ sơ: " + maHS);
            sb.AppendLine("Mã BN: " + Ui.Str(r.Cells["maBN"].Value));
            sb.AppendLine("Họ tên: " + Ui.Str(r.Cells["hoTen"].Value));
            sb.AppendLine("Ngày sinh: " + Ui.Date(r.Cells["ngaySinh"].Value));
            sb.AppendLine("Điện thoại: " + Ui.Str(r.Cells["dienThoai"].Value));
            sb.AppendLine("Địa chỉ: " + Ui.Str(r.Cells["diaChi"].Value));
            sb.AppendLine("Ngày lập: " + Ui.Date(r.Cells["ngayLap"].Value));
            sb.AppendLine("Hết hạn: " + Ui.Date(r.Cells["ngayHetHan"].Value));
            sb.AppendLine("Trạng thái: " + Ui.Str(r.Cells["trangThai"].Value));
            txtCover.Text = sb.ToString();

            byte[] anh = BenhNhanService.GetPhoto(Ui.Str(r.Cells["maBN"].Value));
            if (anh != null && anh.Length > 0)
            {
                try { pic.Image = Image.FromStream(new MemoryStream(anh)); } catch { pic.Image = null; }
            }

            gridPK.DataSource = HoSoService.Visits(maHS);
            Ui.Headers(gridPK, "maPK", "Mã PK", "ngayGioKham", "Ngày giờ khám", "loaiKham", "Loại", "tenBacSi", "Bác sĩ",
                "tenKhoa", "Khoa", "chanDoan", "Chẩn đoán", "ketLuan", "Kết luận");
            Ui.Format(gridPK, "ngayGioKham", "dd/MM/yyyy HH:mm");
            loading = false;
            ShowDetail();
        }

        private void ShowDetail()
        {
            var r = gridPK.CurrentRow;
            txtDetail.Text = r == null ? "Chọn một phiếu khám để xem triệu chứng, xét nghiệm và toa thuốc."
                                       : HoSoService.VisitDetail(Ui.Str(r.Cells["maPK"].Value));
        }
    }
}

using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBenhAn.Services;

namespace QuanLyBenhAn.Forms
{
    /// <summary>Nhập / cập nhật kết quả xét nghiệm của một phiếu khám.</summary>
    public class FrmKetQuaXN : Form
    {
        private readonly TextBox txtPK = Ui.Txt(120);
        private readonly Label lblInfo = Ui.Lbl("(nhập mã phiếu khám rồi bấm Tải)", true);
        private readonly DataGridView grid = Ui.Grid(true);

        public FrmKetQuaXN()
        {
            Text = "Kết quả xét nghiệm";
            Size = new Size(900, 520);
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            var top = Ui.Row(Ui.Lbl("Mã phiếu khám:"), txtPK, Ui.Btn("Tải", (s, e) => Ui.Guard(Tai), 70),
                Ui.Btn("Lưu kết quả", (s, e) => Ui.Guard(Luu), 110), lblInfo);
            Controls.Add(Ui.Layout(top, grid, null, 0));
        }

        private void Tai()
        {
            var info = ToaThuocService.PhieuInfo(txtPK.Text);
            if (info == null) throw new BusinessException("Không tìm thấy phiếu khám " + txtPK.Text.Trim() + ".");
            lblInfo.Text = Ui.Str(info["hoTen"]) + " | khám " + ((DateTime)info["ngayGioKham"]).ToString("dd/MM/yyyy HH:mm");
            grid.DataSource = PhieuKhamService.LoadXetNghiem(txtPK.Text);
            Ui.Headers(grid, "maXN", "Mã XN", "tenXN", "Xét nghiệm", "ngayXN", "Ngày thực hiện", "ketQua", "Kết quả", "ghiChu", "Ghi chú");
            Ui.Format(grid, "ngayXN", "dd/MM/yyyy HH:mm");
            foreach (string c in new[] { "maXN", "tenXN", "ngayXN" }) grid.Columns[c].ReadOnly = true;
            if (grid.Rows.Count == 0) Ui.Info("Phiếu khám này không có chỉ định xét nghiệm.");
        }

        private void Luu()
        {
            var dt = grid.DataSource as DataTable;
            if (dt == null || dt.Rows.Count == 0) throw new BusinessException("Chưa có dữ liệu xét nghiệm để lưu.");
            grid.EndEdit();
            PhieuKhamService.SaveKetQua(txtPK.Text.Trim(), dt);
            Tai();
            Ui.Info("Đã lưu kết quả xét nghiệm.");
        }
    }
}

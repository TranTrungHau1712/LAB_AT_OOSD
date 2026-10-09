namespace QuanLyCongTyDuLich.Forms
{
    partial class FrmMain
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTieuDe = new System.Windows.Forms.Label();
            this.btnDanhMuc = new System.Windows.Forms.Button();
            this.btnTour = new System.Windows.Forms.Button();
            this.btnChuyenLe = new System.Windows.Forms.Button();
            this.btnDangKyLe = new System.Windows.Forms.Button();
            this.btnDangKyDoan = new System.Windows.Forms.Button();
            this.btnPhanCong = new System.Windows.Forms.Button();
            this.btnKetThuc = new System.Windows.Forms.Button();
            this.btnThongKe = new System.Windows.Forms.Button();
            this.btnThoat = new System.Windows.Forms.Button();
            this.SuspendLayout();
            this.lblTieuDe.Name = "lblTieuDe";
            this.lblTieuDe.Location = new System.Drawing.Point(105, 20);
            this.lblTieuDe.Text = "CÔNG TY DU LỊCH VĂN HÓA VIỆT";
            this.lblTieuDe.AutoSize = true;
            this.lblTieuDe.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.lblTieuDe.ForeColor = System.Drawing.Color.FromArgb(0, 51, 102);
            this.Controls.Add(this.lblTieuDe);
            this.btnDanhMuc.Name = "btnDanhMuc";
            this.btnDanhMuc.Location = new System.Drawing.Point(40, 75);
            this.btnDanhMuc.Size = new System.Drawing.Size(210, 45);
            this.btnDanhMuc.Text = "Danh mục";
            this.btnDanhMuc.UseVisualStyleBackColor = true;
            this.Controls.Add(this.btnDanhMuc);
            this.btnTour.Name = "btnTour";
            this.btnTour.Location = new System.Drawing.Point(270, 75);
            this.btnTour.Size = new System.Drawing.Size(210, 45);
            this.btnTour.Text = "Tour - hành trình";
            this.btnTour.UseVisualStyleBackColor = true;
            this.Controls.Add(this.btnTour);
            this.btnChuyenLe.Name = "btnChuyenLe";
            this.btnChuyenLe.Location = new System.Drawing.Point(40, 135);
            this.btnChuyenLe.Size = new System.Drawing.Size(210, 45);
            this.btnChuyenLe.Text = "Lịch chuyến khách lẻ";
            this.btnChuyenLe.UseVisualStyleBackColor = true;
            this.Controls.Add(this.btnChuyenLe);
            this.btnDangKyLe.Name = "btnDangKyLe";
            this.btnDangKyLe.Location = new System.Drawing.Point(270, 135);
            this.btnDangKyLe.Size = new System.Drawing.Size(210, 45);
            this.btnDangKyLe.Text = "Đăng ký khách lẻ";
            this.btnDangKyLe.UseVisualStyleBackColor = true;
            this.Controls.Add(this.btnDangKyLe);
            this.btnDangKyDoan.Name = "btnDangKyDoan";
            this.btnDangKyDoan.Location = new System.Drawing.Point(40, 195);
            this.btnDangKyDoan.Size = new System.Drawing.Size(210, 45);
            this.btnDangKyDoan.Text = "Đăng ký theo đoàn";
            this.btnDangKyDoan.UseVisualStyleBackColor = true;
            this.Controls.Add(this.btnDangKyDoan);
            this.btnPhanCong.Name = "btnPhanCong";
            this.btnPhanCong.Location = new System.Drawing.Point(270, 195);
            this.btnPhanCong.Size = new System.Drawing.Size(210, 45);
            this.btnPhanCong.Text = "Phân công hướng dẫn viên";
            this.btnPhanCong.UseVisualStyleBackColor = true;
            this.Controls.Add(this.btnPhanCong);
            this.btnKetThuc.Name = "btnKetThuc";
            this.btnKetThuc.Location = new System.Drawing.Point(40, 255);
            this.btnKetThuc.Size = new System.Drawing.Size(210, 45);
            this.btnKetThuc.Text = "Kết thúc tour - khảo sát";
            this.btnKetThuc.UseVisualStyleBackColor = true;
            this.Controls.Add(this.btnKetThuc);
            this.btnThongKe.Name = "btnThongKe";
            this.btnThongKe.Location = new System.Drawing.Point(270, 255);
            this.btnThongKe.Size = new System.Drawing.Size(210, 45);
            this.btnThongKe.Text = "Lương - thống kê";
            this.btnThongKe.UseVisualStyleBackColor = true;
            this.Controls.Add(this.btnThongKe);
            this.btnThoat.Name = "btnThoat";
            this.btnThoat.Location = new System.Drawing.Point(155, 320);
            this.btnThoat.Size = new System.Drawing.Size(210, 40);
            this.btnThoat.Text = "Thoát";
            this.btnThoat.UseVisualStyleBackColor = true;
            this.Controls.Add(this.btnThoat);
            this.btnDanhMuc.Click += new System.EventHandler(this.btnDanhMuc_Click);
            this.btnTour.Click += new System.EventHandler(this.btnTour_Click);
            this.btnChuyenLe.Click += new System.EventHandler(this.btnChuyenLe_Click);
            this.btnDangKyLe.Click += new System.EventHandler(this.btnDangKyLe_Click);
            this.btnDangKyDoan.Click += new System.EventHandler(this.btnDangKyDoan_Click);
            this.btnPhanCong.Click += new System.EventHandler(this.btnPhanCong_Click);
            this.btnKetThuc.Click += new System.EventHandler(this.btnKetThuc_Click);
            this.btnThongKe.Click += new System.EventHandler(this.btnThongKe_Click);
            this.btnThoat.Click += new System.EventHandler(this.btnThoat_Click);
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 400);
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Quản lý công ty du lịch Văn Hóa Việt";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTieuDe;
        private System.Windows.Forms.Button btnDanhMuc;
        private System.Windows.Forms.Button btnTour;
        private System.Windows.Forms.Button btnChuyenLe;
        private System.Windows.Forms.Button btnDangKyLe;
        private System.Windows.Forms.Button btnDangKyDoan;
        private System.Windows.Forms.Button btnPhanCong;
        private System.Windows.Forms.Button btnKetThuc;
        private System.Windows.Forms.Button btnThongKe;
        private System.Windows.Forms.Button btnThoat;
    }
}

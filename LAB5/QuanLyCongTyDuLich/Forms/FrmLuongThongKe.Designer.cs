namespace QuanLyCongTyDuLich.Forms
{
    partial class FrmLuongThongKe
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
            this.tabTK = new System.Windows.Forms.TabControl();
            this.tabL = new System.Windows.Forms.TabPage();
            this.tabH = new System.Windows.Forms.TabPage();
            this.lbl1 = new System.Windows.Forms.Label();
            this.numThang = new System.Windows.Forms.NumericUpDown();
            this.lbl2 = new System.Windows.Forms.Label();
            this.numNam = new System.Windows.Forms.NumericUpDown();
            this.btnLuong = new System.Windows.Forms.Button();
            this.lbl3 = new System.Windows.Forms.Label();
            this.dgvLuong = new System.Windows.Forms.DataGridView();
            this.lbl4 = new System.Windows.Forms.Label();
            this.dtTu = new System.Windows.Forms.DateTimePicker();
            this.lbl5 = new System.Windows.Forms.Label();
            this.dtDen = new System.Windows.Forms.DateTimePicker();
            this.btnTongHop = new System.Windows.Forms.Button();
            this.dgvTongHop = new System.Windows.Forms.DataGridView();
            this.btnDong = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numThang)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNam)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLuong)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTongHop)).BeginInit();
            this.SuspendLayout();
            this.tabTK.Name = "tabTK";
            this.tabTK.Location = new System.Drawing.Point(10, 10);
            this.tabTK.Size = new System.Drawing.Size(840, 460);
            this.Controls.Add(this.tabTK);
            this.tabL.Name = "tabL";
            this.tabL.Location = new System.Drawing.Point(4, 22);
            this.tabL.Size = new System.Drawing.Size(832, 434);
            this.tabL.Text = "Lương hướng dẫn viên";
            this.tabL.UseVisualStyleBackColor = true;
            this.tabL.Padding = new System.Windows.Forms.Padding(3);
            this.tabTK.Controls.Add(this.tabL);
            this.tabH.Name = "tabH";
            this.tabH.Location = new System.Drawing.Point(4, 22);
            this.tabH.Size = new System.Drawing.Size(832, 434);
            this.tabH.Text = "Thống kê tổng hợp";
            this.tabH.UseVisualStyleBackColor = true;
            this.tabH.Padding = new System.Windows.Forms.Padding(3);
            this.tabTK.Controls.Add(this.tabH);
            this.lbl1.Name = "lbl1";
            this.lbl1.Location = new System.Drawing.Point(15, 18);
            this.lbl1.Text = "Tháng:";
            this.lbl1.AutoSize = true;
            this.tabL.Controls.Add(this.lbl1);
            this.numThang.Name = "numThang";
            this.numThang.Location = new System.Drawing.Point(65, 15);
            this.numThang.Size = new System.Drawing.Size(60, 22);
            this.numThang.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numThang.Maximum = new decimal(new int[] { 12, 0, 0, 0 });
            this.numThang.Value = new decimal(new int[] { 1, 0, 0, 0 });
            this.tabL.Controls.Add(this.numThang);
            this.lbl2.Name = "lbl2";
            this.lbl2.Location = new System.Drawing.Point(150, 18);
            this.lbl2.Text = "Năm:";
            this.lbl2.AutoSize = true;
            this.tabL.Controls.Add(this.lbl2);
            this.numNam.Name = "numNam";
            this.numNam.Location = new System.Drawing.Point(190, 15);
            this.numNam.Size = new System.Drawing.Size(80, 22);
            this.numNam.Minimum = new decimal(new int[] { 2000, 0, 0, 0 });
            this.numNam.Maximum = new decimal(new int[] { 2100, 0, 0, 0 });
            this.numNam.Value = new decimal(new int[] { 2026, 0, 0, 0 });
            this.tabL.Controls.Add(this.numNam);
            this.btnLuong.Name = "btnLuong";
            this.btnLuong.Location = new System.Drawing.Point(300, 11);
            this.btnLuong.Size = new System.Drawing.Size(120, 30);
            this.btnLuong.Text = "Tính lương";
            this.btnLuong.UseVisualStyleBackColor = true;
            this.tabL.Controls.Add(this.btnLuong);
            this.lbl3.Name = "lbl3";
            this.lbl3.Location = new System.Drawing.Point(440, 18);
            this.lbl3.Text = "Lương = lương căn bản + thù lao các tour kết thúc trong tháng";
            this.lbl3.AutoSize = true;
            this.tabL.Controls.Add(this.lbl3);
            this.dgvLuong.Name = "dgvLuong";
            this.dgvLuong.Location = new System.Drawing.Point(10, 55);
            this.dgvLuong.Size = new System.Drawing.Size(800, 360);
            this.dgvLuong.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvLuong.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLuong.MultiSelect = false;
            this.dgvLuong.AllowUserToDeleteRows = false;
            this.dgvLuong.AllowUserToAddRows = false;
            this.dgvLuong.ReadOnly = true;
            this.tabL.Controls.Add(this.dgvLuong);
            this.lbl4.Name = "lbl4";
            this.lbl4.Location = new System.Drawing.Point(15, 18);
            this.lbl4.Text = "Từ ngày:";
            this.lbl4.AutoSize = true;
            this.tabH.Controls.Add(this.lbl4);
            this.dtTu.Name = "dtTu";
            this.dtTu.Location = new System.Drawing.Point(75, 15);
            this.dtTu.Size = new System.Drawing.Size(130, 22);
            this.dtTu.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.tabH.Controls.Add(this.dtTu);
            this.lbl5.Name = "lbl5";
            this.lbl5.Location = new System.Drawing.Point(235, 18);
            this.lbl5.Text = "Đến ngày:";
            this.lbl5.AutoSize = true;
            this.tabH.Controls.Add(this.lbl5);
            this.dtDen.Name = "dtDen";
            this.dtDen.Location = new System.Drawing.Point(300, 15);
            this.dtDen.Size = new System.Drawing.Size(130, 22);
            this.dtDen.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.tabH.Controls.Add(this.dtDen);
            this.btnTongHop.Name = "btnTongHop";
            this.btnTongHop.Location = new System.Drawing.Point(460, 11);
            this.btnTongHop.Size = new System.Drawing.Size(120, 30);
            this.btnTongHop.Text = "Thống kê";
            this.btnTongHop.UseVisualStyleBackColor = true;
            this.tabH.Controls.Add(this.btnTongHop);
            this.dgvTongHop.Name = "dgvTongHop";
            this.dgvTongHop.Location = new System.Drawing.Point(10, 55);
            this.dgvTongHop.Size = new System.Drawing.Size(800, 360);
            this.dgvTongHop.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTongHop.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTongHop.MultiSelect = false;
            this.dgvTongHop.AllowUserToDeleteRows = false;
            this.dgvTongHop.AllowUserToAddRows = false;
            this.dgvTongHop.ReadOnly = true;
            this.tabH.Controls.Add(this.dgvTongHop);
            this.btnDong.Name = "btnDong";
            this.btnDong.Location = new System.Drawing.Point(750, 478);
            this.btnDong.Size = new System.Drawing.Size(100, 30);
            this.btnDong.Text = "Đóng";
            this.btnDong.UseVisualStyleBackColor = true;
            this.Controls.Add(this.btnDong);
            this.btnLuong.Click += new System.EventHandler(this.btnLuong_Click);
            this.btnTongHop.Click += new System.EventHandler(this.btnTongHop_Click);
            this.btnDong.Click += new System.EventHandler(this.btnDong_Click);
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(860, 520);
            this.Name = "FrmLuongThongKe";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Lương hướng dẫn viên - thống kê";
            this.Load += new System.EventHandler(this.FrmLuongThongKe_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numThang)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNam)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLuong)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTongHop)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.TabControl tabTK;
        private System.Windows.Forms.TabPage tabL;
        private System.Windows.Forms.TabPage tabH;
        private System.Windows.Forms.Label lbl1;
        private System.Windows.Forms.NumericUpDown numThang;
        private System.Windows.Forms.Label lbl2;
        private System.Windows.Forms.NumericUpDown numNam;
        private System.Windows.Forms.Button btnLuong;
        private System.Windows.Forms.Label lbl3;
        private System.Windows.Forms.DataGridView dgvLuong;
        private System.Windows.Forms.Label lbl4;
        private System.Windows.Forms.DateTimePicker dtTu;
        private System.Windows.Forms.Label lbl5;
        private System.Windows.Forms.DateTimePicker dtDen;
        private System.Windows.Forms.Button btnTongHop;
        private System.Windows.Forms.DataGridView dgvTongHop;
        private System.Windows.Forms.Button btnDong;
    }
}

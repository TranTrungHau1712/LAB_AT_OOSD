namespace QuanLyCongTyDuLich.Forms
{
    partial class FrmChuyenLe
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
            this.lbl1 = new System.Windows.Forms.Label();
            this.txtMa = new System.Windows.Forms.TextBox();
            this.lbl2 = new System.Windows.Forms.Label();
            this.cboTour = new System.Windows.Forms.ComboBox();
            this.lbl3 = new System.Windows.Forms.Label();
            this.dtDi = new System.Windows.Forms.DateTimePicker();
            this.lbl4 = new System.Windows.Forms.Label();
            this.lblNgayVe = new System.Windows.Forms.Label();
            this.lbl5 = new System.Windows.Forms.Label();
            this.txtDon = new System.Windows.Forms.TextBox();
            this.btnThem = new System.Windows.Forms.Button();
            this.btnDongDK = new System.Windows.Forms.Button();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.btnDong = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            this.lbl1.Name = "lbl1";
            this.lbl1.Location = new System.Drawing.Point(15, 18);
            this.lbl1.Text = "Mã chuyến:";
            this.lbl1.AutoSize = true;
            this.Controls.Add(this.lbl1);
            this.txtMa.Name = "txtMa";
            this.txtMa.Location = new System.Drawing.Point(100, 15);
            this.txtMa.Size = new System.Drawing.Size(150, 22);
            this.Controls.Add(this.txtMa);
            this.lbl2.Name = "lbl2";
            this.lbl2.Location = new System.Drawing.Point(300, 18);
            this.lbl2.Text = "Tour:";
            this.lbl2.AutoSize = true;
            this.Controls.Add(this.lbl2);
            this.cboTour.Name = "cboTour";
            this.cboTour.Location = new System.Drawing.Point(340, 15);
            this.cboTour.Size = new System.Drawing.Size(330, 22);
            this.cboTour.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTour.FormattingEnabled = true;
            this.Controls.Add(this.cboTour);
            this.lbl3.Name = "lbl3";
            this.lbl3.Location = new System.Drawing.Point(15, 53);
            this.lbl3.Text = "Ngày đi:";
            this.lbl3.AutoSize = true;
            this.Controls.Add(this.lbl3);
            this.dtDi.Name = "dtDi";
            this.dtDi.Location = new System.Drawing.Point(100, 50);
            this.dtDi.Size = new System.Drawing.Size(150, 22);
            this.dtDi.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.Controls.Add(this.dtDi);
            this.lbl4.Name = "lbl4";
            this.lbl4.Location = new System.Drawing.Point(300, 53);
            this.lbl4.Text = "Ngày về:";
            this.lbl4.AutoSize = true;
            this.Controls.Add(this.lbl4);
            this.lblNgayVe.Name = "lblNgayVe";
            this.lblNgayVe.Location = new System.Drawing.Point(360, 53);
            this.lblNgayVe.Text = "-";
            this.lblNgayVe.AutoSize = true;
            this.lblNgayVe.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.Controls.Add(this.lblNgayVe);
            this.lbl5.Name = "lbl5";
            this.lbl5.Location = new System.Drawing.Point(15, 88);
            this.lbl5.Text = "Địa điểm đón:";
            this.lbl5.AutoSize = true;
            this.Controls.Add(this.lbl5);
            this.txtDon.Name = "txtDon";
            this.txtDon.Location = new System.Drawing.Point(100, 85);
            this.txtDon.Size = new System.Drawing.Size(400, 22);
            this.Controls.Add(this.txtDon);
            this.btnThem.Name = "btnThem";
            this.btnThem.Location = new System.Drawing.Point(560, 80);
            this.btnThem.Size = new System.Drawing.Size(120, 30);
            this.btnThem.Text = "Tạo chuyến";
            this.btnThem.UseVisualStyleBackColor = true;
            this.Controls.Add(this.btnThem);
            this.btnDongDK.Name = "btnDongDK";
            this.btnDongDK.Location = new System.Drawing.Point(700, 80);
            this.btnDongDK.Size = new System.Drawing.Size(130, 30);
            this.btnDongDK.Text = "Đóng đăng ký";
            this.btnDongDK.UseVisualStyleBackColor = true;
            this.Controls.Add(this.btnDongDK);
            this.dgv.Name = "dgv";
            this.dgv.Location = new System.Drawing.Point(10, 125);
            this.dgv.Size = new System.Drawing.Size(840, 340);
            this.dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv.MultiSelect = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.AllowUserToAddRows = false;
            this.dgv.ReadOnly = true;
            this.Controls.Add(this.dgv);
            this.btnDong.Name = "btnDong";
            this.btnDong.Location = new System.Drawing.Point(750, 478);
            this.btnDong.Size = new System.Drawing.Size(100, 30);
            this.btnDong.Text = "Đóng";
            this.btnDong.UseVisualStyleBackColor = true;
            this.Controls.Add(this.btnDong);
            this.cboTour.SelectedIndexChanged += new System.EventHandler(this.TinhNgayVe);
            this.dtDi.ValueChanged += new System.EventHandler(this.TinhNgayVe);
            this.btnThem.Click += new System.EventHandler(this.btnThem_Click);
            this.btnDongDK.Click += new System.EventHandler(this.btnDongDK_Click);
            this.btnDong.Click += new System.EventHandler(this.btnDong_Click);
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(860, 520);
            this.Name = "FrmChuyenLe";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Lịch chuyến khách lẻ";
            this.Load += new System.EventHandler(this.FrmChuyenLe_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lbl1;
        private System.Windows.Forms.TextBox txtMa;
        private System.Windows.Forms.Label lbl2;
        private System.Windows.Forms.ComboBox cboTour;
        private System.Windows.Forms.Label lbl3;
        private System.Windows.Forms.DateTimePicker dtDi;
        private System.Windows.Forms.Label lbl4;
        private System.Windows.Forms.Label lblNgayVe;
        private System.Windows.Forms.Label lbl5;
        private System.Windows.Forms.TextBox txtDon;
        private System.Windows.Forms.Button btnThem;
        private System.Windows.Forms.Button btnDongDK;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.Button btnDong;
    }
}

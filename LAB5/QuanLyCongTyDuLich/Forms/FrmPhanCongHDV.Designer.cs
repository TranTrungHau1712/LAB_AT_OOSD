namespace QuanLyCongTyDuLich.Forms
{
    partial class FrmPhanCongHDV
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
            this.txtMaPC = new System.Windows.Forms.TextBox();
            this.lbl2 = new System.Windows.Forms.Label();
            this.cboHDV = new System.Windows.Forms.ComboBox();
            this.lbl3 = new System.Windows.Forms.Label();
            this.cboLoai = new System.Windows.Forms.ComboBox();
            this.lbl4 = new System.Windows.Forms.Label();
            this.cboDoiTuong = new System.Windows.Forms.ComboBox();
            this.lbl5 = new System.Windows.Forms.Label();
            this.numThuLao = new System.Windows.Forms.NumericUpDown();
            this.lbl6 = new System.Windows.Forms.Label();
            this.btnPhanCong = new System.Windows.Forms.Button();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.btnDong = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numThuLao)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            this.lbl1.Name = "lbl1";
            this.lbl1.Location = new System.Drawing.Point(15, 18);
            this.lbl1.Text = "Mã phân công:";
            this.lbl1.AutoSize = true;
            this.Controls.Add(this.lbl1);
            this.txtMaPC.Name = "txtMaPC";
            this.txtMaPC.Location = new System.Drawing.Point(100, 15);
            this.txtMaPC.Size = new System.Drawing.Size(150, 22);
            this.Controls.Add(this.txtMaPC);
            this.lbl2.Name = "lbl2";
            this.lbl2.Location = new System.Drawing.Point(290, 18);
            this.lbl2.Text = "Hướng dẫn viên:";
            this.lbl2.AutoSize = true;
            this.Controls.Add(this.lbl2);
            this.cboHDV.Name = "cboHDV";
            this.cboHDV.Location = new System.Drawing.Point(390, 15);
            this.cboHDV.Size = new System.Drawing.Size(280, 22);
            this.cboHDV.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboHDV.FormattingEnabled = true;
            this.Controls.Add(this.cboHDV);
            this.lbl3.Name = "lbl3";
            this.lbl3.Location = new System.Drawing.Point(15, 53);
            this.lbl3.Text = "Loại:";
            this.lbl3.AutoSize = true;
            this.Controls.Add(this.lbl3);
            this.cboLoai.Name = "cboLoai";
            this.cboLoai.Location = new System.Drawing.Point(100, 50);
            this.cboLoai.Size = new System.Drawing.Size(100, 22);
            this.cboLoai.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLoai.FormattingEnabled = true;
            this.Controls.Add(this.cboLoai);
            this.lbl4.Name = "lbl4";
            this.lbl4.Location = new System.Drawing.Point(290, 53);
            this.lbl4.Text = "Chuyến / đoàn:";
            this.lbl4.AutoSize = true;
            this.Controls.Add(this.lbl4);
            this.cboDoiTuong.Name = "cboDoiTuong";
            this.cboDoiTuong.Location = new System.Drawing.Point(390, 50);
            this.cboDoiTuong.Size = new System.Drawing.Size(450, 22);
            this.cboDoiTuong.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDoiTuong.FormattingEnabled = true;
            this.Controls.Add(this.cboDoiTuong);
            this.lbl5.Name = "lbl5";
            this.lbl5.Location = new System.Drawing.Point(15, 90);
            this.lbl5.Text = "Thù lao tour:";
            this.lbl5.AutoSize = true;
            this.Controls.Add(this.lbl5);
            this.numThuLao.Name = "numThuLao";
            this.numThuLao.Location = new System.Drawing.Point(100, 87);
            this.numThuLao.Size = new System.Drawing.Size(130, 22);
            this.numThuLao.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            this.numThuLao.Maximum = new decimal(new int[] { 1000000000, 0, 0, 0 });
            this.numThuLao.Value = new decimal(new int[] { 0, 0, 0, 0 });
            this.numThuLao.ThousandsSeparator = true;
            this.Controls.Add(this.numThuLao);
            this.lbl6.Name = "lbl6";
            this.lbl6.Location = new System.Drawing.Point(260, 90);
            this.lbl6.Text = "Ngày bắt đầu / kết thúc lấy theo chuyến hoặc phiếu đoàn.";
            this.lbl6.AutoSize = true;
            this.Controls.Add(this.lbl6);
            this.btnPhanCong.Name = "btnPhanCong";
            this.btnPhanCong.Location = new System.Drawing.Point(700, 82);
            this.btnPhanCong.Size = new System.Drawing.Size(140, 32);
            this.btnPhanCong.Text = "Phân công";
            this.btnPhanCong.UseVisualStyleBackColor = true;
            this.Controls.Add(this.btnPhanCong);
            this.dgv.Name = "dgv";
            this.dgv.Location = new System.Drawing.Point(10, 130);
            this.dgv.Size = new System.Drawing.Size(840, 335);
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
            this.cboLoai.SelectedIndexChanged += new System.EventHandler(this.cboLoai_SelectedIndexChanged);
            this.btnPhanCong.Click += new System.EventHandler(this.btnPhanCong_Click);
            this.btnDong.Click += new System.EventHandler(this.btnDong_Click);
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(860, 520);
            this.Name = "FrmPhanCongHDV";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Phân công hướng dẫn viên";
            this.Load += new System.EventHandler(this.FrmPhanCongHDV_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numThuLao)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lbl1;
        private System.Windows.Forms.TextBox txtMaPC;
        private System.Windows.Forms.Label lbl2;
        private System.Windows.Forms.ComboBox cboHDV;
        private System.Windows.Forms.Label lbl3;
        private System.Windows.Forms.ComboBox cboLoai;
        private System.Windows.Forms.Label lbl4;
        private System.Windows.Forms.ComboBox cboDoiTuong;
        private System.Windows.Forms.Label lbl5;
        private System.Windows.Forms.NumericUpDown numThuLao;
        private System.Windows.Forms.Label lbl6;
        private System.Windows.Forms.Button btnPhanCong;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.Button btnDong;
    }
}

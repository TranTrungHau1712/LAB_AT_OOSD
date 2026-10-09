namespace QuanLyCongTyDuLich.Forms
{
    partial class FrmDangKyLe
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
            this.txtSo = new System.Windows.Forms.TextBox();
            this.lbl2 = new System.Windows.Forms.Label();
            this.cboChuyen = new System.Windows.Forms.ComboBox();
            this.lbl3 = new System.Windows.Forms.Label();
            this.cboDiemBan = new System.Windows.Forms.ComboBox();
            this.lbl4 = new System.Windows.Forms.Label();
            this.txtTen = new System.Windows.Forms.TextBox();
            this.lbl5 = new System.Windows.Forms.Label();
            this.txtDT = new System.Windows.Forms.TextBox();
            this.lbl6 = new System.Windows.Forms.Label();
            this.numNguoi = new System.Windows.Forms.NumericUpDown();
            this.lbl7 = new System.Windows.Forms.Label();
            this.lblThanhTien = new System.Windows.Forms.Label();
            this.btnDangKy = new System.Windows.Forms.Button();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.btnDong = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numNguoi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            this.lbl1.Name = "lbl1";
            this.lbl1.Location = new System.Drawing.Point(15, 18);
            this.lbl1.Text = "Số đăng ký:";
            this.lbl1.AutoSize = true;
            this.Controls.Add(this.lbl1);
            this.txtSo.Name = "txtSo";
            this.txtSo.Location = new System.Drawing.Point(100, 15);
            this.txtSo.Size = new System.Drawing.Size(150, 22);
            this.Controls.Add(this.txtSo);
            this.lbl2.Name = "lbl2";
            this.lbl2.Location = new System.Drawing.Point(290, 18);
            this.lbl2.Text = "Chuyến:";
            this.lbl2.AutoSize = true;
            this.Controls.Add(this.lbl2);
            this.cboChuyen.Name = "cboChuyen";
            this.cboChuyen.Location = new System.Drawing.Point(345, 15);
            this.cboChuyen.Size = new System.Drawing.Size(400, 22);
            this.cboChuyen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboChuyen.FormattingEnabled = true;
            this.Controls.Add(this.cboChuyen);
            this.lbl3.Name = "lbl3";
            this.lbl3.Location = new System.Drawing.Point(15, 53);
            this.lbl3.Text = "Điểm bán vé:";
            this.lbl3.AutoSize = true;
            this.Controls.Add(this.lbl3);
            this.cboDiemBan.Name = "cboDiemBan";
            this.cboDiemBan.Location = new System.Drawing.Point(100, 50);
            this.cboDiemBan.Size = new System.Drawing.Size(180, 22);
            this.cboDiemBan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDiemBan.FormattingEnabled = true;
            this.Controls.Add(this.cboDiemBan);
            this.lbl4.Name = "lbl4";
            this.lbl4.Location = new System.Drawing.Point(310, 53);
            this.lbl4.Text = "Người đăng ký:";
            this.lbl4.AutoSize = true;
            this.Controls.Add(this.lbl4);
            this.txtTen.Name = "txtTen";
            this.txtTen.Location = new System.Drawing.Point(400, 50);
            this.txtTen.Size = new System.Drawing.Size(200, 22);
            this.Controls.Add(this.txtTen);
            this.lbl5.Name = "lbl5";
            this.lbl5.Location = new System.Drawing.Point(630, 53);
            this.lbl5.Text = "Điện thoại:";
            this.lbl5.AutoSize = true;
            this.Controls.Add(this.lbl5);
            this.txtDT.Name = "txtDT";
            this.txtDT.Location = new System.Drawing.Point(700, 50);
            this.txtDT.Size = new System.Drawing.Size(140, 22);
            this.Controls.Add(this.txtDT);
            this.lbl6.Name = "lbl6";
            this.lbl6.Location = new System.Drawing.Point(15, 90);
            this.lbl6.Text = "Số người:";
            this.lbl6.AutoSize = true;
            this.Controls.Add(this.lbl6);
            this.numNguoi.Name = "numNguoi";
            this.numNguoi.Location = new System.Drawing.Point(100, 87);
            this.numNguoi.Size = new System.Drawing.Size(70, 22);
            this.numNguoi.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numNguoi.Maximum = new decimal(new int[] { 11, 0, 0, 0 });
            this.numNguoi.Value = new decimal(new int[] { 1, 0, 0, 0 });
            this.Controls.Add(this.numNguoi);
            this.lbl7.Name = "lbl7";
            this.lbl7.Location = new System.Drawing.Point(200, 90);
            this.lbl7.Text = "Thành tiền:";
            this.lbl7.AutoSize = true;
            this.Controls.Add(this.lbl7);
            this.lblThanhTien.Name = "lblThanhTien";
            this.lblThanhTien.Location = new System.Drawing.Point(280, 90);
            this.lblThanhTien.Text = "0 đ";
            this.lblThanhTien.AutoSize = true;
            this.lblThanhTien.ForeColor = System.Drawing.Color.Maroon;
            this.lblThanhTien.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.Controls.Add(this.lblThanhTien);
            this.btnDangKy.Name = "btnDangKy";
            this.btnDangKy.Location = new System.Drawing.Point(600, 82);
            this.btnDangKy.Size = new System.Drawing.Size(240, 32);
            this.btnDangKy.Text = "Đăng ký và thanh toán vé";
            this.btnDangKy.UseVisualStyleBackColor = true;
            this.Controls.Add(this.btnDangKy);
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
            this.cboChuyen.SelectedIndexChanged += new System.EventHandler(this.TinhTien);
            this.numNguoi.ValueChanged += new System.EventHandler(this.TinhTien);
            this.btnDangKy.Click += new System.EventHandler(this.btnDangKy_Click);
            this.btnDong.Click += new System.EventHandler(this.btnDong_Click);
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(860, 520);
            this.Name = "FrmDangKyLe";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Đăng ký khách lẻ theo chuyến";
            this.Load += new System.EventHandler(this.FrmDangKyLe_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numNguoi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lbl1;
        private System.Windows.Forms.TextBox txtSo;
        private System.Windows.Forms.Label lbl2;
        private System.Windows.Forms.ComboBox cboChuyen;
        private System.Windows.Forms.Label lbl3;
        private System.Windows.Forms.ComboBox cboDiemBan;
        private System.Windows.Forms.Label lbl4;
        private System.Windows.Forms.TextBox txtTen;
        private System.Windows.Forms.Label lbl5;
        private System.Windows.Forms.TextBox txtDT;
        private System.Windows.Forms.Label lbl6;
        private System.Windows.Forms.NumericUpDown numNguoi;
        private System.Windows.Forms.Label lbl7;
        private System.Windows.Forms.Label lblThanhTien;
        private System.Windows.Forms.Button btnDangKy;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.Button btnDong;
    }
}

namespace QuanLyThuVien
{
    partial class FrmThongKe
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.dtTu = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.dtDen = new System.Windows.Forms.DateTimePicker();
            this.btnThongKe = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.lblMuon = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblQuaHan = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblMat = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblHuHong = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lblPhiPhat = new System.Windows.Forms.Label();
            this.dgvPhat = new System.Windows.Forms.DataGridView();
            this.btnDong = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPhat)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Từ ngày";
            // 
            // dtTu
            // 
            this.dtTu.Location = new System.Drawing.Point(142, 27);
            this.dtTu.Name = "dtTu";
            this.dtTu.Size = new System.Drawing.Size(200, 22);
            this.dtTu.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(415, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Đến ngày";
            // 
            // dtDen
            // 
            this.dtDen.Location = new System.Drawing.Point(530, 32);
            this.dtDen.Name = "dtDen";
            this.dtDen.Size = new System.Drawing.Size(200, 22);
            this.dtDen.TabIndex = 3;
            // 
            // btnThongKe
            // 
            this.btnThongKe.Location = new System.Drawing.Point(27, 65);
            this.btnThongKe.Name = "btnThongKe";
            this.btnThongKe.Size = new System.Drawing.Size(100, 30);
            this.btnThongKe.TabIndex = 4;
            this.btnThongKe.Text = "Thống kê";
            this.btnThongKe.UseVisualStyleBackColor = true;
            this.btnThongKe.Click += new System.EventHandler(this.btnThongKe_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(116, 16);
            this.label3.TabIndex = 5;
            this.label3.Text = "Số lượt sách mượn";
            // 
            // lblMuon
            // 
            this.lblMuon.AutoSize = true;
            this.lblMuon.Location = new System.Drawing.Point(27, 156);
            this.lblMuon.Name = "lblMuon";
            this.lblMuon.Size = new System.Drawing.Size(0, 16);
            this.lblMuon.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(201, 115);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(107, 16);
            this.label5.TabIndex = 7;
            this.label5.Text = "Số sách quá hạn";
            // 
            // lblQuaHan
            // 
            this.lblQuaHan.AutoSize = true;
            this.lblQuaHan.Location = new System.Drawing.Point(201, 156);
            this.lblQuaHan.Name = "lblQuaHan";
            this.lblQuaHan.Size = new System.Drawing.Size(0, 16);
            this.lblQuaHan.TabIndex = 8;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(389, 115);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(81, 16);
            this.label7.TabIndex = 9;
            this.label7.Text = "Số sách mất";
            // 
            // lblMat
            // 
            this.lblMat.AutoSize = true;
            this.lblMat.Location = new System.Drawing.Point(389, 156);
            this.lblMat.Name = "lblMat";
            this.lblMat.Size = new System.Drawing.Size(0, 16);
            this.lblMat.TabIndex = 10;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(577, 115);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(106, 16);
            this.label9.TabIndex = 11;
            this.label9.Text = "Số sách hư hỏng";
            // 
            // lblHuHong
            // 
            this.lblHuHong.AutoSize = true;
            this.lblHuHong.Location = new System.Drawing.Point(577, 156);
            this.lblHuHong.Name = "lblHuHong";
            this.lblHuHong.Size = new System.Drawing.Size(0, 16);
            this.lblHuHong.TabIndex = 12;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(800, 115);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(89, 16);
            this.label11.TabIndex = 13;
            this.label11.Text = "Tổng phí phạt";
            // 
            // lblPhiPhat
            // 
            this.lblPhiPhat.AutoSize = true;
            this.lblPhiPhat.Location = new System.Drawing.Point(800, 156);
            this.lblPhiPhat.Name = "lblPhiPhat";
            this.lblPhiPhat.Size = new System.Drawing.Size(0, 16);
            this.lblPhiPhat.TabIndex = 14;
            // 
            // dgvPhat
            // 
            this.dgvPhat.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPhat.Location = new System.Drawing.Point(4, 197);
            this.dgvPhat.MultiSelect = false;
            this.dgvPhat.Name = "dgvPhat";
            this.dgvPhat.ReadOnly = true;
            this.dgvPhat.RowHeadersWidth = 51;
            this.dgvPhat.RowTemplate.Height = 24;
            this.dgvPhat.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPhat.Size = new System.Drawing.Size(1047, 343);
            this.dgvPhat.TabIndex = 15;
            // 
            // btnDong
            // 
            this.btnDong.Location = new System.Drawing.Point(912, 580);
            this.btnDong.Name = "btnDong";
            this.btnDong.Size = new System.Drawing.Size(90, 30);
            this.btnDong.TabIndex = 16;
            this.btnDong.Text = "Đóng";
            this.btnDong.UseVisualStyleBackColor = true;
            this.btnDong.Click += new System.EventHandler(this.btnDong_Click);
            // 
            // FrmThongKe
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1052, 653);
            this.Controls.Add(this.btnDong);
            this.Controls.Add(this.dgvPhat);
            this.Controls.Add(this.lblPhiPhat);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.lblHuHong);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.lblMat);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lblQuaHan);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblMuon);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnThongKe);
            this.Controls.Add(this.dtDen);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dtTu);
            this.Controls.Add(this.label1);
            this.Name = "FrmThongKe";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FrmThongKe";
            this.Load += new System.EventHandler(this.FrmThongKe_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPhat)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtTu;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtDen;
        private System.Windows.Forms.Button btnThongKe;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblMuon;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblQuaHan;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblMat;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblHuHong;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lblPhiPhat;
        private System.Windows.Forms.DataGridView dgvPhat;
        private System.Windows.Forms.Button btnDong;
    }
}
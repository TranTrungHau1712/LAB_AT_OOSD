using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyThuVien.Forms
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnDanhMuc_Click(object sender, EventArgs e)
        {
            using (FrmDanhMuc f = new FrmDanhMuc())
            {
                f.ShowDialog();
            }

        }

        private void btnSach_Click(object sender, EventArgs e)
        {
            using (FrmSach f = new FrmSach())
            { f.ShowDialog(); }

        }

        private void btnDocGia_Click(object sender, EventArgs e)
        {
            using (FrmDocGia f = new FrmDocGia())
            {
                f.ShowDialog();
            }

        }

        private void btnMuonTra_Click(object sender, EventArgs e)
        {
            using (FrmMuonTra f = new FrmMuonTra())
            {
                f.ShowDialog();
            }

        }

        private void btnThongKe_Click(object sender, EventArgs e)
        {
            using (FrmThongKe f = new FrmThongKe())
            {
                f.ShowDialog();
            }

        }

        private void btnThoat_Click(object sender, EventArgs e)
        {

        }
    }
}

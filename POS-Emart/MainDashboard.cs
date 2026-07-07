using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS_Emart
{
    public partial class MainDashboard : Form
    {
        public MainDashboard()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnHome_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void MainDashboard_Load(object sender, EventArgs e)
        {
            Global.SCREEN = panelContent;
            lblLoggedName.Text = UserSession.Username;
            lblLoggedRole.Text = UserSession.Role;
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Global.OpenForm(new CategoryFrm());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Global.OpenForm(new ProductFrm());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Global.OpenForm(new UserFrm());
        }

        private void label1_Click(object sender, EventArgs e)
        {
            
        }

        private void panelContent_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

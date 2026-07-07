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
    public partial class UserUpdateFrm : Form
    {
        public UserUpdateFrm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Global.OpenForm(new UserFrm());
        }

        private void UserUpdateFrm_Load(object sender, EventArgs e)
        {

        }
    }
}

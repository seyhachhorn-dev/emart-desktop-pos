using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS_Emart
{
    internal class Global
    {


        public static Panel SCREEN = null;

        public static void OpenForm(Form frm)
        {
            if(SCREEN == null)
            {
                return; 
            }
            if (frm == null)
            {
                return;
            }
            SCREEN.Controls.Clear();
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;
            frm.TopLevel = false;
            frm.Show();
            SCREEN.Controls.Add(frm);

        }
    }
}

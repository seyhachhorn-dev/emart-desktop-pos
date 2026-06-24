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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void categoriesBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.categoriesBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.emartDataSet);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'emartDataSet.Categories' table. You can move, or remove it, as needed.
            this.categoriesTableAdapter.Fill(this.emartDataSet.Categories);

        }

        private void statusCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void toolStripCreateBtn_Click(object sender, EventArgs e)
        {
            this.categoriesBindingSource.AddNew();
            this.statusCheckBox.Checked = false;

        }

        private void idTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void toolStripSaveBtn_Click(object sender, EventArgs e)
        {
            this.categoriesBindingNavigatorSaveItem_Click(sender, e);
            MessageBox.Show("Saved!!");
        }

        private void toolStripDeleteBtn_Click(object sender, EventArgs e)
        {

        }

        private void toolStripSearchBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBoxSearch.Text))
            {
                // Escape single quotes to prevent SQL syntax errors if a user types an apostrophe
                string safeText = textBoxSearch.Text.Replace("'", "''");
                this.categoriesBindingSource.Filter = string.Format("title LIKE '%{0}%'", safeText);

            }
            else
            {
                this.categoriesBindingSource.Filter = null;
            }
        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            
        }
    }
}

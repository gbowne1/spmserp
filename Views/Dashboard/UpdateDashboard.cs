using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using spmserp.Controllers;

namespace spmserp.Views
{
    public partial class UpdateDashboard : Form
    {
        public UpdateDashboard()
        {
            InitializeComponent();
            gridAdmins.DataSource = AdminController.GetAllAdmins();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            dynamic admin = AdminController.GetAdmin(tbSearch.Text);
            if (admin != null)
            {
                tbName.Text = admin.Name;
                tbUsername.Text = admin.Username;
            }
            else
            {
                tbName.Text = "";
                tbUsername.Text = "";
                MessageBox.Show("Admin not found!");
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            var admin = new
            {
                Name = tbName.Text,
                Username = tbUsername.Text,
            };
            var r = AdminController.UpdateAdmin(admin);
            if (r)
            {
                tbName.Text = "";
                tbUsername.Text = "";
                MessageBox.Show("Successfully Updated!");
                gridAdmins.DataSource = AdminController.GetAllAdmins();
            }
            else
            {
                MessageBox.Show("Failed to update");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var r = AdminController.DeleteAdmin(tbUsername.Text);
            if (r)
            {
                tbName.Text = "";
                tbUsername.Text = "";
                MessageBox.Show("Admin Deleted!");
                gridAdmins.DataSource = AdminController.GetAllAdmins();
            }
            else
            {
                tbName.Text = "";
                tbUsername.Text = "";
                MessageBox.Show("Can not Delete!");
            }
        }
    }
}
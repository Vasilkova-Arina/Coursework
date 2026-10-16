using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace practic
{
    public partial class PoductView : Form
    {
        private string cmdStr =
            @"SELECT 
                    p.ProductID,
                    p.ProductName,
                    p.ProductDecription,
                    p.ProductSale,
                    c.CategoryName,
                    s.SupplierName,
                    p.ProductPhoto
                FROM Product p
                JOIN Category c ON c.CategoryID = p.ProductCategory
                JOIN Supplier s ON s.SupplierID = p.ProductSupplier;";
        public PoductView()
        {
            InitializeComponent();
            SetupGrid();
        }

        private void PoductView_Load(object sender, EventArgs e)
        {
            using (var conn = Connection.GetConnection())
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(cmdStr, conn))
                {
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }
                dataGridView1.Columns["ProductID"].Visible = false;
                dataGridView1.Columns["ProductName"].HeaderText = "Название";
                dataGridView1.Columns["ProductDecription"].HeaderText = "Описание";
                dataGridView1.Columns["ProductSale"].HeaderText = "Цена";
                dataGridView1.Columns["CategoryName"].HeaderText = "Категория";
                dataGridView1.Columns["SupplierName"].HeaderText = "Поставщик";
                dataGridView1.Columns["ProductPhoto"].HeaderText = "Фото";
        }
        private void SetupGrid()
        {
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.AllowUserToOrderColumns = true;

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;

            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            dataGridView1.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dataGridView1.ScrollBars = ScrollBars.Both;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms.Count > 1)
            {
                Form pastForm = Application.OpenForms[Application.OpenForms.Count - 2];
                pastForm.Show();
            }

            this.Close();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace practic
{
    public partial class Product : Form
    {
        private string cmdStr =
                @"SELECT 
                    p.ProductID,
                    p.ProductName,
                    p.ProductDecription,
                    p.ProductSale,
                    p.ProductCount,
                    c.CategoryName,
                    s.SupplierName,
                    p.ProductPhoto
                FROM Product p
                JOIN Category c ON c.CategoryID = p.ProductCategory
                JOIN Supplier s ON s.SupplierID = p.ProductSupplier";

        public Product()
        {
            InitializeComponent();
            SetupGrid();
            LoadFilters();
        }

        private void Product_Load(object sender, EventArgs e)
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
            dataGridView1.Columns["ProductCount"].HeaderText = "Кол-во";
            dataGridView1.Columns["CategoryName"].HeaderText = "Категория";
            dataGridView1.Columns["SupplierName"].HeaderText = "Поставщик";
            //Исправить для фото
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

        private void button1_Click(object sender, EventArgs e)
        {
            txtPoick.Text = "Поиск";
            if (cmbFil.Items.Count > 0) cmbFil.SelectedIndex = 0;
            if (cmbSort.Items.Count > 0) cmbSort.SelectedIndex = 0;
            LoadData();
        }
        private void LoadData()
        {
            string sql = cmdStr;

            int productId = 0;
            string searchText = txtPoick.Text.Trim();

            if (!string.IsNullOrEmpty(searchText) && searchText != "Поиск")
            {
                if (int.TryParse(searchText, out int parsedId))
                {
                    productId = parsedId;
                    sql += " AND p.ProductID = @productId ";
                }
                else
                {
                    sql += " AND p.ProductName LIKE @searchText ";
                }
            }

            if (cmbFil.SelectedIndex > 0)
            {
                sql += " AND c.CategoryName = @categoryName ";
            }

            if (cmbSort.SelectedIndex == 1)
                sql += " ORDER BY p.ProductSale ASC ";
            else if (cmbSort.SelectedIndex == 2)
                sql += " ORDER BY p.ProductSale DESC ";
            else
                sql += " ORDER BY p.ProductID ";

            using (var conn = Connection.GetConnection())
            using (var cmd = new MySqlCommand(sql, conn))
            {
                if (productId > 0)
                    cmd.Parameters.AddWithValue("@productId", productId);

                if (!string.IsNullOrEmpty(searchText) && searchText != "Поиск" && !int.TryParse(searchText, out _))
                    cmd.Parameters.AddWithValue("@searchText", "%" + searchText + "%");

                if (cmbFil.SelectedIndex > 0)
                    cmd.Parameters.AddWithValue("@categoryName", cmbFil.SelectedItem.ToString());

                conn.Open();
                var da = new MySqlDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }

            if (dataGridView1.Columns.Count > 0)
            { 
                dataGridView1.Columns["ProductID"].HeaderText = "Номер";
                dataGridView1.Columns["ProductName"].HeaderText = "Название товара";
                dataGridView1.Columns["ProductDecription"].HeaderText = "Описание товара";
                dataGridView1.Columns["ProductSale"].HeaderText = "Цена";
                dataGridView1.Columns["CategoryName"].HeaderText = "Категория";
                dataGridView1.Columns["SupplierName"].HeaderText = "Поставщик";
                dataGridView1.Columns["ProductPhoto"].HeaderText = "Фото товара";

                dataGridView1.Columns["ProductID"].Visible = false;
            }
            label3.Text = "Количество записей: " + dataGridView1.Rows.Count.ToString();

            PaginationHelper pager = new PaginationHelper(dataGridView1, this, 10);
            pager.Draw(800, 500);

        }
        private void LoadFilters()
        {
            cmbFil.Items.Clear();
            cmbFil.Items.Add("Все категории");
            using (var conn = Connection.GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand("SELECT CategoryName FROM Category", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cmbFil.Items.Add(reader.GetString(0));
                    }
                }
            }
            cmbFil.SelectedIndex = 0;

            cmbSort.Items.Clear();
            cmbSort.Items.Add("Без сортировки");
            cmbSort.Items.Add("По цене (по возрастанию)");
            cmbSort.Items.Add("По цене (по убыванию)");
            cmbSort.SelectedIndex = 0;
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView1.Columns[e.ColumnIndex].Name == "ProductCount")
            {
                int count = Convert.ToInt32(e.Value);

                if (count < 5)
                {
                    e.CellStyle.BackColor = Color.Red;
                    e.CellStyle.ForeColor = Color.White;
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            userView.Text = $"Добро пожаловать: {Session.CurrentUser.Fio} | {DateTime.Now:dd.MM.yyyy HH:mm:ss}";
        }

        private void txtPoick_TextChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void cmbFil_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void cmbSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void txtPoick_Enter(object sender, EventArgs e)
        {
            if (txtPoick.Text == "Поиск") txtPoick.Text = "";
        }

        private void txtPoick_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPoick.Text)) txtPoick.Text = "Поиск";
        }
    }
}

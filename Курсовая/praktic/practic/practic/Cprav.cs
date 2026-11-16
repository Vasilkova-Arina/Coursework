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
    public partial class Cprav : Form
    {
        private bool isAddingNew = false;
        private int currentId = 0;
        public Cprav()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Cprav_Load(object sender, EventArgs e)
        {
            label12.Text = $"Добро пожаловать: {Session.CurrentUser.Fio} | {DateTime.Now:dd.MM.yyyy HH:mm:ss}";
            LoadAllData();

            SetupGrid(dataGridView1, "SupplierID", new[] { "SupplierName", "Поставщик", "SupplierPhone", "Телефон" });
            SetupGrid(dataGridView2, "CategoryID", new[] { "CategoryName", "Категория" });
            SetupGrid(dataGridView3, "MethodPaymentID", new[] { "MethodPaymentName", "Способ оплаты" });
            SetupGrid(dataGridView4, "StatusID", new[] { "StatusName", "Статус" });

            ClearFields();
            UpdateCountLabel();
        }
        private void LoadAllData()
        {
            using (var conn = Connection.GetConnection())
            {
                conn.Open();
                FillGrid(conn, "SELECT * FROM Supplier;", dataGridView1);
                FillGrid(conn, "SELECT * FROM Category;", dataGridView2);
                FillGrid(conn, "SELECT * FROM MethodPayment;", dataGridView3);
                FillGrid(conn, "SELECT * FROM Status;", dataGridView4);
            }
            UpdateCountLabel();
        }
        private void UpdateCountLabel()
        {
            if (dataGridView1.Visible)
                label6.Text = "Количество записей: " + dataGridView1.Rows.Count.ToString();
            else if (dataGridView2.Visible)
                label5.Text = "Количество записей: " + dataGridView2.Rows.Count.ToString();
            else if (dataGridView3.Visible)
                label4.Text = "Количество записей: " + dataGridView3.Rows.Count.ToString();
            else if (dataGridView4.Visible)
                label2.Text = "Количество записей: " + dataGridView4.Rows.Count.ToString();
        }

        private void FillGrid(MySqlConnection conn, string sql, DataGridView grid)
        {
            using (var cmd = new MySqlCommand(sql, conn))
            {
                var da = new MySqlDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);
                grid.DataSource = dt;
            }
        }

        private void SetupGrid(DataGridView grid, string idCol, string[] header)
        {
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.RowHeadersVisible = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;

            if (grid.Columns[idCol] != null) grid.Columns[idCol].Visible = false;
            if (grid.Columns[header[0]] != null) grid.Columns[header[0]].HeaderText = header[1];
            if (header.Length > 2 && grid.Columns[header[2]] != null) grid.Columns[header[2]].HeaderText = header[3];
        }

        private void ClearFields()
        {
            txtName.Text = "";
            txtPhone.Text = "";
            txtStatus.Text = "";
            txtPay.Text = "";
            txtCategory.Text = "";
            currentId = 0;
        }

        private void button4_Click(object sender, EventArgs e)
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
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название!"); return;
            }

            try
            {
                using (var conn = Connection.GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand();

                    if (dataGridView1.Visible)
                    {
                        cmd = new MySqlCommand("INSERT INTO Supplier (SupplierName, SupplierPhone) VALUES (@name, @phone)", conn);
                        cmd.Parameters.AddWithValue("@phone", txtPhone.Text);
                        cmd.Parameters.AddWithValue("@name", txtName.Text);
                    }
                    else if (dataGridView2.Visible)
                    {
                        cmd = new MySqlCommand("INSERT INTO Category (CategoryName) VALUES (@category)", conn);
                        cmd.Parameters.AddWithValue("@category", txtCategory.Text);
                    }
                    else if (dataGridView3.Visible)
                    {
                        cmd = new MySqlCommand("INSERT INTO MethodPayment (MethodPaymentName) VALUES (@pay)", conn);
                        cmd.Parameters.AddWithValue("@pay", txtPay.Text);
                    }
                    else if (dataGridView4.Visible)
                    {
                        cmd = new MySqlCommand("INSERT INTO Status (StatusName) VALUES (@status)", conn);
                        cmd.Parameters.AddWithValue("@status", txtStatus.Text);
                    }

                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Запись добавлена!");
                LoadAllData();
                ClearFields();
            }
            catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (currentId == 0) { MessageBox.Show("Выберите строку в таблице!"); return; }

            try
            {
                using (var conn = Connection.GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand();

                    if (dataGridView1.Visible)
                    {
                        cmd = new MySqlCommand("UPDATE Supplier SET SupplierName=@name, SupplierPhone=@phone WHERE SupplierID=@id", conn);
                        cmd.Parameters.AddWithValue("@phone", txtPhone.Text);
                    }
                    else if (dataGridView2.Visible)
                        cmd = new MySqlCommand("UPDATE Category SET CategoryName=@name WHERE CategoryID=@id", conn);
                    else if (dataGridView3.Visible)
                        cmd = new MySqlCommand("UPDATE MethodPayment SET MethodPaymentName=@name WHERE MethodPaymentID=@id", conn);
                    else if (dataGridView4.Visible)
                        cmd = new MySqlCommand("UPDATE Status SET StatusName=@name WHERE StatusID=@id", conn);

                    cmd.Parameters.AddWithValue("@name", txtName.Text);
                    cmd.Parameters.AddWithValue("@id", currentId);
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Запись изменена!");
                LoadAllData();
                ClearFields();
            }
            catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int idToDelete = 0;

            if (dataGridView1.Visible && dataGridView1.SelectedRows.Count > 0)
                idToDelete = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["SupplierID"].Value);
            else if (dataGridView2.Visible && dataGridView2.SelectedRows.Count > 0)
                idToDelete = Convert.ToInt32(dataGridView2.SelectedRows[0].Cells["CategoryID"].Value);
            else if (dataGridView3.Visible && dataGridView3.SelectedRows.Count > 0)
                idToDelete = Convert.ToInt32(dataGridView3.SelectedRows[0].Cells["MethodPaymentID"].Value);
            else if (dataGridView4.Visible && dataGridView4.SelectedRows.Count > 0)
                idToDelete = Convert.ToInt32(dataGridView4.SelectedRows[0].Cells["StatusID"].Value);

            if (idToDelete == 0)
            {
                MessageBox.Show("Выделите запись в таблице (одинарным кликом)!");
                return;
            }

            if (MessageBox.Show("Удалить выбранную запись?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    using (var conn = Connection.GetConnection())
                    {
                        conn.Open();
                        MySqlCommand cmd = new MySqlCommand();

                        if (dataGridView1.Visible)
                            cmd = new MySqlCommand("DELETE FROM Supplier WHERE SupplierID=@id", conn);
                        else if (dataGridView2.Visible)
                            cmd = new MySqlCommand("DELETE FROM Category WHERE CategoryID=@id", conn);
                        else if (dataGridView3.Visible)
                            cmd = new MySqlCommand("DELETE FROM MethodPayment WHERE MethodPaymentID=@id", conn);
                        else if (dataGridView4.Visible)
                            cmd = new MySqlCommand("DELETE FROM Status WHERE StatusID=@id", conn);

                        cmd.Parameters.AddWithValue("@id", idToDelete);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Запись удалена!");
                    LoadAllData();
                    ClearFields();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка удаления (возможно, запись используется в заказах):\n" + ex.Message);
                }
            }
        }

        private void DataGridViewCellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView grid = sender as DataGridView;
            if (e.RowIndex < 0) return;

            DataGridViewRow row = grid.Rows[e.RowIndex];

            if (grid == dataGridView1)
            {
                currentId = Convert.ToInt32(row.Cells["SupplierID"].Value);
                txtName.Text = row.Cells["SupplierName"].Value.ToString();
                txtPhone.Text = row.Cells["SupplierPhone"].Value.ToString();
            }
            else if (grid == dataGridView2)
            {
                currentId = Convert.ToInt32(row.Cells["CategoryID"].Value);
                txtCategory.Text = row.Cells["CategoryName"].Value.ToString();
            }
            else if (grid == dataGridView3)
            {
                currentId = Convert.ToInt32(row.Cells["MethodPaymentID"].Value);
                txtPay.Text = row.Cells["MethodPaymentName"].Value.ToString();
            }
            else if (grid == dataGridView4)
            {
                currentId = Convert.ToInt32(row.Cells["StatusID"].Value);
                txtStatus.Text = row.Cells["StatusName"].Value.ToString();
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateCountLabel();
        }
    }
}



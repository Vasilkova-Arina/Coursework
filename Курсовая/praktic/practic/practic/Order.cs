using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace practic
{
    public partial class Order : Form
    {
        private int currentOrderId = 0;
        private bool isAddingNew = false;

        public Order()
        {
            InitializeComponent();
            SetupGrid();
            LoadComboBoxes();
            LoadFilters();
            ClearFields(); 
        }

        private void SetupGrid()
        {
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        }

        private void LoadComboBoxes()
        {
            using (var conn = Connection.GetConnection())
            {
                conn.Open();

                var daUser = new MySqlDataAdapter("SELECT UserID, CONCAT(UserThreeName, ' ', UserFirstName, ' ', UserName) AS FIO FROM User", conn);
                var dtUser = new DataTable();
                daUser.Fill(dtUser);
                cmbSeller.DataSource = dtUser;
                cmbSeller.DisplayMember = "FIO";
                cmbSeller.ValueMember = "UserID";

                var daProd = new MySqlDataAdapter("SELECT ProductID, ProductName FROM Product", conn);
                var dtProd = new DataTable();
                daProd.Fill(dtProd);
                cmbProduct.DataSource = dtProd;
                cmbProduct.DisplayMember = "ProductName";
                cmbProduct.ValueMember = "ProductID";

                var daPay = new MySqlDataAdapter("SELECT MethodPaymentID, MethodPaymentName FROM MethodPayment", conn);
                var dtPay = new DataTable();
                daPay.Fill(dtPay);
                cmbPayMethod.DataSource = dtPay;
                cmbPayMethod.DisplayMember = "MethodPaymentName";
                cmbPayMethod.ValueMember = "MethodPaymentID";

                var daStat = new MySqlDataAdapter("SELECT StatusID, StatusName FROM Status", conn);
                var dtStat = new DataTable();
                daStat.Fill(dtStat);
                cmbStatus.DataSource = dtStat;
                cmbStatus.DisplayMember = "StatusName";
                cmbStatus.ValueMember = "StatusID";
            }
        }

        private void ClearFields()
        {
            cmbSeller.SelectedIndex = -1;
            cmbProduct.SelectedIndex = -1;
            cmbPayMethod.SelectedIndex = -1;
            cmbStatus.SelectedIndex = -1;

            txtCount.Text = "";
            txtSum.Text = "";

            currentOrderId = 0;
            isAddingNew = true;
        }

        private void LoadFilters()
        {
            combFil.Items.Clear();
            combFil.Items.Add("Все статусы");
            using (var conn = Connection.GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand("SELECT StatusName FROM Status", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        combFil.Items.Add(reader.GetString(0));
                    }
                }
            }
            combFil.SelectedIndex = 0;

            combCort.Items.Clear();
            combCort.Items.Add("Без сортировки");
            combCort.Items.Add("По способу оплаты (А-Я)");
            combCort.Items.Add("По способу оплаты (Я-А)");
            combCort.SelectedIndex = 0;
        }

        private void LoadData()
        {

            string cmdSQL = @"SELECT p.PaymentID, pr.ProductName, p.PaymentCountProduct, 
                                     CONCAT(u.UserThreeName, ' ', u.UserFirstName, ' ', u.UserName) AS FIO,
                                     mp.MethodPaymentName, p.PaymentSumma, st.StatusName,
                                     p.PaymentProduct, p.PaymentUser, p.PaymentMethod, p.PaymentStatus
                              FROM Payment p
                              JOIN Product pr ON pr.ProductID = p.PaymentProduct
                              JOIN User u ON u.UserID = p.PaymentUser
                              JOIN Status st ON st.StatusID = p.PaymentStatus
                              JOIN MethodPayment mp ON mp.MethodPaymentID = p.PaymentMethod
                              WHERE 1=1";

            int orderId = 0;
            string searchText = txtbPoick.Text.Trim();

            if (!string.IsNullOrEmpty(searchText) && searchText != "Поиск")
            {
                if (int.TryParse(searchText, out int parsedId))
                {
                    orderId = parsedId;
                    cmdSQL += " AND p.PaymentID = @orderId";
                }
                else
                {
                    cmdSQL += " AND pr.ProductName LIKE @searchText";
                }
            }

            if (combFil.SelectedIndex > 0)
            {
                cmdSQL += " AND st.StatusName = @statusName";
            }

            if (combCort.SelectedIndex == 1)
                cmdSQL += " ORDER BY mp.MethodPaymentName ASC";
            else if (combCort.SelectedIndex == 2)
                cmdSQL += " ORDER BY mp.MethodPaymentName DESC";
            else
                cmdSQL += " ORDER BY p.PaymentID";

            using (var conn = Connection.GetConnection())
            using (var cmd = new MySqlCommand(cmdSQL, conn))
            {
                if (orderId > 0)
                    cmd.Parameters.AddWithValue("@orderId", orderId);

                if (!string.IsNullOrEmpty(searchText) && searchText != "Поиск" && !int.TryParse(searchText, out _))
                    cmd.Parameters.AddWithValue("@searchText", "%" + searchText + "%");

                if (combFil.SelectedIndex > 0)
                    cmd.Parameters.AddWithValue("@statusName", combFil.SelectedItem.ToString());

                conn.Open();
                var da = new MySqlDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }

            if (dataGridView1.Columns.Count > 0)
            {
                dataGridView1.Columns["PaymentID"].HeaderText = "Номер";
                dataGridView1.Columns["ProductName"].HeaderText = "Товар";
                dataGridView1.Columns["PaymentCountProduct"].HeaderText = "Кол-во";
                dataGridView1.Columns["FIO"].HeaderText = "Пользователь";
                dataGridView1.Columns["MethodPaymentName"].HeaderText = "Способ оплаты";
                dataGridView1.Columns["PaymentSumma"].HeaderText = "Сумма";
                dataGridView1.Columns["StatusName"].HeaderText = "Статус";

                dataGridView1.Columns["PaymentProduct"].Visible = false;
                dataGridView1.Columns["PaymentUser"].Visible = false;
                dataGridView1.Columns["PaymentMethod"].Visible = false;
                dataGridView1.Columns["PaymentStatus"].Visible = false;
            }
            label3.Text = "Количество записей: " + dataGridView1.Rows.Count.ToString();
            PaginationHelper pager = new PaginationHelper(dataGridView1, this, 10);
            pager.Draw(800, 500);

        }


        private void button3_Click(object sender, EventArgs e)
        {
            SaveNewOrder();
            txtCount.Focus(); 
        }


        private void button4_Click(object sender, EventArgs e)
        {
            if (currentOrderId == 0)
            {
                MessageBox.Show("Выберите запись для изменения!");
                return;
            }

            try
            {
                string sql = @"UPDATE Payment SET 
                               PaymentProduct = @prod, PaymentCountProduct = @count, 
                               PaymentUser = @user, PaymentMethod = @method, 
                               PaymentSumma = @sum, PaymentStatus = @status 
                               WHERE PaymentID = @id";

                using (var conn = Connection.GetConnection())
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@prod", cmbProduct.SelectedValue);
                    cmd.Parameters.AddWithValue("@count", int.Parse(txtCount.Text));
                    cmd.Parameters.AddWithValue("@user", cmbSeller.SelectedValue);
                    cmd.Parameters.AddWithValue("@method", cmbPayMethod.SelectedValue);
                    cmd.Parameters.AddWithValue("@sum", double.Parse(txtSum.Text));
                    cmd.Parameters.AddWithValue("@status", cmbStatus.SelectedValue);
                    cmd.Parameters.AddWithValue("@id", currentOrderId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Заказ изменен!");
                LoadData();
            }
            catch (Exception ex) { MessageBox.Show("Ошибка изменения: " + ex.Message); }
        }

        private void SaveNewOrder()
        {
            try
            {
                string sql = @"INSERT INTO Payment (PaymentProduct, PaymentCountProduct, PaymentUser, 
                                PaymentMethod, PaymentSumma, PaymentStatus) 
                               VALUES (@prod, @count, @user, @method, @sum, @status)";

                using (var conn = Connection.GetConnection())
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@prod", cmbProduct.SelectedValue);
                    cmd.Parameters.AddWithValue("@count", int.Parse(txtCount.Text));
                    cmd.Parameters.AddWithValue("@user", cmbSeller.SelectedValue);
                    cmd.Parameters.AddWithValue("@method", cmbPayMethod.SelectedValue);
                    cmd.Parameters.AddWithValue("@sum", double.Parse(txtSum.Text));
                    cmd.Parameters.AddWithValue("@status", cmbStatus.SelectedValue);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Заказ добавлен!");
                LoadData();
                ClearFields();
            }
            catch (Exception ex) { MessageBox.Show("Ошибка добавления: " + ex.Message); }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выделите запись в таблице!");
                return;
            }

            int idToDelete = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["PaymentID"].Value);

            if (MessageBox.Show("Удалить выбранный заказ?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    using (var conn = Connection.GetConnection())
                    using (var cmd = new MySqlCommand("DELETE FROM Payment WHERE PaymentID = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idToDelete);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Заказ удален!");
                    currentOrderId = 0;
                    ClearFields();
                    LoadData();
                }
                catch (Exception ex) { MessageBox.Show("Ошибка удаления: " + ex.Message); }
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView1.Columns[e.ColumnIndex].Name == "StatusName")
            {
                string status = e.Value?.ToString();

                if (status == "Собран")
                {
                    e.CellStyle.BackColor = Color.LightGreen;
                    e.CellStyle.ForeColor = Color.Black;
                }
                else if (status == "В сборке")
                {
                    e.CellStyle.BackColor = Color.Orange;
                    e.CellStyle.ForeColor = Color.Black;
                }
                else if (status == "В ожидании")
                {
                    e.CellStyle.BackColor = Color.LightCoral;
                    e.CellStyle.ForeColor = Color.White;
                }
            }
        }

        private void txtbPoick_TextChanged_1(object sender, EventArgs e) { LoadData(); }
        private void combFil_SelectedIndexChanged_1(object sender, EventArgs e) { LoadData(); }
        private void combCort_SelectedIndexChanged(object sender, EventArgs e) { LoadData(); }

        private void txtbPoick_Enter_1(object sender, EventArgs e)
        {
            if (txtbPoick.Text == "Поиск") txtbPoick.Text = "";
        }

        private void txtbPoick_Leave_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtbPoick.Text)) txtbPoick.Text = "Поиск";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtbPoick.Text = "Поиск";
            if (combFil.Items.Count > 0) combFil.SelectedIndex = 0;
            if (combCort.Items.Count > 0) combCort.SelectedIndex = 0;
            LoadData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms.Count > 1)
                Application.OpenForms[Application.OpenForms.Count - 2].Show();
            this.Close();
        }


        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                isAddingNew = false;
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                currentOrderId = Convert.ToInt32(row.Cells["PaymentID"].Value);

                cmbProduct.SelectedValue = row.Cells["PaymentProduct"].Value;
                cmbSeller.SelectedValue = row.Cells["PaymentUser"].Value;
                cmbPayMethod.SelectedValue = row.Cells["PaymentMethod"].Value;
                cmbStatus.SelectedValue = row.Cells["PaymentStatus"].Value;

                txtCount.Text = row.Cells["PaymentCountProduct"].Value.ToString();
                txtSum.Text = row.Cells["PaymentSumma"].Value.ToString();
            }
        }

        private void Order_Load(object sender, EventArgs e)
        {
            
            
            if (Session.CurrentUser != null && Session.CurrentUser.UserRole == 4)
            {
                button7.Visible = true;  
            }
            else
            {
                button7.Visible = false; 
            }

            LoadData();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label10.Text = $"Добро пожаловать: {Session.CurrentUser.Fio} | {DateTime.Now:dd.MM.yyyy HH:mm:ss}";
        }

        private void txtCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtSum_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                return;
            }

            if (e.KeyChar == '.' || e.KeyChar == ',')
            {
                if (((TextBox)sender).Text.Contains(".") || ((TextBox)sender).Text.Contains(","))
                {
                    e.Handled = true;
                }
                else
                {
                    if (e.KeyChar == ',')
                    {
                        e.KeyChar = '.';
                    }
                }
                return;
            }

            e.Handled = true;
        }
    }
}
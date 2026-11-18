using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace practic
{
    public partial class User : Form
    {
        private int currentOrderId = 0;
        private bool isAddingNew = false;
        private string cmdStr =
            @"SELECT u.UserID,
                u.UserThreeName,
                u.UserFirstName,
                u.UserName,
                u.UserPhone,
                u.UserDateBirthday,
                u.UserLogin,
                u.UserPassword,
                r.RoleName
            FROM `User` u
            JOIN Role r ON r.RoleID = u.UserRole;";
        public User()
        {
            InitializeComponent();
            SetupGrid();
            LoadComboBox();
        }



        private void User_Load(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            comboBox2.Items.Add("Без сортировки");
            comboBox2.Items.Add("По роли (А-Я)");
            comboBox2.Items.Add("По роли (Я-А)");
            comboBox2.SelectedIndex = 0;

            comboBox1.Items.Clear();
            comboBox1.Items.Add("Все года");

            int currentYear = DateTime.Now.Year;

            for (int year = currentYear - 18; year >= currentYear - 80; year--)
            {
                comboBox1.Items.Add(year.ToString());
            }
            comboBox1.SelectedIndex = 0;

            dateBirthd.MaxDate = DateTime.Today.AddYears(-18); 
            dateBirthd.MinDate = DateTime.Today.AddYears(-80); 
            dateBirthd.Value = DateTime.Today.AddYears(-18); 

            textBox1.TextChanged += (s, ev) => LoadData();
            comboBox1.SelectedIndexChanged += (s, ev) => LoadData();
            comboBox2.SelectedIndexChanged += (s, ev) => LoadData();

            LoadData();
        }
        private void LoadComboBox()
        {
            using (var conn = Connection.GetConnection())
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(cmdStr, conn))
                {
                    var daRole = new MySqlDataAdapter("SELECT RoleID, RoleName FROM Role", conn);
                    var dtRole = new DataTable();
                    daRole.Fill(dtRole);
                    cmbRole.DataSource = dtRole;
                    cmbRole.DisplayMember = "RoleName";
                    cmbRole.ValueMember = "RoleID";
                }
            }
        }

        private void LoadData()
        {
            string sql = @"
                    SELECT u.UserID, u.UserThreeName, u.UserFirstName, u.UserName, 
                    u.UserPhone, u.UserDateBirthday, u.UserLogin, u.UserPassword, 
                    u.UserRole, r.RoleName
                    FROM `User` u
                    JOIN Role r ON r.RoleID = u.UserRole
                    WHERE 1=1";

            string searchText = textBox1.Text.Trim();
            if (!string.IsNullOrEmpty(searchText) && searchText != "Поиск")
            {
                sql += " AND (u.UserThreeName LIKE @search OR u.UserFirstName LIKE @search OR u.UserName LIKE @search)";
            }

            if (comboBox1.SelectedIndex > 0)
            {
                sql += " AND YEAR(u.UserDateBirthday) = @year";
            }

            if (comboBox2.Text == "По роли (А-Я)")
                sql += " ORDER BY r.RoleName ASC";
            else if (comboBox2.Text == "По роли (Я-А)")
                sql += " ORDER BY r.RoleName DESC";
            else
                sql += " ORDER BY u.UserID";

            using (var conn = Connection.GetConnection())
            using (var cmd = new MySqlCommand(sql, conn))
            {
                if (!string.IsNullOrEmpty(searchText) && searchText != "Поиск")
                    cmd.Parameters.AddWithValue("@search", "%" + searchText + "%");

                if (comboBox1.SelectedIndex > 0)
                    cmd.Parameters.AddWithValue("@year", comboBox1.SelectedItem.ToString());

                conn.Open();
                var da = new MySqlDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }

            if (dataGridView1.Columns.Count > 0)
            {
                dataGridView1.Columns["UserID"].Visible = false;
                dataGridView1.Columns["UserRole"].Visible = false;
                dataGridView1.Columns["UserThreeName"].HeaderText = "Фамилия";
                dataGridView1.Columns["UserFirstName"].HeaderText = "Имя";
                dataGridView1.Columns["UserName"].HeaderText = "Отчество";
                dataGridView1.Columns["UserPhone"].HeaderText = "Телефон";
                dataGridView1.Columns["UserDateBirthday"].HeaderText = "Дата рождения";
                dataGridView1.Columns["UserLogin"].HeaderText = "Логин";
                dataGridView1.Columns["UserPassword"].HeaderText = "Пароль";
                dataGridView1.Columns["RoleName"].HeaderText = "Роль";

                dataGridView1.Columns["UserDateBirthday"].DefaultCellStyle.Format = "dd.MM.yyyy";
            }

            label2.Text = "Количество записей: " + dataGridView1.Rows.Count.ToString();
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var col = dataGridView1.Columns[e.ColumnIndex].Name;

            if ((col == "UserLogin" || col == "UserPassword") && e.Value != null)
            {
                e.Value = new string('*', e.Value.ToString().Length);
                e.FormattingApplied = true;
            }

            if (col == "UserPhone" && e.Value != null)
            {
                string phone = e.Value.ToString();

                if (phone.Length > 7)
                {

                    string masked = phone.Substring(0, 4) + "****" + phone.Substring(phone.Length - 2);

                    if (phone.Length >= 10)
                    {
                        string last10 = phone.Substring(phone.Length - 10);
                        masked = last10.Substring(0, 3) + "****" + last10.Substring(last10.Length - 2);
                    }

                    e.Value = masked;
                    e.FormattingApplied = true;
                }
            }
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

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void txtFIO_Leave(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt == null || string.IsNullOrWhiteSpace(txt.Text)) return;

            System.Globalization.TextInfo textInfo = new System.Globalization.CultureInfo("ru-RU", false).TextInfo;
            txt.Text = textInfo.ToTitleCase(txt.Text.ToLower());
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text)) textBox1.Text = "Поиск";
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "Поиск") textBox1.Text = "";
        }

        private void textBox1_TextChanged(object sender, EventArgs e){ LoadData(); }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { LoadData(); }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) { LoadData(); }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox1.Text = "Поиск";
            if (comboBox1.Items.Count > 0) comboBox1.SelectedIndex = 0;
            if (comboBox2.Items.Count > 0) comboBox2.SelectedIndex = 0;
            LoadData();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выделите запись в таблице!");
                return;
            }
            int idToDelete = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["UserID"].Value);
            if (MessageBox.Show("Удалить выбранный пользователя?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    using (var conn = Connection.GetConnection())
                    using (var cmd = new MySqlCommand("DELETE FROM User WHERE UserID = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idToDelete);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Пользователь удален!");
                    currentOrderId = 0;
                    ClearFields();
                    LoadData();
                }
                catch (Exception ex) { MessageBox.Show("Ошибка удаления: " + ex.Message); }
            }
        }
        private void ClearFields()
        {
            cmbRole.SelectedIndex = -1;
            dateBirthd.Value = DateTime.Now;

            txtFIO.Text = "";
            txtPhone.Text = "";
            txtLogin.Text = "";
            txtPass.Text = "";

            currentOrderId = 0;
            isAddingNew = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveNewOrder();
            txtFIO.Focus();
        }
        private void SaveNewOrder()
        {
            try
            {
                string sql = @"INSERT INTO `User` 
                                    (UserThreeName, UserFirstName, UserName, UserPhone, UserDateBirthday, UserLogin, UserPassword, UserRole) 
                                    VALUES (@lastName, @firstName, @middleName, @phone, @birthday, @login, @password, @role)";
                using (var conn = Connection.GetConnection())
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    string fio = txtFIO.Text.Trim();

                    string[] fioParts = fio.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    string lastName = fioParts.Length > 0 ? fioParts[0] : "";
                    string firstName = fioParts.Length > 1 ? fioParts[1] : "";
                    string middleName = fioParts.Length > 2 ? fioParts[2] : "";

                    cmd.Parameters.AddWithValue("@lastName", lastName);      
                    cmd.Parameters.AddWithValue("@firstName", firstName);  
                    cmd.Parameters.AddWithValue("@middleName", middleName);  
                    cmd.Parameters.AddWithValue("@phone", txtPhone.Text); 
                    cmd.Parameters.AddWithValue("@birthday", dateBirthd.Value); 
                    cmd.Parameters.AddWithValue("@login", txtLogin.Text);   
                    cmd.Parameters.AddWithValue("@password", txtPass.Text);  
                    cmd.Parameters.AddWithValue("@role", cmbRole.SelectedValue);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Пользователь добавлен!");
                LoadData();
                ClearFields();
            }
            catch (Exception ex) { MessageBox.Show("Ошибка добавления: " + ex.Message); }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label3.Text = $"Добро пожаловать: {Session.CurrentUser.Fio} | {DateTime.Now:dd.MM.yyyy HH:mm:ss}";
        }

        private void txtFIO_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;

            bool isRussianLetter = (e.KeyChar >= 'А' && e.KeyChar <= 'я') || e.KeyChar == 'ё' || e.KeyChar == 'Ё';
            bool isSpace = e.KeyChar == ' ';
            bool isHyphen = e.KeyChar == '-';

            if (!isRussianLetter && !isSpace && !isHyphen)
            {
                e.Handled = true; 
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (currentOrderId == 0)
            {
                MessageBox.Show("Выберите пользователя из таблицы (кликните по строке)!");
                return;
            }

            try
            {
                string sql = @"UPDATE `User` SET 
                   UserThreeName = @lastName, 
                   UserFirstName = @firstName, 
                   UserName = @middleName, 
                   UserPhone = @phone, 
                   UserDateBirthday = @birthday, 
                   UserLogin = @login, 
                   UserPassword = @password, 
                   UserRole = @role 
                   WHERE UserID = @id";

                string[] fioParts = txtFIO.Text.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (fioParts.Length < 2)
                {
                    MessageBox.Show("Введите хотя бы Фамилию и Имя!");
                    return;
                }

                using (var conn = Connection.GetConnection())
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@lastName", fioParts.Length > 0 ? fioParts[0] : "");
                    cmd.Parameters.AddWithValue("@firstName", fioParts.Length > 1 ? fioParts[1] : "");
                    cmd.Parameters.AddWithValue("@middleName", fioParts.Length > 2 ? fioParts[2] : "");

                    cmd.Parameters.AddWithValue("@phone", txtPhone.Text);
                    cmd.Parameters.AddWithValue("@birthday", dateBirthd.Value); 
                    cmd.Parameters.AddWithValue("@login", txtLogin.Text);
                    cmd.Parameters.AddWithValue("@password", txtPass.Text);
                    cmd.Parameters.AddWithValue("@role", cmbRole.SelectedValue);

                    cmd.Parameters.AddWithValue("@id", currentOrderId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Данные пользователя успешно обновлены!");
                LoadData(); 
                ClearFields(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка изменения: " + ex.Message);
            }
        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            isAddingNew = false;
            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

            currentOrderId = Convert.ToInt32(row.Cells["UserID"].Value);

            string lastName = row.Cells["UserThreeName"].Value?.ToString() ?? "";
            string firstName = row.Cells["UserFirstName"].Value?.ToString() ?? "";
            string middleName = row.Cells["UserName"].Value?.ToString() ?? "";
            txtFIO.Text = $"{lastName} {firstName} {middleName}".Trim();

            txtPhone.Text = row.Cells["UserPhone"].Value?.ToString() ?? "";
            txtLogin.Text = row.Cells["UserLogin"].Value?.ToString() ?? "";
            txtPass.Text = row.Cells["UserPassword"].Value?.ToString() ?? "";

            if (row.Cells["UserDateBirthday"].Value != DBNull.Value)
            {
                dateBirthd.Value = Convert.ToDateTime(row.Cells["UserDateBirthday"].Value);
            }

            string roleName = row.Cells["RoleName"].Value.ToString();
            cmbRole.SelectedIndex = cmbRole.FindStringExact(roleName);

        }
    }
}

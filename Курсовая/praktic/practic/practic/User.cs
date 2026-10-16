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
    public partial class User : Form
    {
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
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void User_Load(object sender, EventArgs e)
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
            dataGridView1.Columns["UserID"].Visible = false;
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

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var col = dataGridView1.Columns[e.ColumnIndex].Name;

            if ((col == "UserLogin" || col == "UserPassword") && e.Value != null)
            {
                e.Value = new string('●', e.Value.ToString().Length);
                e.FormattingApplied = true;
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
    }
}

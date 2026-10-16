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
    public partial class Order : Form
    {
        private string cmdStr =
        @"SELECT 
             p.PaymentID,
             pr.ProductName,
             p.PaymentCountProduct,
             CONCAT(u.UserFirstName, ' ', u.UserName, ' ', u.UserThreeName) AS FIO,
             mp.MethodPaymentName,
             p.PaymentSumma,
             st.StatusName
        FROM Payment p
        JOIN Product pr       ON pr.ProductID       = p.PaymentProduct
        JOIN User u           ON u.UserID           = p.PaymentUser
        JOIN MethodPayment mp ON mp.MethodPaymentID = p.PaymentMethod
        JOIN Status st        ON st.StatusID        = p.PaymentStatus;";
        public Order()
        {
            InitializeComponent();
            SetupGrid();
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

        private void Order_Load(object sender, EventArgs e)
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
            dataGridView1.Columns["PaymentID"].Visible = false;
            dataGridView1.Columns["ProductName"].HeaderText = "Товар";
            dataGridView1.Columns["PaymentCountProduct"].HeaderText = "Кол-во";
            dataGridView1.Columns["FIO"].HeaderText = "Пользователь";
            dataGridView1.Columns["MethodPaymentName"].HeaderText = "Способ оплаты";
            dataGridView1.Columns["PaymentSumma"].HeaderText = "Сумма";
            dataGridView1.Columns["StatusName"].HeaderText = "Статус";
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
    }
}

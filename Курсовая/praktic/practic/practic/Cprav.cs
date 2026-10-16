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
        private string cmdSupplier = "SELECT * FROM Supplier;";
        private string cmdCategory = "SELECT * FROM Category;";
        private string cmdMethodPayment = "SELECT * FROM MethodPayment;";
        private string cmdStatus = "SELECT * FROM Status;";
        public Cprav()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Cprav_Load(object sender, EventArgs e)
        {
            using (var conn = Connection.GetConnection())
            {
                conn.Open();

                FillGrid(conn, cmdSupplier, dataGridView1);
                FillGrid(conn, cmdCategory, dataGridView2);
                FillGrid(conn, cmdMethodPayment, dataGridView3);
                FillGrid(conn, cmdStatus, dataGridView4);
            }
            dataGridView1.Columns["SupplierID"].Visible = false;
            dataGridView1.Columns["SupplierName"].HeaderText = "Поставщик";
            dataGridView1.Columns["SupplierPhone"].HeaderText = "Телефон";

            dataGridView2.Columns["CategoryID"].Visible = false;
            dataGridView2.Columns["CategoryName"].HeaderText = "Категория";

            dataGridView3.Columns["MethodPaymentID"].Visible = false;
            dataGridView3.Columns["MethodPaymentName"].HeaderText = "Способ оплаты";

            dataGridView4.Columns["StatusID"].Visible = false;
            dataGridView4.Columns["StatusName"].HeaderText = "Статус";
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

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
    

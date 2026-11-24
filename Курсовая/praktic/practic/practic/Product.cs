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
        private bool isLoading = true;
        private int currentOrderId = 0;
        private bool isAddingNew = false;
        private string oldPhotoName = "glysh.jpg";
        private string newSelectedPhotoPath = null;

        public Product()
        {
            InitializeComponent();
            SetupGrid();
            LoadFilters();
            LoadComboBoxes();
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
            dataGridView1.Columns["ProductPhoto"].HeaderText = "Фото";


            isLoading = false;
            LoadData();

        }
        private void LoadComboBoxes()
        {
            using (var conn = Connection.GetConnection())
            {
                conn.Open();

                var daCategory = new MySqlDataAdapter("SELECT CategoryID, CategoryName FROM Category", conn);
                var dtCategory = new DataTable();
                daCategory.Fill(dtCategory);
                cmbCategory.DataSource = dtCategory;
                cmbCategory.DisplayMember = "CategoryName";
                cmbCategory.ValueMember = "CategoryID";

                var daSup = new MySqlDataAdapter("SELECT SupplierID, SupplierName FROM Supplier", conn);
                var dtSup = new DataTable();
                daSup.Fill(dtSup);
                cmbSallery.DataSource = dtSup;
                cmbSallery.DisplayMember = "SupplierName";
                cmbSallery.ValueMember = "SupplierID";
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

            DataTable dt = new DataTable();
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
                da.Fill(dt);
            }

            dataGridView1.DataSource = null;
            dataGridView1.Columns.Clear();

            dataGridView1.DataSource = dt;

            if (dataGridView1.Columns.Count > 0)
            {
                if (dataGridView1.Columns.Contains("ProductPhoto"))
                    dataGridView1.Columns.Remove("ProductPhoto");

                DataGridViewImageColumn imgCol = new DataGridViewImageColumn();
                imgCol.Name = "ProductImage";
                imgCol.HeaderText = "Фото";
                imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
                imgCol.Width = 100;
                dataGridView1.Columns.Add(imgCol);

                dataGridView1.Columns["ProductID"].Visible = false;
                dataGridView1.Columns["ProductName"].HeaderText = "Название товара";
                dataGridView1.Columns["ProductDecription"].HeaderText = "Описание";
                dataGridView1.Columns["ProductSale"].HeaderText = "Цена";
                dataGridView1.Columns["ProductCount"].HeaderText = "Кол-во";
                dataGridView1.Columns["CategoryName"].HeaderText = "Категория";
                dataGridView1.Columns["SupplierName"].HeaderText = "Поставщик";

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    DataRowView dataRow = row.DataBoundItem as DataRowView;
                    if (dataRow == null) continue;

                    string photoName = dataRow["ProductPhoto"]?.ToString();

                    string folderPath = System.IO.Path.Combine(Application.StartupPath, "Resources", "picture");

                    string fullPath = string.IsNullOrEmpty(photoName) ? "" : System.IO.Path.Combine(folderPath, photoName);

                    if (!System.IO.File.Exists(fullPath))
                    {
                        fullPath = System.IO.Path.Combine(folderPath, "glysh.jpg");
                    }

                    if (System.IO.File.Exists(fullPath))
                    {
                        using (var fs = new System.IO.FileStream(fullPath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                        {
                            row.Cells["ProductImage"].Value = Image.FromStream(fs);
                        }
                    }
                }
            }

            label3.Text = "Количество записей: " + dataGridView1.Rows.Count.ToString();

            this.BeginInvoke(new Action(() =>
            {
                PaginationHelper pager = new PaginationHelper(dataGridView1, this, 10);
                pager.Draw(800, 600);
            }));
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
            if (isLoading) return;
            LoadData();
        }

        private void cmbFil_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoading) return;
            LoadData();
        }

        private void cmbSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoading) return;
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

        private void button3_Click(object sender, EventArgs e)
        {
            SaveNewOrder();
            txtName.Focus();
        }
        private void SaveNewOrder()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Введите название товара!"); return;
                }

                string photoName = "glysh.jpg"; // По умолчанию глушилка

                if (newSelectedPhotoPath != null && System.IO.File.Exists(newSelectedPhotoPath))
                {
                    photoName = System.IO.Path.GetFileName(newSelectedPhotoPath);

                    string folderPath = System.IO.Path.Combine(Application.StartupPath, "Resources", "picture");

                    if (!System.IO.Directory.Exists(folderPath))
                        System.IO.Directory.CreateDirectory(folderPath);

                    string fullPath = System.IO.Path.Combine(folderPath, photoName);

                    System.IO.File.Copy(newSelectedPhotoPath, fullPath, true);
                }

                string sql = @"INSERT INTO Product (ProductName, ProductDecription, ProductSale, 
                    ProductCategory, ProductSupplier, ProductPhoto, ProductCount) 
                   VALUES (@name, @desc, @price, @category, @supplier, @photo, @count)";

                using (var conn = Connection.GetConnection())
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@name", txtName.Text);
                    cmd.Parameters.AddWithValue("@desc", txtDecription.Text);

                    double price = 0;
                    double.TryParse(txtPrice.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out price);
                    cmd.Parameters.AddWithValue("@price", price);

                    cmd.Parameters.AddWithValue("@category", cmbCategory.SelectedValue);
                    cmd.Parameters.AddWithValue("@supplier", cmbSallery.SelectedValue);
                    cmd.Parameters.AddWithValue("@photo", photoName);

                    cmd.Parameters.AddWithValue("@count", txtCount.Text);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Товар успешно добавлен!");
                LoadData();
                ClearFields();
            }

            catch (Exception ex)
            {
                MessageBox.Show("Ошибка добавления: " + ex.Message);
            }

        }
        private void ClearFields()
        {
            cmbCategory.SelectedIndex = -1;
            cmbSallery.SelectedIndex = -1;

            txtCount.Text = "";
            txtName.Text = "";
            txtDecription.Text = "";
            txtPrice.Text = "";

            newSelectedPhotoPath = null; 
            oldPhotoName = "glysh.jpg";

            currentOrderId = 0;
            isAddingNew = true;
        }


        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void pictureProduct_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                openFileDialog.Title = "Выберите картинку для товара";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    pictureProduct.Image = Image.FromFile(openFileDialog.FileName);
                    pictureProduct.SizeMode = PictureBoxSizeMode.Zoom;

                    newSelectedPhotoPath = openFileDialog.FileName;
                }
                else
                {
                    string glyshPath = System.IO.Path.Combine(Application.StartupPath, "Resources", "picture", "glysh.jpg");
                    if (System.IO.File.Exists(glyshPath))
                    {
                        pictureProduct.Image = Image.FromFile(glyshPath);
                        pictureProduct.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                    newSelectedPhotoPath = null; 
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (currentOrderId == 0)
            {
                MessageBox.Show("Выберите товар из таблицы (кликните по строке)!");
                return;
            }

            try
            {
                string photoName = oldPhotoName;

                if (newSelectedPhotoPath != null && System.IO.File.Exists(newSelectedPhotoPath))
                {
                    photoName = System.IO.Path.GetFileName(newSelectedPhotoPath);

                    string folderPath = System.IO.Path.Combine(Application.StartupPath, "Resources", "picture");

                    if (!System.IO.Directory.Exists(folderPath))
                        System.IO.Directory.CreateDirectory(folderPath);

                    string fullPath = System.IO.Path.Combine(folderPath, photoName);

                    System.IO.File.Copy(newSelectedPhotoPath, fullPath, true);
                }
                else
                {
                    photoName = oldPhotoName;
                }

                string sql = @"UPDATE Product SET 
                   ProductName = @name, 
                   ProductDecription = @desc, 
                   ProductSale = @price, 
                   ProductCategory = @category, 
                   ProductSupplier = @supplier, 
                   ProductPhoto = @photo, 
                   ProductCount = @count 
                   WHERE ProductID = @id";

                using (var conn = Connection.GetConnection())
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@name", txtName.Text);
                    cmd.Parameters.AddWithValue("@desc", txtDecription.Text);

                    double price = 0;
                    double.TryParse(txtPrice.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out price);
                    cmd.Parameters.AddWithValue("@price", price);

                    cmd.Parameters.AddWithValue("@category", cmbCategory.SelectedValue);
                    cmd.Parameters.AddWithValue("@supplier", cmbSallery.SelectedValue);
                    cmd.Parameters.AddWithValue("@photo", photoName);

                    cmd.Parameters.AddWithValue("@count", txtCount.Text);

                    cmd.Parameters.AddWithValue("@id", currentOrderId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Товар успешно изменен!");
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
            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

            currentOrderId = Convert.ToInt32(row.Cells["ProductID"].Value);
            txtName.Text = row.Cells["ProductName"].Value.ToString();
            txtDecription.Text = row.Cells["ProductDecription"].Value.ToString();
            txtPrice.Text = row.Cells["ProductSale"].Value.ToString();
            txtCount.Text = row.Cells["ProductCount"].Value.ToString();

            cmbCategory.Text = row.Cells["CategoryName"].Value.ToString(); 
            cmbSallery.Text = row.Cells["SupplierName"].Value.ToString();  

            DataRowView dataRow = row.DataBoundItem as DataRowView;
            if (dataRow != null)
            {
                oldPhotoName = dataRow["ProductPhoto"]?.ToString() ?? "glysh.jpg";

                string folderPath = System.IO.Path.Combine(Application.StartupPath, "Resources", "picture");
                string fullPath = System.IO.Path.Combine(folderPath, oldPhotoName);

                if (!System.IO.File.Exists(fullPath))
                    fullPath = System.IO.Path.Combine(folderPath, "glysh.jpg");

                if (System.IO.File.Exists(fullPath))
                {
                    pictureProduct.Image = Image.FromFile(fullPath);
                    pictureProduct.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выделите запись в таблице!");
                return;
            }

            int idToDelete = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ProductID"].Value);

            if (MessageBox.Show("Удалить выбранный товар?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    using (var conn = Connection.GetConnection())
                    using (var cmd = new MySqlCommand("DELETE FROM Product WHERE ProductID = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idToDelete);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Товар удален!");
                    currentOrderId = 0;
                    ClearFields();
                    LoadData();
                }
                catch (Exception ex) { MessageBox.Show("Ошибка удаления: " + ex.Message); }
            }
        }

        private void txtCount_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;

            bool isRussian = (e.KeyChar >= 'А' && e.KeyChar <= 'я') || e.KeyChar == 'ё' || e.KeyChar == 'Ё';
            bool isEnglish = (e.KeyChar >= 'a' && e.KeyChar <= 'z') || (e.KeyChar >= 'A' && e.KeyChar <= 'Z');
            bool isSpace = e.KeyChar == ' ';
            bool isHyphen = e.KeyChar == '-';

            if (!isRussian && !isEnglish && !isSpace && !isHyphen)
            {
                e.Handled = true; 
            }
        }

        private void txtDecription_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;

            bool isRussian = (e.KeyChar >= 'А' && e.KeyChar <= 'я') || e.KeyChar == 'ё' || e.KeyChar == 'Ё';
            bool isSpace = e.KeyChar == ' ';
            bool isHyphen = e.KeyChar == '-';

            if (!isRussian && !isSpace && !isHyphen)
            {
                e.Handled = true; 
            }
        }

        private void txtCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; 
            }
        }

        private void txtPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
                return;

            if (e.KeyChar == '.' || e.KeyChar == ',')
            {
                if (((TextBox)sender).Text.Contains(".") || ((TextBox)sender).Text.Contains(","))
                {
                    e.Handled = true;
                }
                else
                {
                    if (e.KeyChar == ',') e.KeyChar = '.';
                }
                return;
            }

            e.Handled = true;
        }
    }
}
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
    public partial class Main : Form
    {
        private int count = 0;
        private string capthaOtv = "";
        private bool viewCaptcha = false;


        private readonly Random rnd = new Random();
        
        public Main()
        {
            InitializeComponent();
            lblCptcha.Visible = pictCptcha.Visible = txtCaptcha.Visible = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void SetFormEnabled(bool enabled)
        {
            foreach (Control c in this.Controls)
                SetControlEnabled(c, enabled);
        }

        private void SetControlEnabled(Control c, bool enabled)
        {
            c.Enabled = enabled;
            foreach (Control child in c.Controls)
                SetControlEnabled(child, enabled);
        }

        private async void BlockFor10Seconds()
        {
            SetFormEnabled(false);

            time.Visible = true;
            time.Text = "Подождите 10 сек";

            for (int i = 10; i > 0; i--)
            {
                time.Text = "Подождите " + i + " сек";
                await Task.Delay(1000);
                Application.DoEvents();
            }

            time.Text = "";
            time.Visible = false;

            SetFormEnabled(true);
            txtCaptcha.Clear();
            log.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string login = log.Text.Trim();
            string pass = pwd.Text;

            //проверяем капчу если она есть
            if (viewCaptcha && txtCaptcha.Text.Trim() != capthaOtv)
            {
                Fail();
                return;
            }

            //проверяем логин и пароль
            int roleId = CheckUser(login, pass);

            if (roleId == -1)
            {
                Fail();
                return;
            }

            count = 0;

            //открываем форму по роли
            OpenFormByRole(roleId);
            this.Hide();
        }

        private void Fail()
        {
            count++;
            MessageBox.Show("Неверные данные для входа.");

            if (count == 1 && !viewCaptcha)
            {
                viewCaptcha = true;
                lblCptcha.Visible = pictCptcha.Visible = txtCaptcha.Visible = true;
                GenerateCaptcha();
                return;
            }

            if (count > 1)
            {
                GenerateCaptcha();
                BlockFor10Seconds();
            }
        }

        private void GenerateCaptcha()
        {
            int n = rnd.Next(1, 5); 

            string path = System.IO.Path.Combine(
                Application.StartupPath, "Resources", "Captcha" + n + ".png");

            try
            {
                pictCptcha.Image = Image.FromFile(path);
            }
            catch (System.IO.FileNotFoundException)
            {
                MessageBox.Show("Не найдена картинка капчи: " + path);
                return;
            }

            pictCptcha.SizeMode = PictureBoxSizeMode.StretchImage;

            capthaOtv = new[] { "", "smwm", "good morning", "L88rB6", "OGBMbY4" }[n];
            txtCaptcha.Clear();
        }

        private int CheckUser(string login, string pass)
        {
            using (var cn = Connection.GetConnection())
            using (var cmd = new MySqlCommand(
                @"SELECT u.UserID, u.UserFirstName, u.UserName, u.UserThreeName, u.UserRole
          FROM `user` u
          WHERE u.UserLogin = @l AND u.UserPassword = @p", cn))
            {
                cmd.Parameters.AddWithValue("@l", login);
                cmd.Parameters.AddWithValue("@p", pass);
                cn.Open();

                using (var rd = cmd.ExecuteReader())
                {
                    if (!rd.Read()) return -1;

                    Session.CurrentUser = new Users
                    {
                        UserID = rd.GetInt32(0),
                        UserFirstName = rd.GetString(1),
                        UserName = rd.GetString(2),
                        UserThreeName = rd.GetString(3),
                        UserRole = rd.GetInt32(4)
                    };

                    return Session.CurrentUser.UserRole; 
                }
            }
        }
        private void OpenFormByRole(int roleId)
        {
            Form f;
            switch (roleId)
            {
                case 1: f = new MenuAdmin(); break; // admin
                case 2: f = new MenuMened(); break; // manager
                case 3: f = new MenuClient(); break; //tovar
                case 4: f = new MenuDirector(); break; //direct
                case 5: f = new MenuCass(); break; //cassir
                default:
                    MessageBox.Show("Для этой роли форма ещё не создана.");
                    return;
            }
            f.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

    }
}

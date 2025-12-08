using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Web.UI;

namespace Lab5
{
    public partial class Register : Page
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["TaxiDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblMessage.Text = "";
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            // Серверная валидация
            if (!Page.IsValid)
                return;

            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text.Trim();
            string name = txtName.Text.Trim();
            string surname = txtSurname.Text.Trim();
            string phone = txtPhone.Text.Trim();

            // Минимальная проверка
            if (login.Length == 0 || password.Length == 0)
            {
                lblMessage.Text = "Логин и пароль обязательны.";
                return;
            }

            try
            {
                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    // Проверяем, не занят ли логин
                    string checkSql = "SELECT COUNT(1) FROM Passenger WHERE passengerLogin = @login";
                    using (var checkCmd = new MySqlCommand(checkSql, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@login", login);
                        long exists = Convert.ToInt64(checkCmd.ExecuteScalar());
                        if (exists > 0)
                        {
                            lblMessage.Text = "Пользователь с таким логином уже существует. Выберите другой логин.";
                            return;
                        }
                    }

                    // Вставляем пассажира и возвращаем ID
                    string insertSql = @"
INSERT INTO Passenger (passengerLogin, passengerPassword, passengerName, passengerSurname, phoneNumber)
VALUES (@login, @pass, @name, @surname, @phone);
SELECT LAST_INSERT_ID();";

                    using (var insertCmd = new MySqlCommand(insertSql, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@login", login);
                        insertCmd.Parameters.AddWithValue("@pass", password); // рекомендую хешировать пароли в будущем
                        insertCmd.Parameters.AddWithValue("@name", string.IsNullOrEmpty(name) ? (object)DBNull.Value : name);
                        insertCmd.Parameters.AddWithValue("@surname", string.IsNullOrEmpty(surname) ? (object)DBNull.Value : surname);
                        insertCmd.Parameters.AddWithValue("@phone", string.IsNullOrEmpty(phone) ? (object)DBNull.Value : phone);

                        object newIdObj = insertCmd.ExecuteScalar();
                        if (newIdObj != null)
                        {
                            int newId = Convert.ToInt32(newIdObj);

                            // Автоматический логин: создаём сессию и редиректим на PassengerHome
                            Session["userID"] = newId;
                            Session["role"] = "passenger";

                            Response.Redirect("~/PassengerHome.aspx", false);
                            Context.ApplicationInstance.CompleteRequest();
                            return;
                        }
                        else
                        {
                            lblMessage.Text = "Ошибка при регистрации. Попробуйте ещё раз.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // По желанию логировать ex.Message
                lblMessage.Text = "Ошибка при подключении к базе данных.";
            }
        }

        protected void btnBackToLogin_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Login.aspx");
        }

    }
}

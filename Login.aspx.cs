using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Web.UI;

namespace Lab5
{
    public partial class Login : Page
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["TaxiDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblMessage.Text = "";
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            // Простая серверная валидация
            if (!Page.IsValid) return;

            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text.Trim();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    string query = "SELECT passengerID FROM Passenger WHERE passengerLogin=@login AND passengerPassword=@pass LIMIT 1";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@login", login);
                        cmd.Parameters.AddWithValue("@pass", password);

                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            Session["userID"] = result;
                            Session["role"] = "passenger";

                            Response.Redirect("~/PassengerHome.aspx", false);
                            Context.ApplicationInstance.CompleteRequest();
                        }
                        else
                        {
                            lblMessage.Text = "Неверный логин или пароль пассажира.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Логирование сюда при необходимости
                lblMessage.Text = "Ошибка при подключении к базе данных.";
            }
        }

        protected void btnWorkerLogin_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text.Trim();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    string query = "SELECT workerID FROM Worker WHERE workerLogin=@login AND workerPassword=@pass LIMIT 1";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@login", login);
                        cmd.Parameters.AddWithValue("@pass", password);

                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            Session["workerID"] = result;
                            Session["role"] = "worker";

                            Response.Redirect("~/WorkerHome.aspx", false);
                            Context.ApplicationInstance.CompleteRequest();
                        }
                        else
                        {
                            lblMessage.Text = "Неверный логин или пароль работника.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Ошибка при подключении к базе данных.";
            }
        }
    }
}

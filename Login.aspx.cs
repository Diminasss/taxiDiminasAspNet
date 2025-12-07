using MySql.Data.MySqlClient;
using System;
using System.Configuration;

namespace Lab5
{
    public partial class Login : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["TaxiDB"].ConnectionString;
        // string connStr = "server=localhost;database=YOUR_DB;uid=YOUR_USER;pwd=YOUR_PASSWORD;charset=utf8;";

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text.Trim();

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();

                string query = "SELECT passengerID FROM Passenger WHERE passengerLogin=@login AND passengerPassword=@pass LIMIT 1";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@login", login);
                cmd.Parameters.AddWithValue("@pass", password);

                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    // успешный вход как пассажир
                    Session["userID"] = result;
                    Session["role"] = "passenger";

                    Response.Redirect("~/PassengerHome.aspx");
                }
                else
                {
                    lblMessage.Text = "Неверный логин или пароль пассажира.";
                }
            }
        }


        protected void btnWorkerLogin_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text.Trim();

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();

                string query = "SELECT workerID FROM Worker WHERE workerLogin=@login AND workerPassword=@pass LIMIT 1";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@login", login);
                cmd.Parameters.AddWithValue("@pass", password);

                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    // успешный вход как работник
                    Session["workerID"] = result;
                    Session["role"] = "worker";

                    Response.Redirect("~/WorkerHome.aspx");
                }
                else
                {
                    lblMessage.Text = "Неверный логин или пароль работника.";
                }
            }
        }
    }
}

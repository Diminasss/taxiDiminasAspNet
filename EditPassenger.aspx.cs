using System;
using System.Web;
using System.Web.UI;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace Lab5
{
    public partial class EditPassenger : Page
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["TaxiDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userID"] == null || (Session["role"] == null || Session["role"].ToString() != "passenger"))
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadPassengerData();
            }
        }

        private void LoadPassengerData()
        {
            try
            {
                int pid = Convert.ToInt32(Session["userID"]);

                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string query = "SELECT passengerLogin, passengerName, passengerSurname, phoneNumber FROM Passenger WHERE passengerID=@pid LIMIT 1";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@pid", pid);
                        using (var r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                            {
                                txtLogin.Text = r.IsDBNull(r.GetOrdinal("passengerLogin")) ? "" : r.GetString("passengerLogin");
                                txtName.Text = r.IsDBNull(r.GetOrdinal("passengerName")) ? "" : r.GetString("passengerName");
                                txtSurname.Text = r.IsDBNull(r.GetOrdinal("passengerSurname")) ? "" : r.GetString("passengerSurname");
                                txtPhone.Text = r.IsDBNull(r.GetOrdinal("phoneNumber")) ? "" : r.GetString("phoneNumber");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Ошибка при загрузке данных.";
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                int pid = Convert.ToInt32(Session["userID"]);

                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string query = @"UPDATE Passenger SET 
                                     passengerLogin=@login,
                                     passengerPassword=@pass,
                                     passengerName=@name,
                                     passengerSurname=@surname,
                                     phoneNumber=@phone
                                     WHERE passengerID=@pid";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@login", txtLogin.Text.Trim());
                        cmd.Parameters.AddWithValue("@pass", txtPassword.Text.Trim());
                        cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                        cmd.Parameters.AddWithValue("@surname", txtSurname.Text.Trim());
                        cmd.Parameters.AddWithValue("@phone", txtPhone.Text.Trim());
                        cmd.Parameters.AddWithValue("@pid", pid);

                        cmd.ExecuteNonQuery();
                    }
                }

                // Возврат в ЛК
                Response.Redirect("~/PassengerHome.aspx");
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Ошибка при сохранении данных.";
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/PassengerHome.aspx");
        }
    }
}

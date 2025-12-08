using System;
using System.Web;
using System.Web.UI;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace Lab5
{
    public partial class NewTrip : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userID"] == null || (Session["role"] == null || Session["role"].ToString() != "passenger"))
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadCities();
                LoadStreets();
                // optionally set default select to 0 (placeholder)
            }
        }

        private MySqlConnection GetConn()
        {
            string cs = ConfigurationManager.ConnectionStrings["TaxiDB"].ConnectionString;
            return new MySqlConnection(cs);
        }

        private void LoadCities()
        {
            try
            {
                using (var conn = GetConn())
                {
                    conn.Open();
                    string q = "SELECT cityID, city FROM City ORDER BY city";
                    using (var cmd = new MySqlCommand(q, conn))
                    using (var r = cmd.ExecuteReader())
                    {
                        ddlFromCity.Items.Clear();
                        ddlToCity.Items.Clear();

                        ddlFromCity.Items.Add(new System.Web.UI.WebControls.ListItem("-- выберите город --", "0"));
                        ddlToCity.Items.Add(new System.Web.UI.WebControls.ListItem("-- выберите город --", "0"));

                        while (r.Read())
                        {
                            string name = r.IsDBNull(1) ? "" : r.GetString("city");
                            string id = r.GetInt32("cityID").ToString();
                            ddlFromCity.Items.Add(new System.Web.UI.WebControls.ListItem(HttpUtility.HtmlEncode(name), id));
                            ddlToCity.Items.Add(new System.Web.UI.WebControls.ListItem(HttpUtility.HtmlEncode(name), id));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Ошибка при загрузке списка городов.";
            }
        }

        private void LoadStreets()
        {
            try
            {
                using (var conn = GetConn())
                {
                    conn.Open();
                    string q = "SELECT streetID, street FROM Street ORDER BY street";
                    using (var cmd = new MySqlCommand(q, conn))
                    using (var r = cmd.ExecuteReader())
                    {
                        ddlFromStreet.Items.Clear();
                        ddlToStreet.Items.Clear();

                        ddlFromStreet.Items.Add(new System.Web.UI.WebControls.ListItem("-- выберите улицу --", "0"));
                        ddlToStreet.Items.Add(new System.Web.UI.WebControls.ListItem("-- выберите улицу --", "0"));

                        while (r.Read())
                        {
                            string name = r.IsDBNull(1) ? "" : r.GetString("street");
                            string id = r.GetInt32("streetID").ToString();
                            ddlFromStreet.Items.Add(new System.Web.UI.WebControls.ListItem(HttpUtility.HtmlEncode(name), id));
                            ddlToStreet.Items.Add(new System.Web.UI.WebControls.ListItem(HttpUtility.HtmlEncode(name), id));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Ошибка при загрузке списка улиц.";
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/PassengerHome.aspx");
        }

        protected void btnCreateTrip_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";

            // Простейшая серверная валидация
            if (ddlFromCity.SelectedValue == "0" || ddlFromStreet.SelectedValue == "0" || string.IsNullOrWhiteSpace(txtFromBuilding.Text) ||
                ddlToCity.SelectedValue == "0" || ddlToStreet.SelectedValue == "0" || string.IsNullOrWhiteSpace(txtToBuilding.Text))
            {
                lblMessage.Text = "Заполните все поля формы.";
                return;
            }

            int passengerId = Convert.ToInt32(Session["userID"]);
            int fromCity = Convert.ToInt32(ddlFromCity.SelectedValue);
            int fromStreet = Convert.ToInt32(ddlFromStreet.SelectedValue);
            string fromBuilding = txtFromBuilding.Text.Trim();

            int toCity = Convert.ToInt32(ddlToCity.SelectedValue);
            int toStreet = Convert.ToInt32(ddlToStreet.SelectedValue);
            string toBuilding = txtToBuilding.Text.Trim();

            try
            {
                using (var conn = GetConn())
                {
                    conn.Open();

                    // 1) Получаем или создаём адреса
                    int fromAddressId = GetOrCreateAddress(conn, fromCity, fromStreet, fromBuilding);
                    int toAddressId = GetOrCreateAddress(conn, toCity, toStreet, toBuilding);

                    // 2) Вставляем Trip
                    // Предполагаем, что statusID = 1 соответствует "Requested" (заказан).
                    string insertTrip = @"
INSERT INTO Trip
    (startDateTime, endDateTime, cost, feedbackID, fromAddressID, toAddressID, statusID, passengerID, driverID)
VALUES
    (NOW(), NULL, 0.00, NULL, @fromAddr, @toAddr, @statusId, @pid, NULL);
SELECT LAST_INSERT_ID();";

                    using (var cmd = new MySqlCommand(insertTrip, conn))
                    {
                        cmd.Parameters.AddWithValue("@fromAddr", fromAddressId);
                        cmd.Parameters.AddWithValue("@toAddr", toAddressId);
                        cmd.Parameters.AddWithValue("@statusId", 1); // при необходимости изменить
                        cmd.Parameters.AddWithValue("@pid", passengerId);

                        object o = cmd.ExecuteScalar();
                        long tripId = o != null ? Convert.ToInt64(o) : 0;

                        if (tripId > 0)
                        {
                            // Успех — редирект в личный кабинет
                            Response.Redirect("~/PassengerHome.aspx");
                            return;
                        }
                        else
                        {
                            lblMessage.Text = "Не удалось создать заказ. Попробуйте ещё раз.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // логировать ex по желанию
                lblMessage.Text = "Ошибка при создании заказа: " + HttpUtility.HtmlEncode(ex.Message);
            }
        }

        private int GetOrCreateAddress(MySqlConnection conn, int cityID, int streetID, string building)
        {
            // Ищем существующий адрес
            string find = "SELECT addressID FROM Address WHERE cityID=@cityID AND streetID=@streetID AND building=@building LIMIT 1;";
            using (var cmd = new MySqlCommand(find, conn))
            {
                cmd.Parameters.AddWithValue("@cityID", cityID);
                cmd.Parameters.AddWithValue("@streetID", streetID);
                cmd.Parameters.AddWithValue("@building", building);
                object o = cmd.ExecuteScalar();
                if (o != null)
                {
                    return Convert.ToInt32(o);
                }
            }

            // Если не найден — вставляем
            string insert = "INSERT INTO Address (cityID, streetID, building) VALUES (@cityID, @streetID, @building); SELECT LAST_INSERT_ID();";
            using (var cmd = new MySqlCommand(insert, conn))
            {
                cmd.Parameters.AddWithValue("@cityID", cityID);
                cmd.Parameters.AddWithValue("@streetID", streetID);
                cmd.Parameters.AddWithValue("@building", building);
                object oi = cmd.ExecuteScalar();
                return oi != null ? Convert.ToInt32(oi) : 0;
            }
        }
    }
}

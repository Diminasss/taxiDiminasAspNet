using System;
using System.Data;
using System.Web;
using System.Web.UI;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace Lab5
{
    public partial class AddOrder : Page
    {
        private string ConnStr => ConfigurationManager.ConnectionStrings["TaxiDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Проверяем, что вошёл работник и он диспетчер/администратор (positionID = 2)
            if (Session["workerID"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                // проверим роль в БД — если не админ, редирект
                int workerId = Convert.ToInt32(Session["workerID"]);
                if (!IsAdmin(workerId))
                {
                    Response.Redirect("~/WorkerHome.aspx");
                    return;
                }

                LoadPassengers();
                LoadCities(ddlFromCity);
                LoadCities(ddlToCity);

                // при начальной загрузке наполним списки улиц (если есть выбранные города)
                if (ddlFromCity.Items.Count > 0 && ddlFromCity.SelectedValue != "")
                    LoadStreets(Convert.ToInt32(ddlFromCity.SelectedValue), ddlFromStreet);
                if (ddlToCity.Items.Count > 0 && ddlToCity.SelectedValue != "")
                    LoadStreets(Convert.ToInt32(ddlToCity.SelectedValue), ddlToStreet);
            }
        }

        private MySqlConnection GetConn() => new MySqlConnection(ConnStr);

        private bool IsAdmin(int workerId)
        {
            try
            {
                using (var conn = GetConn())
                {
                    conn.Open();
                    string sql = "SELECT positionID FROM Worker WHERE workerID = @w LIMIT 1";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@w", workerId);
                        object o = cmd.ExecuteScalar();
                        if (o == null) return false;
                        int pos = Convert.ToInt32(o);
                        return pos == 2; // 2 — Administrator (по структуре БД)
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        private void LoadPassengers()
        {
            ddlPassenger.Items.Clear();
            ddlPassenger.Items.Add(new System.Web.UI.WebControls.ListItem("-- выберите пассажира --", ""));

            try
            {
                using (var conn = GetConn())
                {
                    conn.Open();
                    string sql = "SELECT passengerID, passengerName, passengerSurname FROM Passenger ORDER BY passengerName, passengerSurname";
                    using (var cmd = new MySqlCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32("passengerID");
                            string name = reader.IsDBNull(reader.GetOrdinal("passengerName")) ? "" : reader.GetString("passengerName");
                            string surname = reader.IsDBNull(reader.GetOrdinal("passengerSurname")) ? "" : reader.GetString("passengerSurname");
                            ddlPassenger.Items.Add(new System.Web.UI.WebControls.ListItem(HttpUtility.HtmlEncode($"{name} {surname}"), id.ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.CssClass = "text-danger";
                lblMessage.Text = "Ошибка при загрузке списка пассажиров.";
            }
        }

        private void LoadCities(System.Web.UI.WebControls.DropDownList ddl)
        {
            ddl.Items.Clear();
            ddl.Items.Add(new System.Web.UI.WebControls.ListItem("-- выберите город --", ""));

            try
            {
                using (var conn = GetConn())
                {
                    conn.Open();
                    string sql = "SELECT cityID, city FROM City ORDER BY city";
                    using (var cmd = new MySqlCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32("cityID");
                            string city = reader.IsDBNull(reader.GetOrdinal("city")) ? "" : reader.GetString("city");
                            ddl.Items.Add(new System.Web.UI.WebControls.ListItem(HttpUtility.HtmlEncode(city), id.ToString()));
                        }
                    }
                }
            }
            catch
            {
                // если нет городов, оставим пустой список
            }
        }

        private void LoadStreets(int cityId, System.Web.UI.WebControls.DropDownList ddl)
        {
            ddl.Items.Clear();
            ddl.Items.Add(new System.Web.UI.WebControls.ListItem("-- выберите улицу --", ""));

            try
            {
                using (var conn = GetConn())
                {
                    conn.Open();
                    // Попробуем сначала выбрать улицы, которые связаны с адресами в этом городе (часто полезнее)
                    string sql = @"
                        SELECT DISTINCT s.streetID, s.street
                        FROM Street s
                        JOIN Address a ON a.streetID = s.streetID
                        WHERE a.cityID = @city
                        ORDER BY s.street;
                    ";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@city", cityId);
                        using (var r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                ddl.Items.Add(new System.Web.UI.WebControls.ListItem(HttpUtility.HtmlEncode(r.GetString("street")), r.GetInt32("streetID").ToString()));
                            }
                        }
                    }

                    // Если не нашлось — подгрузим все улицы
                    if (ddl.Items.Count == 1) // только заглушка
                    {
                        string sql2 = "SELECT streetID, street FROM Street ORDER BY street";
                        using (var cmd2 = new MySqlCommand(sql2, conn))
                        using (var r2 = cmd2.ExecuteReader())
                        {
                            while (r2.Read())
                            {
                                ddl.Items.Add(new System.Web.UI.WebControls.ListItem(HttpUtility.HtmlEncode(r2.GetString("street")), r2.GetInt32("streetID").ToString()));
                            }
                        }
                    }
                }
            }
            catch
            {
                // игнорируем ошибки — оставим заглушку
            }
        }

        protected void ddlFromCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (int.TryParse(ddlFromCity.SelectedValue, out int cityId))
            {
                LoadStreets(cityId, ddlFromStreet);
            }
            else
            {
                ddlFromStreet.Items.Clear();
                ddlFromStreet.Items.Add(new System.Web.UI.WebControls.ListItem("-- выберите улицу --", ""));
            }
        }

        protected void ddlToCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (int.TryParse(ddlToCity.SelectedValue, out int cityId))
            {
                LoadStreets(cityId, ddlToStreet);
            }
            else
            {
                ddlToStreet.Items.Clear();
                ddlToStreet.Items.Add(new System.Web.UI.WebControls.ListItem("-- выберите улицу --", ""));
            }
        }

        protected void btnCreateOrder_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            lblMessage.CssClass = "";

            // Простая валидация на сервере (дублирует контролы в разметке)
            if (string.IsNullOrEmpty(ddlPassenger.SelectedValue) ||
                string.IsNullOrEmpty(ddlFromCity.SelectedValue) || string.IsNullOrEmpty(ddlFromStreet.SelectedValue) ||
                string.IsNullOrEmpty(ddlToCity.SelectedValue) || string.IsNullOrEmpty(ddlToStreet.SelectedValue))
            {
                lblMessage.CssClass = "text-danger";
                lblMessage.Text = "Заполните все обязательные поля.";
                return;
            }

            int passengerId = Convert.ToInt32(ddlPassenger.SelectedValue);
            int fromCityId = Convert.ToInt32(ddlFromCity.SelectedValue);
            int fromStreetId = Convert.ToInt32(ddlFromStreet.SelectedValue);
            string fromBuilding = string.IsNullOrWhiteSpace(txtFromBuilding.Text) ? null : txtFromBuilding.Text.Trim();

            int toCityId = Convert.ToInt32(ddlToCity.SelectedValue);
            int toStreetId = Convert.ToInt32(ddlToStreet.SelectedValue);
            string toBuilding = string.IsNullOrWhiteSpace(txtToBuilding.Text) ? null : txtToBuilding.Text.Trim();

            try
            {
                using (var conn = GetConn())
                {
                    conn.Open();

                    // Вставляем адреса (можно оптимизировать — сначала искать существующий адрес, но для простоты вставляем новый)
                    int fromAddressId = InsertAddress(conn, fromCityId, fromStreetId, fromBuilding);
                    int toAddressId = InsertAddress(conn, toCityId, toStreetId, toBuilding);

                    // Вставка поездки со статусом 1 (Requested)
                    string insertTripSql = @"
                        INSERT INTO Trip (startDateTime, endDateTime, cost, feedbackID, fromAddressID, toAddressID, statusID, passengerID, driverID)
                        VALUES (@start, NULL, NULL, NULL, @fromAddr, @toAddr, @statusID, @pid, NULL);
                        SELECT LAST_INSERT_ID();";

                    using (var cmd = new MySqlCommand(insertTripSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@start", DateTime.Now);
                        cmd.Parameters.AddWithValue("@fromAddr", fromAddressId);
                        cmd.Parameters.AddWithValue("@toAddr", toAddressId);
                        cmd.Parameters.AddWithValue("@statusID", 1); // Requested
                        cmd.Parameters.AddWithValue("@pid", passengerId);

                        object tripObj = cmd.ExecuteScalar();
                        if (tripObj != null)
                        {
                            int newTripId = Convert.ToInt32(tripObj);
                            lblMessage.CssClass = "text-success";
                            lblMessage.Text = $"Заказ создан (ID = {newTripId}). Он доступен для принятия водителями.";
                            // очистим форму
                            ddlPassenger.SelectedIndex = 0;
                            ddlFromCity.SelectedIndex = 0;
                            ddlFromStreet.Items.Clear(); ddlFromStreet.Items.Add(new System.Web.UI.WebControls.ListItem("-- выберите улицу --", ""));
                            txtFromBuilding.Text = "";
                            ddlToCity.SelectedIndex = 0;
                            ddlToStreet.Items.Clear(); ddlToStreet.Items.Add(new System.Web.UI.WebControls.ListItem("-- выберите улицу --", ""));
                            txtToBuilding.Text = "";
                        }
                        else
                        {
                            lblMessage.CssClass = "text-danger";
                            lblMessage.Text = "Не удалось создать заказ. Попробуйте ещё раз.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.CssClass = "text-danger";
                lblMessage.Text = "Ошибка при создании заказа.";
            }
        }

        private int InsertAddress(MySqlConnection conn, int cityId, int streetId, string building)
        {
            using (var cmd = new MySqlCommand("INSERT INTO Address (cityID, streetID, building) VALUES (@city, @street, @building); SELECT LAST_INSERT_ID();", conn))
            {
                cmd.Parameters.AddWithValue("@city", cityId);
                cmd.Parameters.AddWithValue("@street", streetId);
                if (string.IsNullOrWhiteSpace(building))
                    cmd.Parameters.AddWithValue("@building", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@building", building);
                object o = cmd.ExecuteScalar();
                return Convert.ToInt32(o);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/WorkerHome.aspx");
        }
    }
}

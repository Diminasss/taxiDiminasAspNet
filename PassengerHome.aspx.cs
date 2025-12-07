using System;
using System.Web;
using System.Web.UI;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace Lab5
{
    public partial class PassengerHome : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Проверяем сессию — пассажир должен быть в системе
            if (Session["userID"] == null || (Session["role"] == null || Session["role"].ToString() != "passenger"))
            {
                // не залогинен — перенаправляем на логин
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadPassengerName();
                LoadCurrentTrip();
            }
        }

        private MySqlConnection GetConn()
        {
            string cs = ConfigurationManager.ConnectionStrings["TaxiDB"].ConnectionString;
            return new MySqlConnection(cs);
        }

        private void LoadPassengerName()
        {
            try
            {
                int pid = Convert.ToInt32(Session["userID"]);

                using (var conn = GetConn())
                {
                    conn.Open();
                    string q = "SELECT passengerName, passengerSurname FROM Passenger WHERE passengerID=@pid LIMIT 1";
                    using (var cmd = new MySqlCommand(q, conn))
                    {
                        cmd.Parameters.AddWithValue("@pid", pid);
                        using (var r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                            {
                                string name = r.IsDBNull(0) ? "" : r.GetString("passengerName");
                                string surname = r.IsDBNull(1) ? "" : r.GetString("passengerSurname");
                                lblUserName.Text = HttpUtility.HtmlEncode($"{name} {surname}");
                            }
                            else
                            {
                                lblUserName.Text = "Пользователь";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblUserName.Text = "Пользователь";
                // логирование можно добавить
            }
        }

        private void LoadCurrentTrip()
        {
            // Находим последнюю активную поездку пассажира (Requested/Accepted/InProgress)
            try
            {
                int pid = Convert.ToInt32(Session["userID"]);

                using (var conn = GetConn())
                {
                    conn.Open();

                    string query = @"
SELECT
  t.tripID, t.startDateTime, t.endDateTime, t.cost,
  s.statusName, s.statusID,
  fa.building AS fromBuilding, fcity.city AS fromCity, fstreet.street AS fromStreet,
  ta.building AS toBuilding, tcity.city AS toCity, tstreet.street AS toStreet,
  w.workerName, w.workerSurname,
  c.stateNumber, b.brandName, m.modelName
FROM Trip t
LEFT JOIN Status s ON t.statusID = s.statusID
LEFT JOIN Address fa ON t.fromAddressID = fa.addressID
LEFT JOIN City fcity ON fa.cityID = fcity.cityID
LEFT JOIN Street fstreet ON fa.streetID = fstreet.streetID
LEFT JOIN Address ta ON t.toAddressID = ta.addressID
LEFT JOIN City tcity ON ta.cityID = tcity.cityID
LEFT JOIN Street tstreet ON ta.streetID = tstreet.streetID
LEFT JOIN Driver d ON t.driverID = d.driverID
LEFT JOIN Worker w ON d.workerID = w.workerID
LEFT JOIN Car c ON d.carID = c.carID
LEFT JOIN Brand b ON c.carBrandID = b.carBrandID
LEFT JOIN Model m ON c.carModelID = m.carModelID
WHERE t.passengerID = @pid
  AND s.statusID IN (1,2,3)
ORDER BY t.startDateTime DESC
LIMIT 1;";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@pid", pid);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                // нет активной поездки
                                lblTripNotFound.Text = "У вас нет активной поездки. Нажмите «Заказать поездку», чтобы создать новую.";
                                tripDetails.Visible = false;
                                lblTripNotFound.Visible = true;
                            }
                            else
                            {
                                reader.Read();
                                int statusID = reader.IsDBNull(reader.GetOrdinal("statusID")) ? 0 : reader.GetInt32("statusID");
                                string statusName = reader.IsDBNull(reader.GetOrdinal("statusName")) ? "Неизвестно" : reader.GetString("statusName");

                                string start = reader.IsDBNull(reader.GetOrdinal("startDateTime")) ? "-" : reader.GetDateTime("startDateTime").ToString("g");
                                string end = reader.IsDBNull(reader.GetOrdinal("endDateTime")) ? "-" : reader.GetDateTime("endDateTime").ToString("g");
                                string cost = reader.IsDBNull(reader.GetOrdinal("cost")) ? "-" : reader.GetDecimal("cost").ToString("C2");

                                string fromAddr = FormatAddress(reader, "fromCity", "fromStreet", "fromBuilding");
                                string toAddr = FormatAddress(reader, "toCity", "toStreet", "toBuilding");

                                string driver = (reader.IsDBNull(reader.GetOrdinal("workerName")) ? "" : reader.GetString("workerName")) + " " +
                                                (reader.IsDBNull(reader.GetOrdinal("workerSurname")) ? "" : reader.GetString("workerSurname"));
                                driver = driver.Trim();
                                if (string.IsNullOrEmpty(driver)) driver = "Пока не назначен";

                                string car = "";
                                if (!reader.IsDBNull(reader.GetOrdinal("brandName")) || !reader.IsDBNull(reader.GetOrdinal("modelName")) || !reader.IsDBNull(reader.GetOrdinal("stateNumber")))
                                {
                                    string brand = reader.IsDBNull(reader.GetOrdinal("brandName")) ? "" : reader.GetString("brandName");
                                    string model = reader.IsDBNull(reader.GetOrdinal("modelName")) ? "" : reader.GetString("modelName");
                                    string plate = reader.IsDBNull(reader.GetOrdinal("stateNumber")) ? "" : reader.GetString("stateNumber");
                                    car = $"{brand} {model} {plate}".Trim();
                                }
                                if (string.IsNullOrEmpty(car)) car = "Информация отсутствует";

                                // Заполняем контролы
                                lblStatus.Text = HttpUtility.HtmlEncode(statusName);
                                lblStart.Text = HttpUtility.HtmlEncode(start);
                                lblEnd.Text = HttpUtility.HtmlEncode(end);
                                lblCost.Text = HttpUtility.HtmlEncode(cost);

                                lblFrom.Text = HttpUtility.HtmlEncode(fromAddr);
                                lblTo.Text = HttpUtility.HtmlEncode(toAddr);
                                lblDriver.Text = HttpUtility.HtmlEncode(driver);
                                lblCar.Text = HttpUtility.HtmlEncode(car);

                                // Показываем блок с деталями
                                tripDetails.Visible = true;
                                lblTripNotFound.Visible = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Ошибка при получении данных о поездке.";
                tripDetails.Visible = false;
                lblTripNotFound.Visible = false;
                // При необходимости можно логировать ex
            }
        }

        private string FormatAddress(MySqlDataReader r, string cityCol, string streetCol, string buildingCol)
        {
            string city = r.IsDBNull(r.GetOrdinal(cityCol)) ? "" : r.GetString(cityCol);
            string street = r.IsDBNull(r.GetOrdinal(streetCol)) ? "" : r.GetString(streetCol);
            string building = r.IsDBNull(r.GetOrdinal(buildingCol)) ? "" : r.GetString(buildingCol);

            string[] parts = new string[] { city, street, building };
            return string.Join(", ", Array.FindAll(parts, p => !string.IsNullOrWhiteSpace(p)));
        }

        protected void btnNewTrip_Click(object sender, EventArgs e)
        {
            // Перейдём на страницу создания новой поездки (создай NewTrip.aspx)
            Response.Redirect("~/NewTrip.aspx");
        }

        protected void btnHistory_Click(object sender, EventArgs e)
        {
            // Перейдём на страницу истории поездок (создай TripHistory.aspx)
            Response.Redirect("~/TripHistory.aspx");
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadCurrentTrip();
        }
    }
}

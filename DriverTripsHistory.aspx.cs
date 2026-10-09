using System;
using System.Data;
using System.Web;
using System.Web.UI;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace Lab5
{
    public partial class DriverTripsHistory : Page
    {
        private string ConnStr => ConfigurationManager.ConnectionStrings["TaxiDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["workerID"] == null)
                {
                    Response.Redirect("~/Login.aspx");
                    return;
                }

                LoadData();
            }
        }

        private MySqlConnection GetConn() => new MySqlConnection(ConnStr);

        private void LoadData()
        {
            try
            {
                int workerID = Convert.ToInt32(Session["workerID"]);
                int driverID = GetDriverID(workerID);
                if (driverID == -1)
                {
                    pnlNotDriver.Visible = true;
                    pnlContent.Visible = false;
                    return;
                }

                pnlNotDriver.Visible = false;
                pnlContent.Visible = true;

                // 1) Суммы
                decimal today = GetEarningsToday(driverID);
                decimal month = GetEarningsMonth(driverID);

                lblTodaySum.Text = today.ToString("C2");
                lblMonthSum.Text = month.ToString("C2");

                // 2) Таблица — все поездки водителя
                BindTripsGrid(driverID);

                lblInfo.Text = $"Показаны все поездки водителя (ID = {driverID}). Всего за сегодня: {lblTodaySum.Text}, за месяц: {lblMonthSum.Text}.";
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Ошибка при загрузке истории поездок.";
                // можно залогировать ex.Message при необходимости
            }
        }

        private int GetDriverID(int workerID)
        {
            try
            {
                using (var conn = GetConn())
                {
                    conn.Open();
                    string sql = "SELECT driverID FROM Driver WHERE workerID = @w LIMIT 1";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@w", workerID);
                        object res = cmd.ExecuteScalar();
                        return res == null ? -1 : Convert.ToInt32(res);
                    }
                }
            }
            catch
            {
                return -1;
            }
        }

        private decimal GetEarningsToday(int driverID)
        {
            try
            {
                using (var conn = GetConn())
                {
                    conn.Open();
                    string sql = @"
SELECT COALESCE(SUM(cost),0) 
FROM Trip
WHERE driverID = @driver AND statusID = 3 AND DATE(endDateTime) = CURDATE();";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@driver", driverID);
                        object o = cmd.ExecuteScalar();
                        return o == DBNull.Value || o == null ? 0m : Convert.ToDecimal(o);
                    }
                }
            }
            catch
            {
                return 0m;
            }
        }

        private decimal GetEarningsMonth(int driverID)
        {
            try
            {
                using (var conn = GetConn())
                {
                    conn.Open();
                    string sql = @"
SELECT COALESCE(SUM(cost),0)
FROM Trip
WHERE driverID = @driver AND statusID = 3
  AND YEAR(endDateTime) = YEAR(CURDATE()) AND MONTH(endDateTime) = MONTH(CURDATE());";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@driver", driverID);
                        object o = cmd.ExecuteScalar();
                        return o == DBNull.Value || o == null ? 0m : Convert.ToDecimal(o);
                    }
                }
            }
            catch
            {
                return 0m;
            }
        }

        private void BindTripsGrid(int driverID)
        {
            try
            {
                using (var conn = GetConn())
                {
                    conn.Open();

                    string sql = @"
SELECT
  t.tripID,
  t.startDateTime,
  t.endDateTime,
  t.cost,
  s.statusName,
  CONCAT(fc.city, ', ', fst.street, ' ', IFNULL(fa.building,'')) AS fromAddress,
  CONCAT(tc.city, ', ', tst.street, ' ', IFNULL(ta.building,'')) AS toAddress,
  CONCAT(p.passengerName, ' ', p.passengerSurname) AS passengerName,
  f.mark AS feedbackMark
FROM Trip t
LEFT JOIN Status s ON t.statusID = s.statusID
LEFT JOIN Address fa ON t.fromAddressID = fa.addressID
LEFT JOIN City fc ON fa.cityID = fc.cityID
LEFT JOIN Street fst ON fa.streetID = fst.streetID
LEFT JOIN Address ta ON t.toAddressID = ta.addressID
LEFT JOIN City tc ON ta.cityID = tc.cityID
LEFT JOIN Street tst ON ta.streetID = tst.streetID
LEFT JOIN Passenger p ON t.passengerID = p.passengerID
LEFT JOIN Feedback f ON t.feedbackID = f.feedbackID
WHERE t.driverID = @driver
ORDER BY t.startDateTime DESC;";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@driver", driverID);
                        using (var da = new MySqlDataAdapter(cmd))
                        {
                            var dt = new DataTable();
                            da.Fill(dt);
                            // feedbackMark может быть null -> заменить на '-'
                            if (!dt.Columns.Contains("feedbackMark"))
                                dt.Columns.Add("feedbackMark");
                            foreach (DataRow r in dt.Rows)
                            {
                                if (r.IsNull("feedbackMark"))
                                    r["feedbackMark"] = DBNull.Value;
                            }

                            gvTrips.DataSource = dt;
                            gvTrips.DataBind();
                        }
                    }
                }
            }
            catch
            {
                lblMessage.Text = "Ошибка при получении списка поездок.";
            }
        }

        protected void gvTrips_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            gvTrips.PageIndex = e.NewPageIndex;
            // повторно биндим (LoadData выполнен на PostBack при первой загрузке, но нам надо получить driverID снова)
            try
            {
                int workerID = Convert.ToInt32(Session["workerID"]);
                int driverID = GetDriverID(workerID);
                if (driverID != -1) BindTripsGrid(driverID);
            }
            catch { }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/WorkerHome.aspx");
        }
    }
}

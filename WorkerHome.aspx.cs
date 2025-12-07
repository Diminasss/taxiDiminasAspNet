using System;
using System.Data;
using System.Web;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace Lab5
{
    public partial class WorkerHome : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["workerID"] == null)
                {
                    Response.Redirect("~/Login.aspx");
                    return;
                }

                LoadWorkerInfo();
            }
        }

        private MySqlConnection GetConn()
        {
            string cs = ConfigurationManager.ConnectionStrings["TaxiDB"].ConnectionString;
            return new MySqlConnection(cs);
        }

        private void LoadWorkerInfo()
        {
            try
            {
                int workerID = Convert.ToInt32(Session["workerID"]);

                using (var conn = GetConn())
                {
                    conn.Open();

                    string sql = @"
                        SELECT w.workerName, w.workerSurname, cp.positionName, w.positionID
                        FROM Worker w
                        JOIN CareerPosition cp ON w.positionID = cp.positionID
                        WHERE w.workerID = @id
                        LIMIT 1";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", workerID);

                        using (var r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                            {
                                string name = (r.IsDBNull(r.GetOrdinal("workerName")) ? "" : r.GetString("workerName"));
                                string surname = (r.IsDBNull(r.GetOrdinal("workerSurname")) ? "" : r.GetString("workerSurname"));
                                lblWorkerName.Text = HttpUtility.HtmlEncode($"{name} {surname}");
                                lblWorkerRole.Text = HttpUtility.HtmlEncode(r.GetString("positionName"));

                                int positionID = r.IsDBNull(r.GetOrdinal("positionID")) ? 0 : r.GetInt32("positionID");

                                DriverPanel.Visible = (positionID == 1);
                                DispatcherPanel.Visible = (positionID == 2);

                                if (positionID == 1)
                                {
                                    LoadOrders(); // для водителя: доступные заказы
                                }

                                if (positionID == 2)
                                {
                                    LoadDispatcherOrders(); // для диспетчера: активные заказы
                                }
                            }
                            else
                            {
                                // Не нашли — выходим
                                Session.Abandon();
                                Response.Redirect("~/Login.aspx");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Ошибка при загрузке информации о работнике.";
                // логирование ex можно добавить
            }
        }

        // ===========================
        // Для водителя: загрузка доступных заказов (statusID = 1)
        // ===========================
        private void LoadOrders()
        {
            try
            {
                using (var conn = GetConn())
                {
                    conn.Open();

                    string sql = @"
                        SELECT 
                            t.tripID,
                            CONCAT(fc.city, ', ', fst.street, ' ', fa.building) AS fromAddress,
                            CONCAT(tc.city, ', ', tst.street, ' ', ta.building) AS toAddress,
                            t.startDateTime
                        FROM Trip t
                        LEFT JOIN Address fa ON t.fromAddressID = fa.addressID
                        LEFT JOIN City fc ON fa.cityID = fc.cityID
                        LEFT JOIN Street fst ON fa.streetID = fst.streetID
                        LEFT JOIN Address ta ON t.toAddressID = ta.addressID
                        LEFT JOIN City tc ON ta.cityID = tc.cityID
                        LEFT JOIN Street tst ON ta.streetID = tst.streetID
                        WHERE t.statusID = 1
                        ORDER BY t.startDateTime ASC
                        LIMIT 50;";

                    MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvOrders.DataSource = dt;
                    gvOrders.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Ошибка при загрузке заказов.";
            }
        }

        // ===========================
        // Для диспетчера: активные заказы (statusID = 1)
        // ===========================
        private void LoadDispatcherOrders()
        {
            try
            {
                using (var conn = GetConn())
                {
                    conn.Open();

                    string sql = @"
                        SELECT 
                            t.tripID,
                            CONCAT(fc.city, ', ', fst.street, ' ', fa.building) AS fromAddress,
                            CONCAT(tc.city, ', ', tst.street, ' ', ta.building) AS toAddress,
                            t.startDateTime,
                            CONCAT(p.passengerName, ' ', p.passengerSurname) AS passengerName
                        FROM Trip t
                        LEFT JOIN Address fa ON t.fromAddressID = fa.addressID
                        LEFT JOIN City fc ON fa.cityID = fc.cityID
                        LEFT JOIN Street fst ON fa.streetID = fst.streetID
                        LEFT JOIN Address ta ON t.toAddressID = ta.addressID
                        LEFT JOIN City tc ON ta.cityID = tc.cityID
                        LEFT JOIN Street tst ON ta.streetID = tst.streetID
                        LEFT JOIN Passenger p ON t.passengerID = p.passengerID
                        WHERE t.statusID = 1
                        ORDER BY t.startDateTime ASC
                        LIMIT 200;";

                    MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvDispatcherOrders.DataSource = dt;
                    gvDispatcherOrders.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Ошибка при загрузке активных заказов.";
            }
        }

        // ===========================
        // Кнопки обновить
        // ===========================
        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadOrders();
        }

        protected void btnRefreshDriver_Click(object sender, EventArgs e)
        {
            LoadOrders();
        }

        protected void btnRefreshDispatcher_Click(object sender, EventArgs e)
        {
            LoadDispatcherOrders();
        }

        // ===========================
        // Принять заказ (водитель)
        // ===========================
        protected void btnTake_Click(object sender, EventArgs e)
        {
            try
            {
                var btn = (System.Web.UI.WebControls.Button)sender;
                int tripID = Convert.ToInt32(btn.CommandArgument);

                int workerID = Convert.ToInt32(Session["workerID"]);
                int driverID = GetDriverID(workerID);

                if (driverID == -1)
                {
                    lblMessage.Text = "Вы не привязаны к записи в таблице Driver.";
                    return;
                }

                AssignTripToDriver(tripID, driverID);

                // Перенаправляем на страницу активного заказа (водителя)
                Response.Redirect("~/ActiveTrip.aspx?tripID=" + tripID);
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Ошибка при принятии заказа.";
            }
        }

        // Получить driverID по workerID
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

        // Назначить поездку водителю и сменить статус на InProgress (2)
        private void AssignTripToDriver(int tripID, int driverID)
        {
            try
            {
                using (var conn = GetConn())
                {
                    conn.Open();
                    string sql = "UPDATE Trip SET driverID = @driver, statusID = 2 WHERE tripID = @trip";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@driver", driverID);
                        cmd.Parameters.AddWithValue("@trip", tripID);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // логирование при необходимости
            }
        }

        // ===========================
        // Отмена заказа (диспетчер) -> statusID = 4 (Cancelled)
        // ===========================
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                var btn = (System.Web.UI.WebControls.Button)sender;
                int tripID = Convert.ToInt32(btn.CommandArgument);

                CancelTrip(tripID);

                // Обновляем список
                LoadDispatcherOrders();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Ошибка при отмене заказа.";
            }
        }

        private void CancelTrip(int tripID)
        {
            try
            {
                using (var conn = GetConn())
                {
                    conn.Open();
                    string sql = "UPDATE Trip SET statusID = 4 WHERE tripID = @trip";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@trip", tripID);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // логирование
            }
        }

        // ===========================
        // Кнопки диспетчера: переходы на страницы
        // ===========================
        protected void btnMakeQuery_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Query.aspx");
        }

        protected void btnConfigureTariff_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Tariffs.aspx");
        }

        protected void btnAddOrder_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/AddOrder.aspx");
        }

        // Выход
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}

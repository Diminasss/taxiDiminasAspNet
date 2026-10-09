using System;
using System.Data;
using System.Web;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace Lab5
{
    public partial class WorkerHome : System.Web.UI.Page
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

                LoadWorkerInfo();
            }
        }

        private MySqlConnection GetConn()
        {
            return new MySqlConnection(ConnStr);
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
                                    LoadOrders(); // доступные заказы
                                    LoadDriverActiveTrip(); // текущий активный заказ водителя (если есть)
                                }

                                if (positionID == 2)
                                {
                                    LoadDispatcherOrders(); // для диспетчера: заказы со статусом 2 и 3
                                }
                            }
                            else
                            {
                                Session.Abandon();
                                Response.Redirect("~/Login.aspx");
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                lblMessage.Text = "Ошибка при загрузке информации о работнике.";
            }
        }

        // ===========================
        // Доступные заказы (statusID = 1)
        // ===========================
        private void LoadOrders()
        {
            try
            {
                int workerID = Convert.ToInt32(Session["workerID"]);
                int driverID = GetDriverID(workerID);

                if (driverID != -1 && DriverHasActiveTrip(driverID))
                {
                    gvOrders.DataSource = null;
                    gvOrders.DataBind();
                    lblMessage.Text = "У вас уже есть активный заказ — сначала завершите или отмените его.";
                    return;
                }

                using (var conn = GetConn())
                {
                    conn.Open();

                    string sql = @"
                        SELECT 
                            t.tripID,
                            CONCAT(fc.city, ', ', fst.street, ' ', IFNULL(fa.building,'')) AS fromAddress,
                            CONCAT(tc.city, ', ', tst.street, ' ', IFNULL(ta.building,'')) AS toAddress,
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

                    lblMessage.Text = "";
                }
            }
            catch (Exception)
            {
                lblMessage.Text = "Ошибка при загрузке заказов.";
            }
        }

        // ===========================
        // Текущий активный заказ у водителя (statusID = 2 AND driverID = this driver)
        // ===========================
        private void LoadDriverActiveTrip()
        {
            pnlActiveTrip.Visible = false;
            lblActiveTripID.Text = "";
            lblActiveFrom.Text = "";
            lblActiveTo.Text = "";
            lblActiveStart.Text = "";
            lblActivePassenger.Text = "";

            try
            {
                int workerID = Convert.ToInt32(Session["workerID"]);
                int driverID = GetDriverID(workerID);
                if (driverID == -1) return;

                using (var conn = GetConn())
                {
                    conn.Open();

                    string sql = @"
                        SELECT t.tripID,
                               t.startDateTime,
                               CONCAT(fc.city, ', ', fst.street, ' ', IFNULL(fa.building,'')) AS fromAddress,
                               CONCAT(tc.city, ', ', tst.street, ' ', IFNULL(ta.building,'')) AS toAddress,
                               CONCAT(p.passengerName, ' ', p.passengerSurname) AS passengerName
                        FROM Trip t
                        LEFT JOIN Address fa ON t.fromAddressID = fa.addressID
                        LEFT JOIN City fc ON fa.cityID = fc.cityID
                        LEFT JOIN Street fst ON fa.streetID = fst.streetID
                        LEFT JOIN Address ta ON t.toAddressID = ta.addressID
                        LEFT JOIN City tc ON ta.cityID = tc.cityID
                        LEFT JOIN Street tst ON ta.streetID = tst.streetID
                        LEFT JOIN Passenger p ON t.passengerID = p.passengerID
                        WHERE t.statusID = 2 AND t.driverID = @driver
                        ORDER BY t.startDateTime DESC
                        LIMIT 1;";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@driver", driverID);
                        using (var r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                            {
                                pnlActiveTrip.Visible = true;
                                lblActiveTripID.Text = r.GetInt32(r.GetOrdinal("tripID")).ToString();
                                lblActiveStart.Text = r.IsDBNull(r.GetOrdinal("startDateTime")) ? "-" : r.GetDateTime(r.GetOrdinal("startDateTime")).ToString("g");
                                lblActiveFrom.Text = r.IsDBNull(r.GetOrdinal("fromAddress")) ? "-" : HttpUtility.HtmlEncode(r.GetString(r.GetOrdinal("fromAddress")));
                                lblActiveTo.Text = r.IsDBNull(r.GetOrdinal("toAddress")) ? "-" : HttpUtility.HtmlEncode(r.GetString(r.GetOrdinal("toAddress")));
                                lblActivePassenger.Text = r.IsDBNull(r.GetOrdinal("passengerName")) ? "-" : HttpUtility.HtmlEncode(r.GetString(r.GetOrdinal("passengerName")));
                            }
                            else
                            {
                                pnlActiveTrip.Visible = false;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                lblMessage.Text = "Ошибка при получении активного заказа.";
            }
        }

        // ===========================
        // Dispatcher: загрузка заказов со статусом 2 (InProgress) и 3 (Completed)
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
                            CONCAT(fc.city, ', ', fst.street, ' ', IFNULL(fa.building,'')) AS fromAddress,
                            CONCAT(tc.city, ', ', tst.street, ' ', IFNULL(ta.building,'')) AS toAddress,
                            t.startDateTime,
                            CONCAT(p.passengerName, ' ', p.passengerSurname) AS passengerName,
                            CONCAT(w.workerName, ' ', w.workerSurname) AS driverName,
                            s.statusName
                        FROM Trip t
                        LEFT JOIN Address fa ON t.fromAddressID = fa.addressID
                        LEFT JOIN City fc ON fa.cityID = fc.cityID
                        LEFT JOIN Street fst ON fa.streetID = fst.streetID
                        LEFT JOIN Address ta ON t.toAddressID = ta.addressID
                        LEFT JOIN City tc ON ta.cityID = tc.cityID
                        LEFT JOIN Street tst ON ta.streetID = tst.streetID
                        LEFT JOIN Passenger p ON t.passengerID = p.passengerID
                        LEFT JOIN Driver d ON t.driverID = d.driverID
                        LEFT JOIN Worker w ON d.workerID = w.workerID
                        LEFT JOIN Status s ON t.statusID = s.statusID
                        WHERE t.statusID IN (1,2)
                        ORDER BY t.startDateTime DESC
                        LIMIT 500;";

                    MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvDispatcherOrders.DataSource = dt;
                    gvDispatcherOrders.DataBind();
                }
            }
            catch (Exception)
            {
                lblMessage.Text = "Ошибка при загрузке заказов для диспетчера.";
            }
        }

        // ===========================
        // Обновить (driver)
        // ===========================
        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadOrders();
            LoadDriverActiveTrip();
        }

        // Обновить (dispatcher)
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

                bool ok = TryAssignTripToDriver(tripID, driverID);

                if (!ok)
                {
                    lblMessage.Text = "Не удалось принять заказ — возможно, он уже принят другим водителем или у вас есть активный заказ.";
                }
                else
                {
                    lblMessage.Text = "Заказ принят.";
                }

                LoadOrders();
                LoadDriverActiveTrip();
            }
            catch (Exception)
            {
                lblMessage.Text = "Ошибка при принятии заказа.";
            }
        }

        // Попытаться назначить поездку водителю — транзакция + проверка
        private bool TryAssignTripToDriver(int tripID, int driverID)
        {
            try
            {
                using (var conn = GetConn())
                {
                    conn.Open();

                    using (var tx = conn.BeginTransaction())
                    {
                        using (var cmdCheck = new MySqlCommand("SELECT COUNT(*) FROM Trip WHERE driverID = @driver AND statusID = 2 FOR UPDATE", conn, tx))
                        {
                            cmdCheck.Parameters.AddWithValue("@driver", driverID);
                            object o = cmdCheck.ExecuteScalar();
                            int cnt = o == null ? 0 : Convert.ToInt32(o);
                            if (cnt > 0)
                            {
                                tx.Rollback();
                                return false;
                            }
                        }

                        using (var cmdUpdate = new MySqlCommand("UPDATE Trip SET driverID = @driver, statusID = 2 WHERE tripID = @trip AND statusID = 1", conn, tx))
                        {
                            cmdUpdate.Parameters.AddWithValue("@driver", driverID);
                            cmdUpdate.Parameters.AddWithValue("@trip", tripID);
                            int affected = cmdUpdate.ExecuteNonQuery();
                            if (affected > 0)
                            {
                                tx.Commit();
                                return true;
                            }
                            else
                            {
                                tx.Rollback();
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Проверка, есть ли у водителя активный заказ
        private bool DriverHasActiveTrip(int driverID)
        {
            try
            {
                using (var conn = GetConn())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM Trip WHERE driverID = @d AND statusID = 2", conn))
                    {
                        cmd.Parameters.AddWithValue("@d", driverID);
                        object o = cmd.ExecuteScalar();
                        return Convert.ToInt32(o) > 0;
                    }
                }
            }
            catch
            {
                return false;
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

                LoadDispatcherOrders();
                lblMessage.Text = "Заказ отменён.";
            }
            catch (Exception)
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
            catch
            {
                // логирование при необходимости
            }
        }

        // ===========================
        // Кнопка отмены у водителя (на активном заказе)
        // ===========================
        protected void btnDriverCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(lblActiveTripID.Text))
                {
                    lblMessage.Text = "Нет активного заказа для отмены.";
                    return;
                }

                int tripID = Convert.ToInt32(lblActiveTripID.Text);

                using (var conn = GetConn())
                {
                    conn.Open();
                    string sql = "UPDATE Trip SET statusID = 4, driverID = NULL WHERE tripID = @trip";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@trip", tripID);
                        cmd.ExecuteNonQuery();
                    }
                }

                lblMessage.Text = "Заказ отменён.";
                LoadOrders();
                LoadDriverActiveTrip();
            }
            catch
            {
                lblMessage.Text = "Ошибка при отмене заказа.";
            }
        }

        // ===========================
        // Кнопка завершения у водителя (на активном заказе)
        // ===========================
        protected void btnDriverFinish_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(lblActiveTripID.Text))
                {
                    lblMessage.Text = "Нет активного заказа для завершения.";
                    return;
                }

                int tripID = Convert.ToInt32(lblActiveTripID.Text);
                decimal cost = 0;

                using (var conn = GetConn())
                {
                    conn.Open();

                    // 1️⃣ Получаем цену за километр города отправления
                    string sqlPrice = @"
                SELECT c.pricePerKilometer
                FROM Trip t
                JOIN Address a ON t.fromAddressID = a.addressID
                JOIN City c ON a.cityID = c.cityID
                WHERE t.tripID = @trip
                LIMIT 1";

                    using (var cmdPrice = new MySqlCommand(sqlPrice, conn))
                    {
                        cmdPrice.Parameters.AddWithValue("@trip", tripID);
                        object res = cmdPrice.ExecuteScalar();

                        if (res == null)
                        {
                            lblMessage.Text = "Не удалось определить тариф города.";
                            return;
                        }

                        int pricePerKm = Convert.ToInt32(res);

                        // 2️⃣ Генерируем случайное расстояние 1–10 км
                        Random rnd = new Random();
                        int distanceKm = rnd.Next(1, 11);

                        // 3️⃣ Считаем стоимость
                        cost = pricePerKm * distanceKm;
                    }

                    // 4️⃣ Обновляем поездку
                    string sqlUpdate = @"
                UPDATE Trip
                SET statusID = 3,
                    endDateTime = @now,
                    cost = @cost
                WHERE tripID = @trip";

                    using (var cmdUpdate = new MySqlCommand(sqlUpdate, conn))
                    {
                        cmdUpdate.Parameters.AddWithValue("@trip", tripID);
                        cmdUpdate.Parameters.AddWithValue("@now", DateTime.Now);
                        cmdUpdate.Parameters.AddWithValue("@cost", cost);
                        cmdUpdate.ExecuteNonQuery();
                    }
                }

                lblMessage.Text = $"Поездка завершена. Стоимость: {cost} ₽";
                LoadOrders();
                LoadDriverActiveTrip();
            }
            catch
            {
                lblMessage.Text = "Ошибка при завершении поездки.";
            }
        }


        // ===========================
        // Остальные кнопки диспетчера / навигация
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
        protected void btnEditUsers_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/EditUsers.aspx");
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

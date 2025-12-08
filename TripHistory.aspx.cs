using System;
using System.Data;
using System.Web.UI;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace Lab5
{
    public partial class TripHistory : Page
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["TaxiDB"].ConnectionString;
        private int CurrentTripID
        {
            get { return ViewState["CurrentTripID"] != null ? (int)ViewState["CurrentTripID"] : 0; }
            set { ViewState["CurrentTripID"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userID"] == null || (Session["role"] == null || Session["role"].ToString() != "passenger"))
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadTrips();
            }
        }

        private void LoadTrips()
        {
            try
            {
                int pid = Convert.ToInt32(Session["userID"]);

                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string query = @"
SELECT
    t.tripID,
    t.startDateTime,
    t.endDateTime,
    t.cost,
    t.feedbackID,
    s.statusName,
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
  AND t.statusID IN (3,4)
ORDER BY t.startDateTime DESC";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@pid", pid);
                        using (var reader = cmd.ExecuteReader())
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader);

                            dt.Columns.Add("fromAddress", typeof(string));
                            dt.Columns.Add("toAddress", typeof(string));
                            dt.Columns.Add("driver", typeof(string));
                            dt.Columns.Add("car", typeof(string));

                            foreach (DataRow row in dt.Rows)
                            {
                                row["fromAddress"] = $"{row["fromCity"]}, {row["fromStreet"]}, {row["fromBuilding"]}";
                                row["toAddress"] = $"{row["toCity"]}, {row["toStreet"]}, {row["toBuilding"]}";

                                string driver = $"{row["workerName"]} {row["workerSurname"]}".Trim();
                                row["driver"] = string.IsNullOrEmpty(driver) ? "Не назначен" : driver;

                                string car = $"{row["brandName"]} {row["modelName"]} {row["stateNumber"]}".Trim();
                                row["car"] = string.IsNullOrEmpty(car) ? "Информация отсутствует" : car;
                            }

                            gvTrips.DataSource = dt;
                            gvTrips.DataBind();
                        }
                    }
                }
            }
            catch (Exception)
            {
                lblMessage.Text = "Ошибка при загрузке истории поездок.";
            }
        }

        protected void gvTrips_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Feedback")
            {
                CurrentTripID = Convert.ToInt32(e.CommandArgument);
                lblFeedbackMessage.Text = "";
                txtFeedbackText.Text = "";
                ddlMark.SelectedValue = "5";

                ScriptManager.RegisterStartupScript(this, this.GetType(), "showModal", "showModal();", true);
            }
        }

        protected void btnCancelFeedback_Click(object sender, EventArgs e)
        {
            // Сброс модального окна
            CurrentTripID = 0;
        }

        protected void btnSubmitFeedback_Click(object sender, EventArgs e)
        {
            if (CurrentTripID == 0) return;

            int mark = Convert.ToInt32(ddlMark.SelectedValue);
            string text = txtFeedbackText.Text.Trim();

            try
            {
                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    // Вставляем отзыв
                    string insertFeedback = "INSERT INTO Feedback(text, mark) VALUES(@text, @mark); SELECT LAST_INSERT_ID();";
                    int feedbackID;
                    using (var cmd = new MySqlCommand(insertFeedback, conn))
                    {
                        cmd.Parameters.AddWithValue("@text", string.IsNullOrEmpty(text) ? DBNull.Value : (object)text);
                        cmd.Parameters.AddWithValue("@mark", mark);
                        feedbackID = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Обновляем поездку
                    string updateTrip = "UPDATE Trip SET feedbackID=@fid WHERE tripID=@tid";
                    using (var cmd = new MySqlCommand(updateTrip, conn))
                    {
                        cmd.Parameters.AddWithValue("@fid", feedbackID);
                        cmd.Parameters.AddWithValue("@tid", CurrentTripID);
                        cmd.ExecuteNonQuery();
                    }

                    lblMessage.Text = "Отзыв успешно добавлен!";
                    CurrentTripID = 0;
                    LoadTrips();
                }
            }
            catch (Exception ex)
            {
                lblFeedbackMessage.Text = "Ошибка при добавлении отзыва.";
            }
        }
    }
}

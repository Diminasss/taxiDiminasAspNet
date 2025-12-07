using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Lab5
{
    public partial class Selection : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadBrands();
                LoadYears();
                DoSearch(); // показать всё по умолчанию
            }
        }

        private MySqlConnection Conn()
        {
            string connStr = System.Configuration.ConfigurationManager
                .ConnectionStrings["TaxiDB"].ConnectionString;
            return new MySqlConnection(connStr);
        }

        private void LoadBrands()
        {
            using (var conn = Conn())
            {
                conn.Open();
                string sql = "SELECT carBrandID, brandName FROM Brand ORDER BY brandName";
                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    ddlBrand.Items.Clear();
                    ddlBrand.Items.Add(new ListItem("— Все бренды —", "0"));
                    while (reader.Read())
                    {
                        string id = reader["carBrandID"].ToString();
                        string name = reader["brandName"].ToString();
                        ddlBrand.Items.Add(new ListItem(name, id));
                    }
                }
            }
        }

        private void LoadYears()
        {
            using (var conn = Conn())
            {
                conn.Open();
                string sql = "SELECT DISTINCT productionYear FROM Car WHERE productionYear IS NOT NULL ORDER BY productionYear DESC";
                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    ddlYearFrom.Items.Clear();
                    ddlYearTo.Items.Clear();
                    ddlYearFrom.Items.Add(new ListItem("— выбрать —", "0"));
                    ddlYearTo.Items.Add(new ListItem("— выбрать —", "0"));

                    while (reader.Read())
                    {
                        string year = reader["productionYear"].ToString();
                        ddlYearFrom.Items.Add(new ListItem(year, year));
                        ddlYearTo.Items.Add(new ListItem(year, year));
                    }
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            DoSearch();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ddlBrand.SelectedValue = "0";
            ddlYearFrom.SelectedValue = "0";
            ddlYearTo.SelectedValue = "0";
            txtYearFrom.Value = "";
            txtYearTo.Value = "";

            DoSearch();
        }

        private void DoSearch()
        {
            int brandId = 0;
            int.TryParse(ddlBrand.SelectedValue, out brandId);

            int? yearFrom = null;
            if (!string.IsNullOrWhiteSpace(txtYearFrom.Value))
            {
                if (int.TryParse(txtYearFrom.Value.Trim(), out int yf))
                    yearFrom = yf;
            }
            else if (ddlYearFrom.SelectedValue != "0")
            {
                if (int.TryParse(ddlYearFrom.SelectedValue, out int yf2))
                    yearFrom = yf2;
            }

            int? yearTo = null;
            if (!string.IsNullOrWhiteSpace(txtYearTo.Value))
            {
                if (int.TryParse(txtYearTo.Value.Trim(), out int yt))
                    yearTo = yt;
            }
            else if (ddlYearTo.SelectedValue != "0")
            {
                if (int.TryParse(ddlYearTo.SelectedValue, out int yt2))
                    yearTo = yt2;
            }

            if (yearFrom.HasValue && yearTo.HasValue && yearFrom > yearTo)
            {
                int tmp = yearFrom.Value;
                yearFrom = yearTo;
                yearTo = tmp;
            }

            string sql = @"
SELECT 
  c.carID,
  b.brandName,
  m.modelName,
  c.productionYear,
  c.stateNumber,
  CONCAT(w.workerName, ' ', w.workerSurname) AS driverName
FROM Car c
LEFT JOIN Brand b ON c.carBrandID = b.carBrandID
LEFT JOIN Model m ON c.carModelID = m.carModelID
LEFT JOIN Driver d ON c.carID = d.carID
LEFT JOIN Worker w ON d.workerID = w.workerID
WHERE 1=1";

            var dt = new DataTable();
            using (var conn = Conn())
            {
                conn.Open();
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;

                    if (brandId != 0)
                    {
                        sql += " AND c.carBrandID = @brandId";
                        cmd.Parameters.AddWithValue("@brandId", brandId);
                    }
                    if (yearFrom.HasValue)
                    {
                        sql += " AND c.productionYear >= @yearFrom";
                        cmd.Parameters.AddWithValue("@yearFrom", yearFrom.Value);
                    }
                    if (yearTo.HasValue)
                    {
                        sql += " AND c.productionYear <= @yearTo";
                        cmd.Parameters.AddWithValue("@yearTo", yearTo.Value);
                    }

                    sql += " ORDER BY b.brandName, m.modelName, c.productionYear DESC";
                    cmd.CommandText = sql;

                    using (var adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }

            gvResults.DataSource = dt;
            gvResults.DataBind();

            lblInfo.Text = $"Найдено: {dt.Rows.Count} записей. " +
                           $"Фильтры: бренд = {(brandId == 0 ? "все" : ddlBrand.SelectedItem.Text)}, " +
                           $"год от = {(yearFrom.HasValue ? yearFrom.Value.ToString() : "любой")}, " +
                           $"год до = {(yearTo.HasValue ? yearTo.Value.ToString() : "любой")}.";

            tripsCard.Visible = false; // скрываем таблицу поездок при новом поиске
        }

        protected void gvResults_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvResults.PageIndex = e.NewPageIndex;
            DoSearch();
        }

        protected void gvResults_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ShowTrips")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                int carID = Convert.ToInt32(gvResults.DataKeys[index].Value);
                LoadTrips(carID);
            }
        }

        private void LoadTrips(int carID)
        {
            var dtTrips = new DataTable();
            dtTrips.Columns.Add("startDateTime", typeof(DateTime));
            dtTrips.Columns.Add("endDateTime", typeof(DateTime));
            dtTrips.Columns.Add("cost", typeof(decimal));
            dtTrips.Columns.Add("fromAddress", typeof(string));
            dtTrips.Columns.Add("toAddress", typeof(string));
            dtTrips.Columns.Add("feedback", typeof(string));
            dtTrips.Columns.Add("statusName", typeof(string));

            using (var conn = Conn())
            {
                conn.Open();
                string sql = @"
SELECT 
    t.startDateTime,
    t.endDateTime,
    t.cost,
    CONCAT(cFrom.city, ', ', sFrom.street, ' ', aFrom.building) AS fromAddress,
    CONCAT(cTo.city, ', ', sTo.street, ' ', aTo.building) AS toAddress,
    IFNULL(f.text,'') AS feedback,
    st.statusName
FROM Trip t
LEFT JOIN Address aFrom ON t.fromAddressID = aFrom.addressID
LEFT JOIN City cFrom ON aFrom.cityID = cFrom.cityID
LEFT JOIN Street sFrom ON aFrom.streetID = sFrom.streetID
LEFT JOIN Address aTo ON t.toAddressID = aTo.addressID
LEFT JOIN City cTo ON aTo.cityID = cTo.cityID
LEFT JOIN Street sTo ON aTo.streetID = sTo.streetID
LEFT JOIN Feedback f ON t.feedbackID = f.feedbackID
LEFT JOIN Status st ON t.statusID = st.statusID
LEFT JOIN Driver d ON t.driverID = d.driverID
WHERE d.carID = @carID
ORDER BY t.startDateTime DESC";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@carID", carID);
                    using (var adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dtTrips);
                    }
                }
            }

            gvTrips.DataSource = dtTrips;
            gvTrips.DataBind();
            tripsCard.Visible = true;
        }
    }
}

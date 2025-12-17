using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace Lab5
{
    public partial class Tariffs : Page
    {
        private string ConnStr => ConfigurationManager.ConnectionStrings["TaxiDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Проверяем право доступа: только администратор (positionID = 2)
                if (!IsCurrentWorkerAdmin())
                {
                    // перенаправляем назад — нет доступа
                    Response.Redirect("~/WorkerHome.aspx");
                    return;
                }

                LoadCities();
            }
        }

        private bool IsCurrentWorkerAdmin()
        {
            try
            {
                if (Session["workerID"] == null) return false;
                int workerID = Convert.ToInt32(Session["workerID"]);

                using (var conn = new MySqlConnection(ConnStr))
                {
                    conn.Open();
                    string q = "SELECT positionID FROM Worker WHERE workerID = @w LIMIT 1";
                    using (var cmd = new MySqlCommand(q, conn))
                    {
                        cmd.Parameters.AddWithValue("@w", workerID);
                        object o = cmd.ExecuteScalar();
                        if (o == null) return false;
                        int pos = Convert.ToInt32(o);
                        return pos == 2; // 2 == Administrator в вашей структуре
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        private void LoadCities()
        {
            try
            {
                using (var conn = new MySqlConnection(ConnStr))
                {
                    conn.Open();
                    string sql = "SELECT cityID, city, pricePerKilometer FROM City ORDER BY city ASC";
                    using (var da = new MySqlDataAdapter(sql, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        gvCities.DataSource = dt;
                        gvCities.DataBind();
                    }
                }

                lblMessage.Text = "";
                lblMessage.CssClass = "";
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Ошибка при загрузке городов: " + HttpUtility.HtmlEncode(ex.Message);
                lblMessage.CssClass = "text-danger";
            }
        }

        protected void gvCities_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvCities.EditIndex = e.NewEditIndex;
            LoadCities();
        }

        protected void gvCities_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvCities.EditIndex = -1;
            LoadCities();
        }

        protected void gvCities_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            lblMessage.Text = "";
            lblMessage.CssClass = "";

            int cityID = Convert.ToInt32(gvCities.DataKeys[e.RowIndex].Value);

            // находим контролы в EditItemTemplate
            TextBox txtCity = gvCities.Rows[e.RowIndex].FindControl("txtCity") as TextBox;
            TextBox txtPrice = gvCities.Rows[e.RowIndex].FindControl("txtPrice") as TextBox;

            if (txtCity == null || txtPrice == null)
            {
                lblMessage.Text = "Внутренняя ошибка: контролы не найдены.";
                lblMessage.CssClass = "text-danger";
                return;
            }

            string cityName = txtCity.Text.Trim();
            if (string.IsNullOrWhiteSpace(cityName))
            {
                lblMessage.Text = "Название города не может быть пустым.";
                lblMessage.CssClass = "text-danger";
                return;
            }

            if (!int.TryParse(txtPrice.Text.Trim(), out int price) || price < 0)
            {
                lblMessage.Text = "Цена за км должна быть положительным целым числом.";
                lblMessage.CssClass = "text-danger";
                return;
            }

            try
            {
                using (var conn = new MySqlConnection(ConnStr))
                {
                    conn.Open();
                    string sql = "UPDATE City SET city = @city, pricePerKilometer = @price WHERE cityID = @id";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@city", cityName);
                        cmd.Parameters.AddWithValue("@price", price);
                        cmd.Parameters.AddWithValue("@id", cityID);
                        cmd.ExecuteNonQuery();
                    }
                }

                gvCities.EditIndex = -1;
                LoadCities();

                lblMessage.Text = "Тариф обновлён.";
                lblMessage.CssClass = "text-success";
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Ошибка при обновлении: " + HttpUtility.HtmlEncode(ex.Message);
                lblMessage.CssClass = "text-danger";
            }
        }

        protected void btnAddCity_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            lblMessage.CssClass = "";

            string cityName = txtNewCity.Text.Trim();
            string priceText = txtNewPrice.Text.Trim();

            if (string.IsNullOrWhiteSpace(cityName))
            {
                lblMessage.Text = "Введите название города.";
                lblMessage.CssClass = "text-danger";
                return;
            }

            if (!int.TryParse(priceText, out int price) || price < 0)
            {
                lblMessage.Text = "Цена за км должна быть положительным целым числом.";
                lblMessage.CssClass = "text-danger";
                return;
            }

            try
            {
                using (var conn = new MySqlConnection(ConnStr))
                {
                    conn.Open();

                    // проверим, нет ли уже города с таким именем
                    using (var check = new MySqlCommand("SELECT COUNT(*) FROM City WHERE city = @city", conn))
                    {
                        check.Parameters.AddWithValue("@city", cityName);
                        object c = check.ExecuteScalar();
                        if (Convert.ToInt32(c) > 0)
                        {
                            lblMessage.Text = "Город с таким именем уже существует.";
                            lblMessage.CssClass = "text-danger";
                            return;
                        }
                    }

                    using (var cmd = new MySqlCommand("INSERT INTO City (city, pricePerKilometer) VALUES (@city, @price)", conn))
                    {
                        cmd.Parameters.AddWithValue("@city", cityName);
                        cmd.Parameters.AddWithValue("@price", price);
                        cmd.ExecuteNonQuery();
                    }
                }

                txtNewCity.Text = "";
                txtNewPrice.Text = "";

                LoadCities();

                lblMessage.Text = "Город успешно добавлен.";
                lblMessage.CssClass = "text-success";
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Ошибка при добавлении города: " + HttpUtility.HtmlEncode(ex.Message);
                lblMessage.CssClass = "text-danger";
            }
        }
    }
}

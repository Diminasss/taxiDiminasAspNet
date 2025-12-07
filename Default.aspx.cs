using MySql.Data.MySqlClient;
using System;

namespace Lab5
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadStats();
                LoadCitiesWithPrices();
                LoadFeedback();
            }
        }

        private MySqlConnection Conn()
        {
            string connStr = System.Configuration.ConfigurationManager
                .ConnectionStrings["TaxiDB"].ConnectionString;
            return new MySqlConnection(connStr);
        }

        private void LoadStats()
        {
            using (var conn = Conn())
            {
                conn.Open();

                using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM City", conn))
                    lblCities.Text = cmd.ExecuteScalar().ToString();

                using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM Driver", conn))
                    lblDrivers.Text = cmd.ExecuteScalar().ToString();

                using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM Car", conn))
                    lblCars.Text = cmd.ExecuteScalar().ToString();
            }
        }

        private void LoadCitiesWithPrices()
        {
            using (var conn = Conn())
            {
                conn.Open();

                var cmd = new MySqlCommand(
                    "SELECT city, pricePerKilometer FROM City ORDER BY city", conn);

                var reader = cmd.ExecuteReader();
                CitiesRepeater.DataSource = reader;
                CitiesRepeater.DataBind();
            }
        }

        private void LoadFeedback()
        {
            using (var conn = Conn())
            {
                conn.Open();

                var cmd = new MySqlCommand(
                    "SELECT text, mark FROM Feedback ORDER BY feedbackID DESC LIMIT 10", conn);

                var reader = cmd.ExecuteReader();

                FeedbackRepeater.DataSource = reader;
                FeedbackRepeater.DataBind();
            }
        }
    }
}

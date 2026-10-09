using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace Lab5
{
    public partial class EditUsers : System.Web.UI.Page
    {
        private string ConnStr => ConfigurationManager.ConnectionStrings["TaxiDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadUsers();
                LoadPositions();
            }
        }

        private MySqlConnection GetConn() => new MySqlConnection(ConnStr);

        // Загрузка пользователей и водителей
        private void LoadUsers()
        {
            try
            {
                using (var conn = GetConn())
                {
                    conn.Open();
                    // Возвращаем именно workerLogin (без AS), чтобы совпадало с DataField в GridView
                    string sql = @"
                        SELECT w.workerID, w.workerName, w.workerSurname, w.workerLogin, cp.positionName
                        FROM Worker w
                        JOIN CareerPosition cp ON w.positionID = cp.positionID
                        ORDER BY w.workerID";
                    MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvUsers.DataSource = dt;
                    gvUsers.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Ошибка при загрузке пользователей: " + ex.Message;
            }
        }

        // Загрузка ролей в DropDownList
        private void LoadPositions()
        {
            try
            {
                using (var conn = GetConn())
                {
                    conn.Open();
                    string sql = "SELECT positionID, positionName FROM CareerPosition";
                    MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ddlPosition.DataSource = dt;
                    ddlPosition.DataTextField = "positionName";
                    ddlPosition.DataValueField = "positionID";
                    ddlPosition.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Ошибка при загрузке списка ролей: " + ex.Message;
            }
        }

        // Выбор пользователя из GridView
        protected void gvUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            int workerID = Convert.ToInt32(gvUsers.SelectedDataKey.Value);
            lblDetailID.Text = workerID.ToString();
            pnlDetail.Visible = true;

            try
            {
                using (var conn = GetConn())
                {
                    conn.Open();
                    string sql = "SELECT workerName, workerSurname, workerLogin, positionID FROM Worker WHERE workerID=@id LIMIT 1";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", workerID);
                        using (var r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                            {
                                txtName.Text = r["workerName"].ToString();
                                txtSurname.Text = r["workerSurname"].ToString();
                                txtLogin.Text = r["workerLogin"].ToString();
                                ddlPosition.SelectedValue = r["positionID"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Ошибка при загрузке профиля: " + ex.Message;
            }
        }

        // Сохранение изменений
        protected void btnUpdateUser_Click(object sender, EventArgs e)
        {
            try
            {
                int workerID = Convert.ToInt32(lblDetailID.Text);
                using (var conn = GetConn())
                {
                    conn.Open();
                    string sql = @"
                        UPDATE Worker 
                        SET workerName=@name, workerSurname=@surname, workerLogin=@login, positionID=@pos
                        WHERE workerID=@id";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                        cmd.Parameters.AddWithValue("@surname", txtSurname.Text.Trim());
                        cmd.Parameters.AddWithValue("@login", txtLogin.Text.Trim());
                        cmd.Parameters.AddWithValue("@pos", ddlPosition.SelectedValue);
                        cmd.Parameters.AddWithValue("@id", workerID);
                        cmd.ExecuteNonQuery();
                    }
                }

                lblMessage.Text = "Профиль успешно обновлён.";
                pnlDetail.Visible = false;
                LoadUsers(); // обновляем GridView
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Ошибка при обновлении профиля: " + ex.Message;
            }
        }
    }
}

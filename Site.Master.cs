using System;
using System.Web;
using System.Web.UI;

namespace Lab5
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            bool loggedIn =
                Session["userID"] != null ||
                Session["workerID"] != null;

            // базовое переключение
            hlLogin.Visible = !loggedIn;
            btnLogout.Visible = loggedIn;

            // определяем текущую страницу
            string path = Request.Path.ToLower();

            bool isLoginPage =
                path.EndsWith("login.aspx") ||
                path.EndsWith("/login") ||
                path.EndsWith("/login/");

            // скрываем Авторизацию на странице Login
            if (isLoginPage)
                hlLogin.Visible = false;
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();

            // удаляем cookie ASP.NET_SessionId
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                HttpCookie cookie = new HttpCookie("ASP.NET_SessionId", "");
                cookie.Expires = DateTime.Now.AddYears(-1);
                Response.Cookies.Add(cookie);
            }

            Response.Redirect("~/Default.aspx");
        }
    }
}

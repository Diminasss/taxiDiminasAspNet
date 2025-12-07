using System;

namespace Lab5
{
    public partial class Query : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // При первом открытии берём первого пассажира
                if (Session["PassengerID"] == null)
                {
                    Session["PassengerID"] = 1;
                }
            }
        }

        protected void ddlPassengers_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["PassengerID"] = ddlPassengers.SelectedValue;
            gvTrips.DataBind();
        }

        protected void gvTrips_SelectedIndexChanged(object sender, EventArgs e)
        {
            // сохраняем выбранный tripID в Session
            int tripID = Convert.ToInt32(gvTrips.SelectedDataKey.Value);
            Session["TripID"] = tripID;
            dvTrip.DataBind();
        }
    }
}

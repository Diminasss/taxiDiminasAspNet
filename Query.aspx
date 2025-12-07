<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Query.aspx.cs" Inherits="Lab5.Query" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />

<div class="container mt-4">

    <h2 class="mb-4">Выборка поездок по пассажиру</h2>

    <!-- список пассажиров -->
    <asp:DropDownList 
        ID="ddlPassengers" 
        runat="server"
        CssClass="form-select w-50 mb-3"
        AutoPostBack="true"
        DataSourceID="SqlPassengers"
        DataTextField="passengerName"
        DataValueField="passengerID"
        OnSelectedIndexChanged="ddlPassengers_SelectedIndexChanged">
    </asp:DropDownList>

    <!-- Источник данных пассажиров -->
    <asp:SqlDataSource 
        ID="SqlPassengers" 
        runat="server"
        ConnectionString="<%$ ConnectionStrings:TaxiDB %>"
        ProviderName="MySql.Data.MySqlClient"
        SelectCommand="SELECT passengerID, CONCAT(passengerSurname,' ', passengerName) AS passengerName FROM Passenger">
    </asp:SqlDataSource>


    <!-- таблица поездок -->
    <asp:GridView 
        ID="gvTrips" 
        runat="server"
        CssClass="table table-striped mt-3"
        AutoGenerateColumns="False"
        DataKeyNames="tripID"
        DataSourceID="SqlTrips"
        OnSelectedIndexChanged="gvTrips_SelectedIndexChanged">

        <Columns>
            <asp:BoundField DataField="tripID" HeaderText="ID" />
            <asp:BoundField DataField="startDateTime" HeaderText="Начало" />
            <asp:BoundField DataField="endDateTime" HeaderText="Конец" />
            <asp:BoundField DataField="cost" HeaderText="Стоимость" />

            <asp:CommandField ShowSelectButton="True" SelectText="Подробнее" />
        </Columns>

    </asp:GridView>

    <!-- Источник поездок -->
    <asp:SqlDataSource 
        ID="SqlTrips" 
        runat="server"
        ConnectionString="<%$ ConnectionStrings:TaxiDB %>"
        ProviderName="MySql.Data.MySqlClient"
        SelectCommand="
            SELECT tripID, startDateTime, endDateTime, cost 
            FROM Trip
            WHERE passengerID = @pid">
        <SelectParameters>
            <asp:SessionParameter SessionField="PassengerID" Name="pid" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>

    <h3 class="mt-4">Детали выбранной поездки</h3>

    <asp:DetailsView 
        ID="dvTrip" 
        runat="server"
        AutoGenerateRows="true"
        CssClass="table"
        DataSourceID="SqlTripDetails">
    </asp:DetailsView>

    <!-- данные о выбранной поездке -->
    <asp:SqlDataSource 
        ID="SqlTripDetails" 
        runat="server"
        ConnectionString="<%$ ConnectionStrings:TaxiDB %>"
        ProviderName="MySql.Data.MySqlClient"
        SelectCommand="
            SELECT Trip.*, 
                   Feedback.text AS feedbackText,
                   Feedback.mark AS feedbackMark
            FROM Trip
            LEFT JOIN Feedback ON Trip.feedbackID = Feedback.feedbackID
            WHERE Trip.tripID = @tid">
        <SelectParameters>
            <asp:SessionParameter Name="tid" SessionField="TripID" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>

</div>

</asp:Content>

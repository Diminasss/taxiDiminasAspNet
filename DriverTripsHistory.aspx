<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DriverTripsHistory.aspx.cs" Inherits="Lab5.DriverTripsHistory" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />

<div class="container py-4">
    <div class="d-flex justify-content-between align-items-center mb-3">
        <div>
            <h3 class="mb-0">История поездок — водитель</h3>
            <small class="text-muted">Все поездки, где вы были назначены как водитель</small>
        </div>
        <div>
            <asp:Button ID="btnBack" runat="server" CssClass="btn btn-outline-secondary" Text="Назад" OnClick="btnBack_Click" />
        </div>
    </div>

    <asp:Panel ID="pnlNotDriver" runat="server" Visible="false">
        <div class="alert alert-warning">Ваша учётная запись не привязана к таблице водителей. Обратитесь к администратору.</div>
    </asp:Panel>

    <asp:Panel ID="pnlContent" runat="server" Visible="false">
        <div class="row mb-3">
            <div class="col-md-6">
                <div class="card p-3">
                    <div class="d-flex justify-content-between">
                        <div>
                            <div class="small text-muted">Заработок за сегодня</div>
                            <div class="h5 fw-bold" id="todaySumLabel"><asp:Label ID="lblTodaySum" runat="server" Text="0.00"></asp:Label></div>
                        </div>
                        <div class="align-self-center text-muted small">→</div>
                    </div>
                </div>
            </div>

            <div class="col-md-6">
                <div class="card p-3">
                    <div class="d-flex justify-content-between">
                        <div>
                            <div class="small text-muted">Заработок за текущий месяц</div>
                            <div class="h5 fw-bold"><asp:Label ID="lblMonthSum" runat="server" Text="0.00"></asp:Label></div>
                        </div>
                        <div class="align-self-center text-muted small">→</div>
                    </div>
                </div>
            </div>
        </div>

        <asp:Label ID="lblInfo" runat="server" CssClass="text-muted mb-2 d-block"></asp:Label>

        <asp:GridView ID="gvTrips" runat="server" AutoGenerateColumns="False" CssClass="table table-striped table-bordered"
            EmptyDataText="Поездок не найдено." AllowPaging="true" PageSize="20" OnPageIndexChanging="gvTrips_PageIndexChanging">
            <Columns>
                <asp:BoundField DataField="tripID" HeaderText="ID" ItemStyle-Width="60px" />
                <asp:BoundField DataField="startDateTime" HeaderText="Начало" DataFormatString="{0:g}" />
                <asp:BoundField DataField="endDateTime" HeaderText="Окончание" DataFormatString="{0:g}" />
                <asp:BoundField DataField="cost" HeaderText="Стоимость" DataFormatString="{0:C2}" ItemStyle-Width="110px" />
                <asp:BoundField DataField="statusName" HeaderText="Статус" />
                <asp:BoundField DataField="fromAddress" HeaderText="Откуда" />
                <asp:BoundField DataField="toAddress" HeaderText="Куда" />
                <asp:BoundField DataField="passengerName" HeaderText="Пассажир" />
                <asp:BoundField DataField="feedbackMark" HeaderText="Оценка" ItemStyle-Width="80px" />
            </Columns>
        </asp:GridView>
    </asp:Panel>

    <asp:Label ID="lblMessage" runat="server" CssClass="text-danger mt-3"></asp:Label>
</div>

</asp:Content>

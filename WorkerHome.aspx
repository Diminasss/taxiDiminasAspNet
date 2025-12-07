<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WorkerHome.aspx.cs"
    Inherits="Lab5.WorkerHome" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />

<div class="container mt-4">

    <h2 class="mb-4">Панель сотрудника</h2>

    <!-- Информация о сотруднике -->
    <div class="alert alert-info d-flex justify-content-between align-items-center">
        <div>
            <strong>Вы вошли как:</strong>
            <asp:Label ID="lblWorkerName" runat="server" CssClass="fw-bold"></asp:Label>
            &nbsp;—&nbsp;
            <asp:Label ID="lblWorkerRole" runat="server"></asp:Label>
        </div>

        <div>
            <asp:Button ID="btnLogout" runat="server" CssClass="btn btn-outline-secondary btn-sm" Text="Выйти" OnClick="btnLogout_Click" />
        </div>
    </div>

    <!-- Блок для водителя -->
    <asp:Panel ID="DriverPanel" runat="server" Visible="false">

        <div class="d-flex justify-content-between align-items-center mb-2">
            <h4 class="mb-0">Доступные заказы</h4>
            <div class="d-flex gap-2">
                <asp:Button ID="btnRefreshDriver" runat="server" CssClass="btn btn-secondary btn-sm" Text="Обновить" OnClick="btnRefresh_Click" />
            </div>
        </div>

        <asp:GridView ID="gvOrders" runat="server" AutoGenerateColumns="False" CssClass="table table-striped"
            EmptyDataText="Нет доступных заказов.">
            <Columns>
                <asp:BoundField DataField="tripID" HeaderText="ID" ItemStyle-Width="60px" />
                <asp:BoundField DataField="fromAddress" HeaderText="Откуда" />
                <asp:BoundField DataField="toAddress" HeaderText="Куда" />
                <asp:BoundField DataField="startDateTime" HeaderText="Дата/Время" DataFormatString="{0:g}" />
                <asp:TemplateField HeaderText="Действие" ItemStyle-Width="120px">
                    <ItemTemplate>
                        <asp:Button ID="btnTake" runat="server" Text="Принять" CssClass="btn btn-success btn-sm"
                            CommandArgument='<%# Eval("tripID") %>' OnClick="btnTake_Click" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <div class="mt-3">
            <a href="ActiveTrip.aspx" class="btn btn-primary me-2">Активный заказ</a>
            <a href="DriverTripsHistory.aspx" class="btn btn-outline-dark me-2">История поездок</a>
            <a href="DriverEarnings.aspx" class="btn btn-warning">Мой заработок</a>
        </div>

    </asp:Panel>

    <!-- Блок для диспетчера (Administrator) -->
    <asp:Panel ID="DispatcherPanel" runat="server" Visible="false">

        <div class="d-flex justify-content-between align-items-center mb-3">
            <h4 class="mb-0">Панель диспетчера</h4>
            <div class="d-flex gap-2">
                <asp:Button ID="btnMakeQuery" runat="server" CssClass="btn btn-outline-primary btn-sm" Text="Сделать выборку" OnClick="btnMakeQuery_Click" />
                <asp:Button ID="btnConfigureTariff" runat="server" CssClass="btn btn-outline-secondary btn-sm" Text="Настроить тариф" OnClick="btnConfigureTariff_Click" />
                <asp:Button ID="btnAddOrder" runat="server" CssClass="btn btn-success btn-sm" Text="Добавить заказ" OnClick="btnAddOrder_Click" />
                <asp:Button ID="btnRefreshDisp" runat="server" CssClass="btn btn-secondary btn-sm" Text="Обновить" OnClick="btnRefreshDispatcher_Click" />
            </div>
        </div>

        <h5 class="mb-2">Активные заказы</h5>

        <asp:GridView ID="gvDispatcherOrders" runat="server" AutoGenerateColumns="False" CssClass="table table-hover"
            EmptyDataText="Нет активных заказов.">
            <Columns>
                <asp:BoundField DataField="tripID" HeaderText="ID" ItemStyle-Width="60px" />
                <asp:BoundField DataField="fromAddress" HeaderText="Откуда" />
                <asp:BoundField DataField="toAddress" HeaderText="Куда" />
                <asp:BoundField DataField="startDateTime" HeaderText="Дата/Время" DataFormatString="{0:g}" />
                <asp:BoundField DataField="passengerName" HeaderText="Пассажир" />
                <asp:TemplateField HeaderText="Отмена" ItemStyle-Width="90px">
                    <ItemTemplate>
                        <asp:Button ID="btnCancel" runat="server" Text="✖" CssClass="btn btn-danger btn-sm"
                            CommandArgument='<%# Eval("tripID") %>' OnClick="btnCancel_Click" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

    </asp:Panel>

    <asp:Label ID="lblMessage" runat="server" CssClass="text-danger mt-3"></asp:Label>

</div>

</asp:Content>

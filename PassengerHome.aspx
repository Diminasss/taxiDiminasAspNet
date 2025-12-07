<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PassengerHome.aspx.cs" Inherits="Lab5.PassengerHome" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />

<div class="container py-5">

    <div class="d-flex justify-content-between align-items-center mb-4">
        <div>
            <h1 class="h3 mb-0">Добро пожаловать, <asp:Label ID="lblUserName" runat="server" CssClass="fw-bold"></asp:Label></h1>
            <small class="text-muted">Здесь вы можете заказать поездку или просмотреть историю</small>
        </div>

        <div class="d-flex gap-2">
            <asp:Button ID="btnNewTrip" runat="server" CssClass="btn btn-success" Text="Заказать поездку" OnClick="btnNewTrip_Click" />
            <asp:Button ID="btnHistory" runat="server" CssClass="btn btn-outline-secondary" Text="История поездок" OnClick="btnHistory_Click" />
        </div>
    </div>

    <!-- Текущая поездка -->
    <h4 class="mb-3">Текущая сессия поездки</h4>

    <asp:Panel ID="pnlCurrentTrip" runat="server">
        <div class="card shadow-sm mb-4">
            <div class="card-body">
                <asp:Label ID="lblTripNotFound" runat="server" CssClass="text-muted">
                    Загрузка...
                </asp:Label>

                <div id="tripDetails" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-6">
                            <p class="mb-1"><strong>Статус:</strong> <asp:Label ID="lblStatus" runat="server"></asp:Label></p>
                            <p class="mb-1"><strong>Начало:</strong> <asp:Label ID="lblStart" runat="server"></asp:Label></p>
                            <p class="mb-1"><strong>Окончание:</strong> <asp:Label ID="lblEnd" runat="server"></asp:Label></p>
                            <p class="mb-1"><strong>Стоимость:</strong> <asp:Label ID="lblCost" runat="server"></asp:Label></p>
                        </div>

                        <div class="col-md-6">
                            <p class="mb-1"><strong>Водитель:</strong> <asp:Label ID="lblDriver" runat="server"></asp:Label></p>
                            <p class="mb-1"><strong>Автомобиль:</strong> <asp:Label ID="lblCar" runat="server"></asp:Label></p>
                            <p class="mb-1"><strong>Откуда:</strong> <asp:Label ID="lblFrom" runat="server"></asp:Label></p>
                            <p class="mb-1"><strong>Куда:</strong> <asp:Label ID="lblTo" runat="server"></asp:Label></p>
                        </div>
                    </div>

                    <div class="mt-3">
                        <!-- опционально: добавить кнопку отмены/связаться -->
                        <asp:Button ID="btnRefresh" runat="server" CssClass="btn btn-sm btn-outline-primary" Text="Обновить" OnClick="btnRefresh_Click" />
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>

    <asp:Label ID="lblMessage" runat="server" CssClass="text-danger"></asp:Label>

</div>

</asp:Content>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddOrder.aspx.cs" Inherits="Lab5.AddOrder" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />

<div class="container py-4">
    <h3 class="mb-4">Добавить заказ (администратор)</h3>

    <asp:ValidationSummary ID="vsMain" runat="server" CssClass="text-danger mb-3" />

    <div class="card">
        <div class="card-body">
            <asp:Label ID="lblMessage" runat="server" CssClass="d-block mb-3"></asp:Label>

            <div class="mb-3">
                <label class="form-label">Пассажир</label>
                <asp:DropDownList ID="ddlPassenger" runat="server" CssClass="form-select" />
                <asp:RequiredFieldValidator ID="rfvPassenger" runat="server" ControlToValidate="ddlPassenger"
                    InitialValue="" ErrorMessage="Выберите пассажира" Display="Dynamic" CssClass="text-danger small" />
            </div>

            <div class="row">
                <div class="col-md-6">
                    <h6>Откуда</h6>

                    <div class="mb-2">
                        <label class="form-label">Город</label>
                        <asp:DropDownList ID="ddlFromCity" runat="server" CssClass="form-select" AutoPostBack="true"
                            OnSelectedIndexChanged="ddlFromCity_SelectedIndexChanged" />
                        <asp:RequiredFieldValidator ID="rfvFromCity" runat="server" ControlToValidate="ddlFromCity"
                            InitialValue="" ErrorMessage="Выберите город отправления" Display="Dynamic" CssClass="text-danger small" />
                    </div>

                    <div class="mb-2">
                        <label class="form-label">Улица</label>
                        <asp:DropDownList ID="ddlFromStreet" runat="server" CssClass="form-select" />
                        <asp:RequiredFieldValidator ID="rfvFromStreet" runat="server" ControlToValidate="ddlFromStreet"
                            InitialValue="" ErrorMessage="Выберите улицу отправления" Display="Dynamic" CssClass="text-danger small" />
                    </div>

                    <div class="mb-2">
                        <label class="form-label">Дом / корпус</label>
                        <asp:TextBox ID="txtFromBuilding" runat="server" CssClass="form-control" />
                    </div>
                </div>

                <div class="col-md-6">
                    <h6>Куда</h6>

                    <div class="mb-2">
                        <label class="form-label">Город</label>
                        <asp:DropDownList ID="ddlToCity" runat="server" CssClass="form-select" AutoPostBack="true"
                            OnSelectedIndexChanged="ddlToCity_SelectedIndexChanged" />
                        <asp:RequiredFieldValidator ID="rfvToCity" runat="server" ControlToValidate="ddlToCity"
                            InitialValue="" ErrorMessage="Выберите город назначения" Display="Dynamic" CssClass="text-danger small" />
                    </div>

                    <div class="mb-2">
                        <label class="form-label">Улица</label>
                        <asp:DropDownList ID="ddlToStreet" runat="server" CssClass="form-select" />
                        <asp:RequiredFieldValidator ID="rfvToStreet" runat="server" ControlToValidate="ddlToStreet"
                            InitialValue="" ErrorMessage="Выберите улицу назначения" Display="Dynamic" CssClass="text-danger small" />
                    </div>

                    <div class="mb-2">
                        <label class="form-label">Дом / корпус</label>
                        <asp:TextBox ID="txtToBuilding" runat="server" CssClass="form-control" />
                    </div>
                </div>
            </div>

            <div class="mt-3 d-flex gap-2">
                <asp:Button ID="btnCreateOrder" runat="server" CssClass="btn btn-primary" Text="Создать заказ" OnClick="btnCreateOrder_Click" />
                <asp:Button ID="btnCancel" runat="server" CssClass="btn btn-secondary" Text="Отмена" OnClick="btnCancel_Click" />
            </div>
        </div>
    </div>
</div>

</asp:Content>

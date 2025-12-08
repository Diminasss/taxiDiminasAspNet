<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NewTrip.aspx.cs" Inherits="Lab5.NewTrip" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />

<div class="container py-5">
    <div class="card mx-auto" style="max-width:800px;">
        <div class="card-body">
            <h3 class="card-title mb-3">Новый заказ такси (только моментально)</h3>

            <asp:Label ID="lblMessage" runat="server" CssClass="text-danger mb-3 d-block"></asp:Label>

            <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="text-danger" />

            <div class="row">
                <div class="col-md-6">
                    <h5>Откуда</h5>

                    <div class="mb-3">
                        <label class="form-label">Город</label>
                        <asp:DropDownList ID="ddlFromCity" runat="server" CssClass="form-select" />
                        <asp:RequiredFieldValidator ID="rfvFromCity" runat="server" ControlToValidate="ddlFromCity"
                            InitialValue="0" ErrorMessage="Выберите город (откуда)" Display="Dynamic" CssClass="text-danger" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label">Улица</label>
                        <asp:DropDownList ID="ddlFromStreet" runat="server" CssClass="form-select" />
                        <asp:RequiredFieldValidator ID="rfvFromStreet" runat="server" ControlToValidate="ddlFromStreet"
                            InitialValue="0" ErrorMessage="Выберите улицу (откуда)" Display="Dynamic" CssClass="text-danger" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label">Дом / корпус</label>
                        <asp:TextBox ID="txtFromBuilding" runat="server" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="rfvFromBuilding" runat="server" ControlToValidate="txtFromBuilding"
                            ErrorMessage="Укажите номер дома (откуда)" Display="Dynamic" CssClass="text-danger" />
                    </div>
                </div>

                <div class="col-md-6">
                    <h5>Куда</h5>

                    <div class="mb-3">
                        <label class="form-label">Город</label>
                        <asp:DropDownList ID="ddlToCity" runat="server" CssClass="form-select" />
                        <asp:RequiredFieldValidator ID="rfvToCity" runat="server" ControlToValidate="ddlToCity"
                            InitialValue="0" ErrorMessage="Выберите город (куда)" Display="Dynamic" CssClass="text-danger" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label">Улица</label>
                        <asp:DropDownList ID="ddlToStreet" runat="server" CssClass="form-select" />
                        <asp:RequiredFieldValidator ID="rfvToStreet" runat="server" ControlToValidate="ddlToStreet"
                            InitialValue="0" ErrorMessage="Выберите улицу (куда)" Display="Dynamic" CssClass="text-danger" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label">Дом / корпус</label>
                        <asp:TextBox ID="txtToBuilding" runat="server" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="rfvToBuilding" runat="server" ControlToValidate="txtToBuilding"
                            ErrorMessage="Укажите номер дома (куда)" Display="Dynamic" CssClass="text-danger" />
                    </div>
                </div>
            </div>

            <div class="d-flex justify-content-end gap-2 mt-3">
                <asp:Button ID="btnCancel" runat="server" CssClass="btn btn-outline-secondary" Text="Отмена" OnClick="btnCancel_Click" />
                <asp:Button ID="btnCreateTrip" runat="server" CssClass="btn btn-primary" Text="Заказать сейчас" OnClick="btnCreateTrip_Click" />
            </div>

        </div>
    </div>
</div>

</asp:Content>

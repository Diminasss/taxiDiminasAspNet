<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditPassenger.aspx.cs" Inherits="Lab5.EditPassenger" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />

<div class="container py-5">
    <h3 class="mb-4">Редактирование данных аккаунта</h3>

    <asp:Label ID="lblMessage" runat="server" CssClass="text-danger mb-3 d-block"></asp:Label>

    <asp:Panel ID="pnlForm" runat="server">
        <div class="card shadow-sm">
            <div class="card-body">
                <div class="mb-3">
                    <label class="form-label">Логин</label>
                    <asp:TextBox ID="txtLogin" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                </div>

                <div class="mb-3">
                    <label class="form-label">Пароль</label>
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" MaxLength="50"></asp:TextBox>
                </div>

                <div class="mb-3">
                    <label class="form-label">Имя</label>
                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                </div>

                <div class="mb-3">
                    <label class="form-label">Фамилия</label>
                    <asp:TextBox ID="txtSurname" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                </div>

                <div class="mb-3">
                    <label class="form-label">Телефон</label>
                    <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" MaxLength="20"></asp:TextBox>
                </div>

                <div class="d-flex gap-2 mt-3">
                    <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary" Text="Сохранить" OnClick="btnSave_Click" />
                    <asp:Button ID="btnBack" runat="server" CssClass="btn btn-outline-secondary" Text="Вернуться назад" OnClick="btnBack_Click" />
                </div>
            </div>
        </div>
    </asp:Panel>
</div>

</asp:Content>

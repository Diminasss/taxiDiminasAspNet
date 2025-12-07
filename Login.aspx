<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" 
    Inherits="Lab5.Login" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<div class="container mt-5" style="max-width: 450px;">

    <h2 class="text-center mb-4">Авторизация</h2>

    <div class="card p-4 shadow-sm">

        <div class="mb-3">
            <label class="form-label">Логин</label>
            <asp:TextBox ID="txtLogin" CssClass="form-control" runat="server"></asp:TextBox>
        </div>

        <div class="mb-3">
            <label class="form-label">Пароль</label>
            <asp:TextBox ID="txtPassword" CssClass="form-control" 
                         runat="server" TextMode="Password"></asp:TextBox>
        </div>

        <asp:Label ID="lblMessage" runat="server" CssClass="text-danger"></asp:Label>

        <!-- Основная кнопка входа -->
        <asp:Button ID="btnLogin" 
            CssClass="btn btn-primary w-100 mt-3" 
            runat="server" Text="Войти" 
            OnClick="btnLogin_Click" />

        <!-- Дополнительная кнопка "Войти как работник" -->
        <asp:Button ID="btnWorkerLogin"
            runat="server"
            Text="Войти как работник"
            CssClass="btn btn-link w-100 mt-2"
            OnClick="btnWorkerLogin_Click" />

    </div>
</div>

</asp:Content>

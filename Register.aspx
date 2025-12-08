<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs"
    Inherits="Lab5.Register" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Bootstrap (если в MasterPage уже есть, можно оставить) -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>

    <style>
        .btn-wide {
            width: 100% !important;
            height: 48px !important;
            font-size: 16px;
            font-weight: 600;
            border-radius: 8px !important;
            display: inline-flex;
            align-items: center;
            justify-content: center;
        }
    </style>

    <div class="container mt-5" style="max-width: 480px;">
        <div class="text-center mb-4">
            <h2 class="h4">Регистрация пассажира</h2>
            <p class="text-muted small mb-0">Создайте аккаунт пассажира</p>
        </div>

        <div class="card p-4 shadow-sm">

            <asp:ValidationSummary ID="vsSummary" runat="server" CssClass="text-danger mb-2" />

            <!-- Login -->
            <div class="mb-3">
                <label class="form-label" for="<%= txtLogin.ClientID %>">Логин</label>
                <asp:TextBox ID="txtLogin" runat="server" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="rfvLogin" runat="server"
                    ControlToValidate="txtLogin" ErrorMessage="Введите логин."
                    Display="Dynamic" CssClass="text-danger small" />
            </div>

            <!-- Password -->
            <div class="mb-3">
                <label class="form-label" for="<%= txtPassword.ClientID %>">Пароль</label>
                <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" />
                <asp:RequiredFieldValidator ID="rfvPass" runat="server"
                    ControlToValidate="txtPassword" ErrorMessage="Введите пароль."
                    Display="Dynamic" CssClass="text-danger small" />
            </div>

            <!-- Имя -->
            <div class="mb-3">
                <label class="form-label" for="<%= txtName.ClientID %>">Имя</label>
                <asp:TextBox ID="txtName" runat="server" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="rfvName" runat="server"
                    ControlToValidate="txtName" ErrorMessage="Введите имя."
                    Display="Dynamic" CssClass="text-danger small" />
            </div>

            <!-- Фамилия -->
            <div class="mb-3">
                <label class="form-label" for="<%= txtSurname.ClientID %>">Фамилия</label>
                <asp:TextBox ID="txtSurname" runat="server" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="rfvSurname" runat="server"
                    ControlToValidate="txtSurname" ErrorMessage="Введите фамилию."
                    Display="Dynamic" CssClass="text-danger small" />
            </div>

            <!-- Телефон -->
            <div class="mb-3">
                <label class="form-label" for="<%= txtPhone.ClientID %>">Телефон</label>
                <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" />
                <asp:RegularExpressionValidator ID="revPhone" runat="server"
                    ControlToValidate="txtPhone"
                    ErrorMessage="Неверный формат телефона (пример: +71234567890 или 81234567890)."
                    ValidationExpression="^(\+?\d{10,15})$"
                    Display="Dynamic" CssClass="text-danger small" />
            </div>

            <asp:Label ID="lblMessage" runat="server" CssClass="text-danger d-block mb-2"></asp:Label>

            <div class="d-grid gap-2 mt-2">
                <asp:Button ID="btnRegister" runat="server" CssClass="btn btn-success btn-wide"
                    Text="Зарегистрироваться" OnClick="btnRegister_Click" />

                <asp:Button ID="btnBackToLogin"
                    runat="server"
                    Text="Назад к входу"
                    CssClass="btn btn-outline-secondary btn-wide"
                    OnClick="btnBackToLogin_Click" />

            </div>

        </div>
    </div>

</asp:Content>

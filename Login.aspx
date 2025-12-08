<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs"
    Inherits="Lab5.Login" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Bootstrap -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>

    <style>
        /* Одинаковые широкие кнопки */
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

        /* Немного отступа между полями и заголовком */
        .card .form-label {
            font-weight: 500;
        }

        /* Сообщение об ошибке чуть ниже и заметнее */
        #<%= lblMessage.ClientID %> {
            margin-top: 6px;
        }
    </style>

    <div class="container mt-5" style="max-width: 450px;">

        <div class="text-center mb-4">
            <h2 class="h4">Авторизация</h2>
            <p class="text-muted small mb-0">Вход для пассажиров и работников</p>
        </div>

        <div class="card p-4 shadow-sm">

            <!-- Логин -->
            <div class="mb-3">
                <label class="form-label" for="<%= txtLogin.ClientID %>">Логин</label>
                <asp:TextBox ID="txtLogin" CssClass="form-control" runat="server" />
            </div>

            <!-- Пароль -->
            <div class="mb-3">
                <label class="form-label" for="<%= txtPassword.ClientID %>">Пароль</label>
                <asp:TextBox ID="txtPassword" CssClass="form-control" runat="server" TextMode="Password" />
            </div>

            <asp:Label ID="lblMessage" runat="server" CssClass="text-danger d-block mb-2"></asp:Label>

            <!-- Кнопки (все одинаковые по размеру) -->
            <div class="d-grid gap-2 mt-2">

                <!-- Войти (пассажир) -->
                <asp:Button ID="btnLogin"
                    CssClass="btn btn-primary btn-wide"
                    runat="server" Text="Войти"
                    OnClick="btnLogin_Click" />

                <!-- Войти как работник -->
                <asp:Button ID="btnWorkerLogin"
                    CssClass="btn btn-outline-secondary btn-wide"
                    runat="server" Text="Войти как работник"
                    OnClick="btnWorkerLogin_Click" />

                <!-- Зарегистрироваться: asp:Button с клиентским перенаправлением,
                     чтобы не менять code-behind -->
                <asp:Button ID="btnRegister"
                    CssClass="btn btn-success btn-wide"
                    runat="server" Text="Зарегистрироваться"
                    OnClientClick="window.location.href='Register.aspx'; return false;" />

            </div>

            <!-- Подсказка -->
            <div class="text-center mt-3">
                <small class="text-muted">Если у вас нет учётной записи — зарегистрируйтесь.</small>
            </div>

        </div>
    </div>

</asp:Content>

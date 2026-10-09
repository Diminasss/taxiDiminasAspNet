<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditUsers.aspx.cs" 
    Inherits="Lab5.EditUsers" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />

<div class="container mt-4">

    <h2 class="mb-4">Редактирование пользователей и водителей</h2>

    <asp:Label ID="lblMessage" runat="server" CssClass="text-danger mb-2"></asp:Label>

    <!-- GridView со всеми пользователями -->
    <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="False" CssClass="table table-hover"
        DataKeyNames="workerID" AutoGenerateSelectButton="true" OnSelectedIndexChanged="gvUsers_SelectedIndexChanged">
        <Columns>
            <asp:BoundField DataField="workerID" HeaderText="ID" ItemStyle-Width="60px" ReadOnly="true"/>
            <asp:BoundField DataField="workerName" HeaderText="Имя" />
            <asp:BoundField DataField="workerSurname" HeaderText="Фамилия" />
            <asp:BoundField DataField="workerLogin" HeaderText="Логин" />
            <asp:BoundField DataField="positionName" HeaderText="Роль" />
        </Columns>
    </asp:GridView>

    <hr />

    <!-- DetailView для редактирования выбранного пользователя -->
    <asp:Panel ID="pnlDetail" runat="server" Visible="false" CssClass="border p-3 rounded">
        <h4>Редактирование профиля</h4>
        <asp:Label ID="lblDetailID" runat="server" CssClass="d-none"></asp:Label>

        <div class="mb-3">
            <label class="form-label">Имя</label>
            <asp:TextBox ID="txtName" runat="server" CssClass="form-control"></asp:TextBox>
        </div>

        <div class="mb-3">
            <label class="form-label">Фамилия</label>
            <asp:TextBox ID="txtSurname" runat="server" CssClass="form-control"></asp:TextBox>
        </div>

        <div class="mb-3">
            <label class="form-label">Логин</label>
            <asp:TextBox ID="txtLogin" runat="server" CssClass="form-control"></asp:TextBox>
        </div>

        <div class="mb-3">
            <label class="form-label">Роль</label>
            <asp:DropDownList ID="ddlPosition" runat="server" CssClass="form-select">
            </asp:DropDownList>
        </div>

        <asp:Button ID="btnUpdateUser" runat="server" CssClass="btn btn-success" Text="Сохранить изменения" OnClick="btnUpdateUser_Click" />

    </asp:Panel>

</div>
</asp:Content>

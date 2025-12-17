<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Tariffs.aspx.cs"
    Inherits="Lab5.Tariffs" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />

<div class="container py-4">
    <h3 class="mb-3">Тарифы по городам — редактирование (только для администратора)</h3>

    <asp:Label ID="lblMessage" runat="server" CssClass="mb-3 d-block"></asp:Label>

    <div class="card mb-4">
        <div class="card-body p-3">
            <asp:GridView ID="gvCities" runat="server" CssClass="table table-striped"
                AutoGenerateColumns="False" DataKeyNames="cityID"
                OnRowEditing="gvCities_RowEditing"
                OnRowCancelingEdit="gvCities_RowCancelingEdit"
                OnRowUpdating="gvCities_RowUpdating"
                >
                <Columns>
                    
                    <asp:BoundField DataField="cityID" HeaderText="ID" ReadOnly="True" Visible="false" />

                   
                    <asp:TemplateField HeaderText="Город">
                        <ItemTemplate>
                            <%# Eval("city") %>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtCity" runat="server" CssClass="form-control"
                                Text='<%# Bind("city") %>'></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>

                    
                    <asp:TemplateField HeaderText="Цена за км">
                        <ItemTemplate>
                            <%# Eval("pricePerKilometer") %>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtPrice" runat="server" CssClass="form-control"
                                Text='<%# Bind("pricePerKilometer") %>'></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>

                    
                    <asp:CommandField ShowEditButton="True" HeaderText="Действия" />
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <!-- Добавление нового города -->
    <div class="card">
        <div class="card-body">
            <h5 class="card-title">Добавить новый город</h5>
            <div class="row g-2 align-items-end">
                <div class="col-md-6">
                    <label class="form-label">Название города</label>
                    <asp:TextBox ID="txtNewCity" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-3">
                    <label class="form-label">Цена за км</label>
                    <asp:TextBox ID="txtNewPrice" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-3">
                    <asp:Button ID="btnAddCity" runat="server" CssClass="btn btn-success w-100"
                        Text="Добавить" OnClick="btnAddCity_Click" />
                </div>
            </div>
        </div>
    </div>

</div>

</asp:Content>

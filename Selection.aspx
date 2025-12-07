<%@ Page Title="Selection" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Selection.aspx.cs" Inherits="Lab5.Selection" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>

    <div class="container py-4">
        <h2 class="mb-4">Выборка по автомобилям</h2>

        <div class="card mb-4 shadow-sm">
            <div class="card-body">

                <div id="frmFilter" class="row g-3">

                    <div class="col-md-4">
                        <label class="form-label">Бренд</label>
                        <asp:DropDownList ID="ddlBrand" runat="server" CssClass="form-select">
                            <asp:ListItem Value="0">— Все бренды —</asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-4">
                        <label class="form-label">Год (от)</label>
                        <div class="input-group">
                            <asp:DropDownList ID="ddlYearFrom" runat="server" CssClass="form-select">
                                <asp:ListItem Value="0">— выбрать —</asp:ListItem>
                            </asp:DropDownList>
                            <input type="text" id="txtYearFrom" runat="server" class="form-control"
                                   placeholder="ввести год вручную (опц.)" />
                        </div>
                    </div>

                    <div class="col-md-4">
                        <label class="form-label">Год (до)</label>
                        <div class="input-group">
                            <asp:DropDownList ID="ddlYearTo" runat="server" CssClass="form-select">
                                <asp:ListItem Value="0">— выбрать —</asp:ListItem>
                            </asp:DropDownList>
                            <input type="text" id="txtYearTo" runat="server" class="form-control"
                                   placeholder="ввести год вручную (опц.)" />
                        </div>
                    </div>

                    <div class="col-12 mt-2">
                        <asp:Button ID="btnSearch" runat="server" Text="Найти"
                                    CssClass="btn btn-primary me-2" OnClick="btnSearch_Click" />
                        <asp:Button ID="btnClear" runat="server" Text="Сбросить"
                                    CssClass="btn btn-secondary" OnClick="btnClear_Click" />
                    </div>

                </div>
            </div>
        </div>

        <div class="card shadow-sm mb-4">
            <div class="card-body">
                <h5 class="card-title">Результаты</h5>

                <asp:Label ID="lblInfo" runat="server" CssClass="text-muted mb-2 d-block" />

                <asp:GridView ID="gvResults" runat="server" CssClass="table table-striped"
                              AutoGenerateColumns="False" EmptyDataText="Ничего не найдено"
                              AllowPaging="True" PageSize="10"
                              DataKeyNames="carID"
                              OnPageIndexChanging="gvResults_PageIndexChanging"
                              OnRowCommand="gvResults_RowCommand">

                    <Columns>
                        <asp:BoundField DataField="brandName" HeaderText="Бренд" />
                        <asp:BoundField DataField="modelName" HeaderText="Модель" />
                        <asp:BoundField DataField="productionYear" HeaderText="Год" />
                        <asp:BoundField DataField="stateNumber" HeaderText="Гос. номер" />
                        <asp:BoundField DataField="driverName" HeaderText="Водитель" />
                        <asp:ButtonField ButtonType="Button" CommandName="ShowTrips"
                                         Text="Подробнее" HeaderText="Действие" />
                    </Columns>

                </asp:GridView>
            </div>
        </div>

        <div class="card shadow-sm" ID="tripsCard" runat="server" Visible="false">
            <div class="card-body">
                <h5 class="card-title">Поездки автомобиля</h5>

                <asp:GridView ID="gvTrips" runat="server" CssClass="table table-striped"
                              AutoGenerateColumns="False" EmptyDataText="Поездок не найдено">
                    <Columns>
                        <asp:BoundField DataField="startDateTime" HeaderText="Дата начала" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                        <asp:BoundField DataField="endDateTime" HeaderText="Дата окончания" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                        <asp:BoundField DataField="cost" HeaderText="Цена" DataFormatString="{0:C}" />
                        <asp:BoundField DataField="fromAddress" HeaderText="Адрес отправки" />
                        <asp:BoundField DataField="toAddress" HeaderText="Адрес прибытия" />
                        <asp:BoundField DataField="feedback" HeaderText="Отзыв" />
                        <asp:BoundField DataField="statusName" HeaderText="Статус" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>

    </div>

</asp:Content>

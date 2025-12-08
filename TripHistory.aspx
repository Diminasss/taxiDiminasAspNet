<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TripHistory.aspx.cs" Inherits="Lab5.TripHistory" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>

<div class="container py-5">
    <h3 class="mb-4">История поездок</h3>

    <asp:Label ID="lblMessage" runat="server" CssClass="text-danger mb-3 d-block"></asp:Label>

    <asp:GridView ID="gvTrips" runat="server" CssClass="table table-striped"
        AutoGenerateColumns="False" EmptyDataText="История поездок пуста"
        OnRowCommand="gvTrips_RowCommand">
        <Columns>
            <asp:BoundField DataField="tripID" HeaderText="ID" Visible="false" />
            <asp:BoundField DataField="startDateTime" HeaderText="Начало" DataFormatString="{0:g}" />
            <asp:BoundField DataField="endDateTime" HeaderText="Окончание" DataFormatString="{0:g}" />
            <asp:BoundField DataField="fromAddress" HeaderText="Откуда" />
            <asp:BoundField DataField="toAddress" HeaderText="Куда" />
            <asp:BoundField DataField="driver" HeaderText="Водитель" />
            <asp:BoundField DataField="car" HeaderText="Автомобиль" />
            <asp:BoundField DataField="statusName" HeaderText="Статус" />
            <asp:TemplateField HeaderText="Действие">
                <ItemTemplate>
                    <asp:Button ID="btnFeedback" runat="server" Text="Оставить отзыв"
                        CommandName="Feedback" CommandArgument='<%# Eval("tripID") %>'
                        Visible='<%# Eval("feedbackID") == DBNull.Value %>' CssClass="btn btn-sm btn-primary" />
                    <span runat="server" visible='<%# Eval("feedbackID") != DBNull.Value %>'>Отзыв оставлен</span>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</div>

<!-- Модальное окно для отзыва -->
<asp:Panel ID="pnlFeedbackModal" runat="server" CssClass="modal fade" Style="display:none;" aria-hidden="true">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Оставить отзыв</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
                <asp:Label ID="lblFeedbackMessage" runat="server" CssClass="text-danger mb-2"></asp:Label>
                <div class="mb-3">
                    <label class="form-label">Оценка (1-5)</label>
                    <asp:DropDownList ID="ddlMark" runat="server" CssClass="form-select">
                        <asp:ListItem Text="1" Value="1" />
                        <asp:ListItem Text="2" Value="2" />
                        <asp:ListItem Text="3" Value="3" />
                        <asp:ListItem Text="4" Value="4" />
                        <asp:ListItem Text="5" Value="5" Selected="True" />
                    </asp:DropDownList>
                </div>
                <div class="mb-3">
                    <label class="form-label">Текст отзыва (необязательно, до 255 символов)</label>
                    <asp:TextBox ID="txtFeedbackText" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" MaxLength="255"></asp:TextBox>
                </div>
            </div>
            <div class="modal-footer">
                <asp:Button ID="btnCancelFeedback" runat="server" CssClass="btn btn-outline-secondary" Text="Отмена" OnClick="btnCancelFeedback_Click" />
                <asp:Button ID="btnSubmitFeedback" runat="server" CssClass="btn btn-primary" Text="Сохранить" OnClick="btnSubmitFeedback_Click" />
            </div>
        </div>
    </div>
</asp:Panel>

<script type="text/javascript">
    function showModal() {
        var modalEl = document.getElementById('<%= pnlFeedbackModal.ClientID %>');
        var modal = new bootstrap.Modal(modalEl);
        modal.show();
    }
</script>

</asp:Content>

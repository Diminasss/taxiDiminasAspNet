<%@ Page Title="Diminas Taxi — Ваше городское такси" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Lab5._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>

<main class="container py-5">

    <div class="text-center mb-5">
        <h1 class="display-4">Taxi Diminas — Быстрое и надёжное такси</h1>
        <p class="lead">Перевозим пассажиров по городу и области. Современный парк автомобилей, профессиональные водители и круглосуточная поддержка.</p>
    </div>

    <div class="row text-center mb-5">
        <div class="col-md-4">
    <div class="card shadow-sm p-4">
        <h3>Водители</h3>
        <asp:Label ID="lblDrivers" runat="server" CssClass="fw-bold fs-2"></asp:Label>
    </div>
</div>
        <div class="col-md-4">
            <div class="card shadow-sm p-4">
                <h3>Города</h3>
                <asp:Label ID="lblCities" runat="server" CssClass="fw-bold fs-2"></asp:Label>

                <!-- список городов -->
                <div class="mt-3 text-start">
                    <asp:Repeater ID="CitiesRepeater" runat="server">
                        <ItemTemplate>
                            <div>
                                <strong><%# Eval("city") %></strong> — 
                                <%# Eval("pricePerKilometer") %> ₽/км
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

            </div>
        </div>

        

        <div class="col-md-4">
            <div class="card shadow-sm p-4">
                <h3>Автомобили</h3>
                <asp:Label ID="lblCars" runat="server" CssClass="fw-bold fs-2"></asp:Label>
            </div>
        </div>

    </div>


    <h2 class="mb-4">Последние отзывы клиентов</h2>

    <div id="feedbackCarousel" class="carousel slide" data-bs-ride="carousel" data-bs-interval="3000">
        <div class="carousel-inner">

            <asp:Repeater ID="FeedbackRepeater" runat="server">
                <ItemTemplate>
                    <div class='carousel-item <%# Container.ItemIndex == 0 ? "active" : "" %>'>
                        <div class="card shadow-sm p-4">
                            <p><strong>Оценка:</strong> <%# Eval("mark") %>/5</p>
                            <p><%# Eval("text") %></p>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

        </div>

        <button class="carousel-control-prev" type="button" data-bs-target="#feedbackCarousel" data-bs-slide="prev">
            <span class="carousel-control-prev-icon" aria-hidden="true"></span>
        </button>

        <button class="carousel-control-next" type="button" data-bs-target="#feedbackCarousel" data-bs-slide="next">
            <span class="carousel-control-next-icon" aria-hidden="true"></span>
        </button>
    </div>

</main>

</asp:Content>

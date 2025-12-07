<%@ Page Title="Контакты" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Contact.aspx.cs" Inherits="Lab5.Contact" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <main aria-labelledby="title">

        <h2 id="title">Контакты</h2>
        <h4 class="text-muted">Свяжитесь с нами удобным для вас способом</h4>

        <p class="mt-3">
            Если у вас возникли вопросы по работе сервиса, предложения по улучшению или проблемы с поездкой — мы всегда готовы помочь.
        </p>

        <h4 class="mt-4">Наш офис</h4>

        <address class="mt-2">
            Diminas Taxi Service<br />
            г. Санкт-Петербург<br />
            Невский проспект, 12А<br />
            <abbr title="Phone">Тел.:</abbr> +7 (812) 555-12-34
        </address>

        <h4 class="mt-4">Электронная почта</h4>
        <address>
            <strong>Техническая поддержка:</strong>
            <a href="mailto:support@diminas-taxi.com">support@diminas-taxi.com</a><br />

            <strong>Отдел жалоб и предложений:</strong>
            <a href="mailto:feedback@diminas-taxi.com">feedback@diminas-taxi.com</a><br />

            <strong>Для сотрудничества:</strong>
            <a href="mailto:partners@diminas-taxi.com">partners@diminas-taxi.com</a>
        </address>

        <h4 class="mt-4">Режим работы</h4>
        <p>
            Поддержка пассажиров и водителей — <strong>24/7</strong><br />
            Офис — <strong>с 10:00 до 18:00</strong>, понедельник–пятница
        </p>

    </main>

</asp:Content>

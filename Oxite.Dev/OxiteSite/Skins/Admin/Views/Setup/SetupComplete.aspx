<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="../Shared/Setup.master"
    Inherits="System.Web.Mvc.ViewPage<OxiteViewModelItem<Site>>" %>

<%@ Import Namespace="Oxite.Extensions" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="title">
        <%=Model.Localize("Site.Add", "Initial Site Setup") %></h2>

<div>
Congratulations!  You have a site!  Its ID is: <%=Model.Item.ID.ToString() %>
</div>
<div>
<%Html.ActionLink(Model.Localize("GoHome", "Go Home"), "Default"); %>
</div>
<div>
<%Html.ActionLink(Model.Localize("EditSettings", "Edit Settings"), "Site", "ManageSite"); %>
</div>
</asp:Content>
<asp:Content ContentPlaceHolderID="Title" runat="server">
    <%=Html.PageTitle(Model.Localize("Admin"), Model.Localize("Site"), Model.Localize("Setup")) %>
</asp:Content>
<asp:Content ContentPlaceHolderID="Scripts" runat="server">
    <%
        Html.RenderScriptTag("base.js"); %>
</asp:Content>

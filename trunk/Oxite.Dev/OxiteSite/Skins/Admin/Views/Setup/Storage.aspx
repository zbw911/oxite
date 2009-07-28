<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="../Shared/Setup.master"
    Inherits="System.Web.Mvc.ViewPage<OxiteViewModelItem<SetupInput>>" %>

<%@ Import Namespace="Oxite.Extensions" %>
<%@ Import Namespace="Oxite.Modules.Setup.Models" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="title">
        <%=(Model.Item.SiteID == Guid.Empty ? Model.Localize("Site.Add", "Initial Site Setup") : Model.Localize("Site.Edit", "Edit Site"))%></h2>
    <%=Html.ValidationSummary() %>
    <form action="" method="post" id="siteSettings">
    <%
        if (Model.Item.SiteID != Guid.Empty)
        { %>
    <div>
        <%=Html.Hidden("siteID", Model.Item.SiteID)%></div>
    <%
        } %>
    <h3>
        <%=Model.Localize("Setup.Storage", "What is your storage mechanism?") %></h3>
    <div>
        <%=Html.CheckBox("xmlFileStorage", false, "Xml Files", new { size = 60, @class = "checkbox" })%></div>
    <div>
        <%=Html.CheckBox("sqlStorage", true, "SQL Server", new { size = 60, @class = "checkbox" })%></div>
    <div class="buttons">
        <input type="submit" name="submit" class="button submit" value="<%=Model.Item.SiteID == Guid.Empty ? Model.Localize("Site.BasicSettings", "Next: Basic Site Settings") : Model.Localize("Site.Edit", "Edit Site") %>" />
        <%
            if (Model.Item.SiteID != Guid.Empty)
            { %>
        <%=Html.Button(
                "cancel",
                Model.Localize("Cancel"),
                new { @class = "cancel", onclick = string.Format("if (window.confirm('{0}')){{window.document.location='{1}';}}return false;", Model.Localize("really?"), Url.ManageSite()) }
                )%>
        <%=Html.Link(
                Model.Localize("Cancel"),
                Url.ManageSite(),
                new { @class = "cancel" })%><%
                                                } %><%=
            Html.OxiteAntiForgeryToken() %>
    </div>
    </form>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="Title" runat="server">
    <%=Html.PageTitle(Model.Localize("Admin"), Model.Localize("Site"), Model.Item.SiteID == Guid.Empty ? Model.Localize("Setup") : Model.Localize("Edit"))%>
</asp:Content>
<asp:Content ContentPlaceHolderID="Scripts" runat="server">
    <%
        Html.RenderScriptTag("base.js"); %>
</asp:Content>

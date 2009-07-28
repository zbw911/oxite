<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="../Shared/Setup.master"
    Inherits="System.Web.Mvc.ViewPage<OxiteViewModelItem<SetupInput>>" %>

<%@ Import Namespace="Oxite.Extensions" %>
<%@ Import Namespace="Oxite.Modules.Setup.Models" %>
<%@ Import Namespace="Oxite.Modules.Setup" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="title">
        <%=Model.Localize("Site.SelectTemplate", "Select Site Template")%></h2>
    <%=Html.ValidationSummary() %>
    <form action="" method="post" id="siteSettings">
    <div>
        <%=Html.Hidden("siteID", Model.Item.SiteID)%></div>
    <h3>
        <%=Model.Localize("Setup.WhichTemplate", "What site template would you like to start with?") %></h3>
    <div>
        <%=Html.RadioButton("siteType", SiteType.PersonalBlog, Model.Item.SiteType == SiteType.PersonalBlog, new { size = 60, @class = "checkbox" }, "Personal Blog")%></div>
    <div>
        <%=Html.RadioButton("siteType", SiteType.CorporateBlog, Model.Item.SiteType == SiteType.CorporateBlog, new { id = "useCorporateBlog", size = 60, @class = "checkbox" }, "Large Corporate Blog")%></div>
    <div>
        <%=Html.RadioButton("siteType", SiteType.ECommerce, Model.Item.SiteType == SiteType.ECommerce, new { id = "useCommerce", size = 60, @class = "checkbox" }, "E-Commerce Store")%></div>
    <div>
        <%=Html.RadioButton("siteType", SiteType.Community, Model.Item.SiteType == SiteType.Community, new { id = "useCommunity", size = 60, @class = "checkbox" }, "Community Site with Forums")%></div>
    <div class="buttons">
        <input type="submit" name="submit" class="button submit" value="<%=Model.Localize("Site.BasicSettings", "Next: Basic Site Settings")%>" />
        <%=Html.Button(
                "cancel",
                Model.Localize("Cancel"),
                new { @class = "cancel", onclick = string.Format("if (window.confirm('{0}')){{window.document.location='{1}';}}return false;", Model.Localize("really?"), Url.ManageSite()) }
                )%>
        <%=Html.OxiteAntiForgeryToken() %>
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

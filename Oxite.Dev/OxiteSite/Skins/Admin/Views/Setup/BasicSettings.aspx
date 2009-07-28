<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="../Shared/Setup.master"
    Inherits="System.Web.Mvc.ViewPage<OxiteViewModelItem<SetupInput>>" %>

<%@ Import Namespace="Oxite.Extensions" %>
<%@ Import Namespace="Oxite.Modules.Setup.Models" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="title">
        <%=Model.Localize("Site.Add", "Initial Site Setup") %></h2>
    <%=Html.ValidationSummary() %>
    <form action="" method="post" id="siteSettings">
    <%
        if (Model.Item.SiteID != Guid.Empty)
        { %>
    <div>
        <%=Html.Hidden("siteID", Model.Item.SiteID)%></div>
    <%
        } %>
    <fieldset>
        <%=Html.TextBox("blogName", m => m.Item != null ? m.Item.BlogName : "", "Name", true, new { size = 20, @class = "text" })%>
        <%=Html.TextBox("blogDisplayName", m => m.Item != null ? m.Item.BlogDisplayName : "", "Display Name", true, new { size = 40, @class = "text" })%>
        <%=Html.TextArea("blogDescription", m => m.Item != null ? m.Item.Description : "", 6, 80, "Description", true, new { @class = "text" })%>
    </fieldset>
    <fieldset>
        <%=Html.CheckBox("blogCommentingEnabled",
                        m => m.Item != null ? !m.Item.CommentingDisabled : true,
                        Model.Localize("CommentingEnabled", "Commenting Enabled")
                        )%>
    </fieldset>
    <fieldset>
        <div>
            <%=Html.TextBox("userName", m => "", "Username", true, new { size = 20, @class = "text" })%></div>
        <div>
            <%=Html.TextBox("userDisplayName", m => "", "Display Name", true, new { size = 20, @class = "text" })%></div>
        <div>
            <%=Html.TextBox("userEmail", m => "", "Email", true, new { size = 20, @class = "text" })%></div>
        <div>
            <%=Html.Password("userPassword", "", "Password", true, new { size = 40, @class = "text" })%></div>
        <div>
            <%=Html.Password("userPasswordConfirm", "", "Password (Confirm)", true, new { size = 40, @class = "text" })%></div>
    </fieldset>
    <fieldset>
        <% foreach (Module module in Model.Item.Modules)
           { %>
        <div>
            <%=Html.CheckBox(module.Name, module.Enabled, module.Name, new { size = 60, @class = "checkbox" })%></div>
        <%} %>
    </fieldset>
    <div class="buttons">
        <input type="submit" name="submit" class="button submit" value="<%=Model.Localize("Setup.CreateSite", "Create My Site!")%>" />
    </div>
    </form>
</asp:Content>
<asp:Content ContentPlaceHolderID="Title" runat="server">
    <%=Html.PageTitle(Model.Localize("Admin"), Model.Localize("Site"), Model.Localize("Setup")) %>
</asp:Content>
<asp:Content ContentPlaceHolderID="Scripts" runat="server">
    <%
        Html.RenderScriptTag("base.js"); %>
</asp:Content>

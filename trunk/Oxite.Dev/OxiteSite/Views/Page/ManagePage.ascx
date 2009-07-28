﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<OxiteViewModelItem<Oxite.Models.Page>>" %>
<%@ Import Namespace="Oxite.Extensions" %>
<%@ Import Namespace="Oxite.Models.Extensions" %>
<%@ Import Namespace="Oxite.Modules.CMS.Extensions" %><% 
bool urlIsLocked = Model.Item.HasChildren;

if (Model.User.IsInRole("Admin"))
{ %>
<div class="admin manage buttons"><% 
    if (Model.Item.State != EntityState.Removed)
    { %>
<a href="<%=Url.PageAdd(Model.Item) %>" title="<%=Model.Localize("Add") %>" class="ibutton add"><%=Html.SkinImage("/images/page_add.png", Model.Localize("Add"), new { width = 16, height = 16 })%></a>
<a href="<%=Url.PageEdit(Model.Item) %>" title="<%=Model.Localize("Edit") %>" class="ibutton edit"><%=Html.SkinImage("/images/page_edit.png", Model.Localize("Edit"), new { width = 16, height = 16 })%></a><%
        if (!urlIsLocked)
        { %>
<form class="remove post" method="post" action="<%=Url.PageRemove(Model.Item) %>">
    <fieldset>
        <input type="image" src="<%=Url.CssPath("images/page_delete.png", ViewContext) %>" alt="<%=Model.Localize("Remove") %>" title="<%=Model.Localize("Remove") %>" class="ibutton image remove" />
        <%=Html.Hidden("returnUri", Url.Page(Model.Item.Parent))%>
        <%=Html.OxiteAntiForgeryToken() %>
    </fieldset>
</form><%
        } 
    } else
    {
        %><%=Model.Localize("Removed") %><%
    } %>
</div><%
} %>
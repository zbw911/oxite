﻿<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="../Shared/Site.master" Inherits="System.Web.Mvc.ViewPage<OxiteViewModelItems<Post>>" %>
<%@ Import Namespace="Oxite.Extensions" %>
<%@ Import Namespace="Oxite.Models.Extensions" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
            <div class="sections">
                <div class="primary">
                    <h2 class="title"><% Html.RenderPartialFromSkin("ArchiveBreadcrumb"); %></h2>
                    <%=Html.PageState((IPageOfItems<Post>)Model.Items, (k, v) => Model.Localize(k, v)) %><% 
                    Html.RenderPartialFromSkin("PostListMedium");
                    %><%=Html.PostArchiveListPager((IPageOfItems<Post>)Model.Items, (k, v) => Model.Localize(k, v)) %>
                </div>
                <div class="secondary"><% 
                    Html.RenderPartialFromSkin("SideBar"); %>
                </div>
            </div>
</asp:Content>
<asp:Content ContentPlaceHolderID="Title" runat="server"><%
    ArchiveData archiveData = ((ArchiveContainer)Model.Container).ArchiveData; %>
    <%=Html.PageTitle(
        Model.Localize("ArchiveTitle","Archives"),
        archiveData.Year.ToString(),
        archiveData.Month > 0 ? new DateTime(archiveData.Year, archiveData.Month, 1).ToString("MMMM") : null,
        archiveData.Day > 0 ? archiveData.Day.ToString() : null
        ) %>
</asp:Content>
<asp:Content ContentPlaceHolderID="Scripts" runat="server"><%
    Html.RenderScriptTag("base.js"); %>
</asp:Content>

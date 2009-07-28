﻿<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="../Shared/Site.master" Inherits="System.Web.Mvc.ViewPage<OxiteViewModelItems<Comment>>" %>
<%@ Import Namespace="Oxite.Extensions" %>
<%@ Import Namespace="Oxite.Models.Extensions" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="sections">
        <div class="primary"> 
            <h2 class="title">Recent Comments</h2>
            <%=Html.PageState((IPageOfItems<Comment>)Model.Items, (k, v) => Model.Localize(k, v)) %><%
            Html.RenderPartialFromSkin("CommentListMedium");
            %><%=Html.CommentListPager((IPageOfItems<Comment>)Model.Items, (k, v) => Model.Localize(k, v)) %>
        </div>
        <div class="secondary"><% 
            Html.RenderPartialFromSkin("SideBar"); %>
        </div>
    </div>
</asp:Content>
<asp:Content ContentPlaceHolderID="Scripts" runat="server"><%
    Html.RenderScriptTag("base.js"); %>
</asp:Content>
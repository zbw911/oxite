﻿<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="../Shared/Site.master" Inherits="System.Web.Mvc.ViewPage<OxiteViewModelItems<Post>>" %>
<%@ Import Namespace="Oxite.Extensions" %>
<%@ Import Namespace="Oxite.Models.Extensions" %>
<%@ Import Namespace="Oxite.Modules.Blogs.Extensions"%>
<%@ Import Namespace="Oxite.Modules.MetaWeblog.Extensions" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
            <div class="sections">
                <div class="primary"><%
                    if (Model.Site.HasMultipleBlogs) { %><h2 class="title"><%=Model.Container.GetDisplayName() %></h2><% } 
                    %><%=Html.PageState((IPageOfItems<Post>)Model.Items, (k, v) => Model.Localize(k, v))%><%
                    Html.RenderPartialFromSkin("PostListMedium");
                    %><%=Html.PostListByBlogPager((IPageOfItems<Post>)Model.Items, (k,v) => Model.Localize(k,v), Model.Container.Name) %>
                </div>
                <div class="secondary"><% 
                    Html.RenderPartialFromSkin("SideBar"); %>
                </div>
            </div>
</asp:Content>
<asp:Content ContentPlaceHolderID="Title" runat="server">
    <%=Html.PageTitle(Model.Site.HasMultipleBlogs ? Model.Container.GetDisplayName() : null) %>
</asp:Content>
<asp:Content ContentPlaceHolderID="MetaDescription" runat="server">
    <%=Html.PageDescription(((Blog)Model.Container).Description) %>
</asp:Content>
<asp:Content ContentPlaceHolderID="Scripts" runat="server"><%
    Html.RenderScriptTag("base.js"); %>
</asp:Content>
<asp:Content ContentPlaceHolderID="HeadCustom" runat="server"><%
    Html.RenderFeedDiscoveryRss(string.Format("{0} Posts (RSS)", Model.Container.GetDisplayName()), Url.Container(Model.Container, "RSS"));
    Html.RenderFeedDiscoveryAtom(string.Format("{0} Posts (ATOM)", Model.Container.GetDisplayName()), Url.Container(Model.Container, "ATOM"));
    Html.RenderFeedDiscoveryRss(string.Format("All {0} Comments (RSS)", Model.Container.GetDisplayName()), Url.ContainerComments(Model.Container, "RSS"));
    Html.RenderFeedDiscoveryAtom(string.Format("All {0} Comments (ATOM)", Model.Container.GetDisplayName()), Url.ContainerComments(Model.Container, "ATOM"));
    Html.RenderFeedDiscoveryRss(string.Format("{0} (RSS)", Model.Site.DisplayName), Url.Posts("RSS"));
    Html.RenderFeedDiscoveryAtom(string.Format("{0} (ATOM)", Model.Site.DisplayName), Url.Posts("ATOM"));
    Html.RenderRsd(Model.Container.Name);
    Html.RenderLiveWriterManifest(); %>
</asp:Content>

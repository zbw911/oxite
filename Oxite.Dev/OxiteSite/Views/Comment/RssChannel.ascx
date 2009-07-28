<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<OxiteViewModelItems<Comment>>" %>
<%@ Import Namespace="Oxite.Extensions" %>
<%@ Import Namespace="Oxite.Modules.Blogs.Extensions" %>
    <channel>
        <title><%=Html.PageTitle(Model.Container.Name, "Comments")%></title>
        <description><%=Model.Site.Description %></description>
        <link><%=Url.Container(Model.Container) %></link>
        <language><%=Model.Site.LanguageDefault %></language>
        <image>
            <url><%=Url.AbsolutePath(Url.AppPath(Model.Site.FavIconUrl.Replace(".ico", ".png"))) %></url>
            <title><%=Model.Site.DisplayName %></title>
            <link><%=Url.Container(Model.Container)%></link>
            <width>64</width>
            <height>64</height>
        </image><%
    foreach (Comment c in Model.Items)
    {
        Html.RenderPartialFromSkin("RssChannelItem", new OxiteViewModelPartial<Comment>(Model, c));
    } %>
    </channel>
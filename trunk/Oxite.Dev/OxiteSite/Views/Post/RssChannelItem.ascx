<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<OxiteViewModelPartial<Post>>" %>
<%@ Import Namespace="Oxite.Extensions" %>
<%@ Import Namespace="Oxite.Modules.Blogs.Extensions" %><%
    Post post = Model.PartialModel;
    string postUrl = Url.AbsolutePath(Url.Post(post)); %>
        <item>
            <dc:creator><%=Html.Encode(post.Creator.Name)%></dc:creator>
            <title><%=Html.Encode(post.Title)%></title>
            <description><%=Html.Encode(post.Body)%></description>
            <link><%=postUrl %></link>
            <guid isPermaLink="true"><%=postUrl %></guid>
            <pubDate><%=post.Published.Value.ToStringForFeed()%></pubDate><%
        if (Model.RootModel.Site.HasMultipleBlogs)
        { %>
            <category><%=post.Blog.Name %></category><%
        }
        foreach (Tag tag in post.Tags)
        { %>
            <category><%=tag.Name %></category><%
        } %>
        </item>
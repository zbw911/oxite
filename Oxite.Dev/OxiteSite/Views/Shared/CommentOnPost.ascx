<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<OxiteViewModelItem<Post>>" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="Oxite.Extensions" %>
<%@ Import Namespace="Oxite.Modules.Blogs.Extensions" %>
    <form method="post" id="comment" action="<%=Url.AddCommentToPost(Model.Item) %>#comment" class="<%=Model.User != null && Model.User.IsAuthenticated ? "user" : "anon" %>" ><%
        if (Request.QueryString.Get("pending") == bool.TrueString)
        { %>
        <div class="message info"><%=Model.Localize("PendingComment", "Thanks for the comment. It'll show up here pending admin approval.") %></div><%
        } %>
        <%=Html.ValidationSummary() %>
        <%
        if (Model.User != null && Model.User.IsAuthenticated)
            Html.RenderPartialFromSkin("CommentFormAuthenticated", new OxiteViewModelPartial<Comment>(Model, null));
        else
            Html.RenderPartialFromSkin("CommentFormAnonymous", new OxiteViewModelPartial<Comment>(Model, null));
        %>
    </form>

<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<OxiteViewModelPartial<Comment>>" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="Oxite.Extensions" %>
<%@ Import Namespace="Oxite.Modules.Blogs.Extensions" %>
    <form method="post" id="comment" action="<%=Url.AddCommentToPost(Model.PartialModel.Post) %>#comment" class="<%=Model.RootModel.User.IsAuthenticated ? "user" : "anon" %>">
        <div class="replyingto"><%=
            Model.RootModel.Localize("ReplyingTo", "Replying to") %> <%=
            Html.Link(Model.PartialModel.CreatorName.CleanText(), Url.CommentPermalinkReply(Model.PartialModel)) %>. <%=
            Html.Link(Model.RootModel.Localize("Cancel"), Url.Comment(Model.PartialModel)) %> - <%=
            Html.Link(Model.RootModel.Localize("CommentOnPost", "Comment on the post"), Url.CommentOnPost(Model.PartialModel.Post)) %></div><%
        if (Request.QueryString.Get("pending") == bool.TrueString)
        { %>
        <div class="message info"><%=Model.RootModel.Localize("PendingComment", "Thanks for the comment. It'll show up here pending admin approval.") %></div><%
        } %>
        <%=Html.ValidationSummary() %>
        <fieldset>
            <%=Html.Hidden("parentID", Model.PartialModel.ID.ToString()) %>
        </fieldset>
        <%
        if (Model.RootModel.User != null && Model.RootModel.User.IsAuthenticated)
            Html.RenderPartialFromSkin("CommentFormAuthenticated");
        else
            Html.RenderPartialFromSkin("CommentFormAnonymous");
        %>
    </form>

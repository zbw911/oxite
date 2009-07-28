<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<OxiteViewModelItemItems<Post, Comment>>" %>
<%@ Import Namespace="Oxite.Extensions" %>
<div id="comments"><% 
    string statusClass = "status";
    if (Model.Item.CommentCount < 1)
        statusClass = statusClass + " empty";
    %>
	<div class="<%=statusClass %>">
		<h3><%= string.Format(Model.Item.CommentCount == 1 ? Model.Localize("SingleCommentStatus", "{0} Comment") : Model.Localize("MultiCommentStatus", "{0} Comments"), Model.Item.CommentCount)%></h3>
		<div><a href="#comment"><%=Model.Localize("leave your own")%></a></div>
	</div>
	<%
    Html.RenderPartialFromSkin("CommentListMedium", new OxiteViewModelItems<Comment>(Model) { Items = Model.Items });

    if (!Model.CommentingDisabled && string.IsNullOrEmpty(ViewData["CommentingOnComment"] as string))
    {
        Html.RenderPartialFromSkin("CommentOnPost");
    }
    %>
</div>
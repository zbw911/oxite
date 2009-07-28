<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<OxiteViewModelPartial<Comment>>" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="Oxite.Extensions" %>
<%@ Import Namespace="Oxite.Models.Extensions" %>
<fieldset class="info">
    <legend><%=Model.RootModel.Localize("Your Information") %></legend>
    <div id="comment_grav"><%= Html.Gravatar(Model.RootModel, "48")%></div>
    <p class="gravatarhelp"><%= string.Format(Model.RootModel.Localize("&lt;-- It's a {0}"), Html.Link(Model.RootModel.Localize("gravatar"), "http://gravatar.com/site/signup")) %></p>
    <div class="name">
        <label for="comment_name"><%=Model.RootModel.Localize("Name") %></label>
        <%=Html.TextBox("name", (Request.Form["name"] ?? (Model.RootModel.User != null ? Model.RootModel.User.Name : "")), new { id = "comment_name", @class = "text", title = Model.RootModel.Localize("comment_name", "Your name...") })%><%= Html.ValidationMessage("UserBase.Name", "You must provide a name.") %>
    </div>
    <div class="email">
        <label for="comment_email"><%=Model.RootModel.Localize("Email") %></label>
        <%=Html.TextBox("email", (Request.Form["email"] ?? (Model.RootModel.User != null ? Model.RootModel.User.Email : "")), new { id = "comment_email", @class = "text", title = Model.RootModel.Localize("comment_email","Your email...") }) %><%= Html.ValidationMessage("PostSubscription.Email", "Valid email required to subscribe.") %><%= Html.ValidationMessage("UserBase.Email", "Invalid email.") %>
        <span><%=Model.RootModel.Localize("email saved for notifications but never distributed") %></span>
    </div>
    <div class="url">
        <label for="comment_url"><%=Model.RootModel.Localize("URL") %></label>
        <%=Html.TextBox("url", (Request.Form["url"] ?? (Model.RootModel.User != null ? Model.RootModel.User.Url : "")), new { id = "comment_url", @class = "text", title = Model.RootModel.Localize("comment_url", "Your home on the interwebs (URL)...") })%><%= Html.ValidationMessage("UserBase.Url", "URL looks a little off.") %>
    </div>
    <div class="remember">
        <%=Html.CheckBox("remember", Request.Form.IsTrue("remember") || (Model.RootModel.User != null && !string.IsNullOrEmpty(Model.RootModel.User.Email)), new { id = "comment_remember" })%>
        <label for="comment_remember"><%=Model.RootModel.Localize("Remember your info?") %></label>
    </div>
    <div class="subscribe">
        <%=Html.CheckBox("subscribe", Request.Form.IsTrue("subscribe"), new { id = "comment_subscribe" })%>
        <label for="comment_subscribe"><%=Model.RootModel.Localize("Subscribe?") %></label>
    </div>
    <div class="submit">
        <input type="submit" value="<%=Model.RootModel.Localize("Submit Comment") %>" id="comment_submit" class="submit button" />
    </div>
</fieldset>
<fieldset class="comment">
    <legend><%=Model.RootModel.Localize("your comment") %></legend>
    <label for="comment_body"><%= Model.RootModel.Localize("Leave a comment...") %></label><%=Html.ValidationMessage("Comment.Body") %>
    <%=Html.TextArea("body", Request.Form["body"] ?? "", 12, 60, new { id = "comment_body", title = Model.RootModel.Localize("comment_body", "Leave a comment...") })%>
</fieldset>
﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<OxiteViewModelPartial<Comment>>" %>
<%@ Import Namespace="Oxite.Extensions" %>
<div class="avatar"><%=Html.Gravatar(Model.RootModel, "48")%></div>
<fieldset class="comment">
    <legend><%=Model.RootModel.Localize("your comment") %></legend>
    <div>
        <label for="comment_body"><%=Model.RootModel.Localize("Leave a comment...") %></label><%=Html.ValidationMessage("Comment.Body") %>
        <%=Html.TextArea("body", Request.Form["body"] ?? "", 12, 60, new { id = "comment_body", @class = "authed", title = Model.RootModel.Localize("comment_body", "Leave a comment...") })%>
    </div>
    <div class="subscribe">
        <%=Html.CheckBox("subscribe", Request.Form.IsTrue("subscribe"), Model.RootModel.Localize("Subscribe?"), !string.IsNullOrEmpty(Model.RootModel.User.Email))%>
    </div>
    <div class="submit">
        <input type="submit" value="<%=Model.RootModel.Localize("Submit Comment") %>" id="comment_submit" class="submit button" />
        <%=Html.OxiteAntiForgeryToken() %>
    </div>
</fieldset>
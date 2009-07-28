<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<OxiteViewModelPartial<Comment>>" %>
<%@ Import Namespace="Oxite.Extensions" %> 
<%@ Import Namespace="Oxite.Modules.Blogs.Extensions" %>
<span><%=Model.RootModel.Localize("said") %><br /><%= 
        Html.Link(Model.PartialModel.Created.ToRelativeDateTime(), Url.Comment(Model.PartialModel)) %></span>
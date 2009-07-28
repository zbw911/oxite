<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<OxiteViewModel>" %>
<%@ Import Namespace="Oxite.Extensions" %>
<% 
    Html.RenderPartialFromSkin("Search");
    Html.RenderPartialFromSkin("Archives", new OxiteViewModelPartial<ArchiveViewModel>(Model, Model.GetModelItem<ArchiveViewModel>())); %>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<OxiteViewModelItem<PageInput>>" %>
<%@ Import Namespace="Oxite.Extensions" %>
<%@ Import Namespace="Oxite.Modules.CMS.Models" %><%

PageListViewModel pageListViewModel = Model.GetModelItem<PageListViewModel>();
List<SelectListItem> allPages = pageListViewModel.Pages.OrderBy(p => p.Path).Select(p => new SelectListItem { Value = p.ID.ToString(), Text = p.Path, Selected = p.ID == Model.Item.ParentID ? true : false }).ToList();
allPages.Insert(0, new SelectListItem { Value = Guid.Empty.ToString(), Text = "/"});

var page = Model.GetModelItem<Oxite.Models.Page>();
if (page != null)
{
    allPages.RemoveAll(sli => sli.Text.StartsWith(page.Path));
}

%>
<fieldset class="title">
    <%=Html.ValidationMessage("Page.Title", Model.Localize("Title isn't valid.")) %>
    <%=Html.TextBox(
        "title",
        m => m.Item.Title,
        Model.Localize("Page.Title", "Title"),
        new { id = "page_title", @class = "text", size = "60" }
        ) %>
    <%=Html.OxiteAntiForgeryToken() %>
</fieldset>
<fieldset class="url">
    <label for="page_slug"><%=Model.Localize("Page.Url", "Location") %></label><%=Html.ValidationMessage("Page.Slug", Model.Localize("URL isn't valid.")) %>
    <span><%=string.Format(
                "{0}{1}",
                Url.AbsolutePath(Url.Home()),
                allPages.Count() > 0
                    ? Html.DropDownList(
                        "parentID",
                        allPages,
                        a => string.Format("{0}{1}", a.Text, a.Text.EndsWith("/") ? "" : "/"),
                        a => a.Value,
                        Model.Item.ParentID.ToString(),
                        new { id = "page_blog" },
                        true)
                    : "/" + Html.Hidden("parentID", Guid.Empty.ToString())
              ) 
    %></span>
    <%=Html.TextBox(
        "slug", 
        Model.Item.Slug,
        new { id = "page_slug", @class = "text", size = "60" }
        ) %>
</fieldset>
<fieldset class="content">
    <%=Html.ValidationMessage("Page.Body", Model.Localize("Page.BodyEdit", "Body isn't valid.")) %>
    <%=Html.TextArea(
        "body",
        m => m.Item.Body,
        21 /*rows*/,
        100 /*cols*/,
        Model.Localize("Page.Body", "Content"),
        new { @class = "html" }
        ) %>
</fieldset>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<OxiteViewModelPartial<Plugin>>" %>
<%@ Import Namespace="Oxite.Plugins.Extensions"%>
<%@ Import Namespace="Oxite.Extensions"%>
<%@ Import Namespace="Oxite.Modules.Plugins.Extensions" %>
<div class="managePlugin"><%
if (Model.PartialModel.Container.GetDateModified() != null)
{ %>
    <a class="editPlugin ibutton edit" href="<%=Url.PluginEdit(Model.PartialModel) %>" title="<%=Model.RootModel.Localize("Edit") %>"><%=Html.SkinImage("/images/page_edit.png", Model.RootModel.Localize("Edit"), new { width = 16, height = 16 })%></a>
    <%
}
bool? installed = (bool?) ViewContext.RouteData.Values["installed"];
string returnUrl = installed != null && (bool) installed ? Url.PluginsInstalled() : Url.Plugins();
        
if (Model.PartialModel.Container.CompilationException == null)
{
    if (Model.PartialModel.Enabled)
    {
    %><form class="disablePlugin modifyPlugin" method="POST" action="<%=Url.PluginDisable(Model.PartialModel) %>">
        <input type="image" src="<%=Url.CssPath("images/plugin_disabled.png", ViewContext) %>" alt="<%=Model.RootModel.Localize("Disable") %>" title="<%=Model.RootModel.Localize("Disable") %>" class="ibutton image pluginDisable" />
        <input type="hidden" name="returnUrl" value="<%=returnUrl %>">
        <%=Html.OxiteAntiForgeryToken() %>
    </form><%
    }
    else 
    {
    %><form class="enablePlugin modifyPlugin" method="POST" action="<%=Url.PluginEnable(Model.PartialModel) %>">
        <input type="image" src="<%=Url.CssPath("images/plugin.png", ViewContext) %>" alt="<%=Model.RootModel.Localize("Enable") %>" title="<%=Model.RootModel.Localize("Enable") %>" class="ibutton image pluginEnable" />
        <input type="hidden" name="returnUrl" value="<%=returnUrl %>">
        <%=Html.OxiteAntiForgeryToken() %>
    </form><%
    }
}
    %>
    <form class="uninstallPlugin modifyPlugin" method="POST" action="<%=Url.PluginUninstall(Model.PartialModel) %>">
        <input type="image" src="<%=Url.CssPath("images/delete.png", ViewContext) %>" alt="<%=Model.RootModel.Localize("Uninstall") %>" title="<%=Model.RootModel.Localize("Uninstall") %>" class="ibutton image remove" />
        <input type="hidden" name="returnUrl" value="<%=returnUrl %>">
        <%=Html.OxiteAntiForgeryToken() %>
    </form>
</div>
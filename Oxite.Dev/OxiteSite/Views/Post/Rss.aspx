﻿<%@ Page Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage<OxiteViewModelItems<Post>>" %>
<%@ Import Namespace="Oxite.Extensions"
%><rss version="2.0" xmlns:dc="http://purl.org/dc/elements/1.1/"><%
    Html.RenderPartialFromSkin("RssChannel", Model); %>
</rss>

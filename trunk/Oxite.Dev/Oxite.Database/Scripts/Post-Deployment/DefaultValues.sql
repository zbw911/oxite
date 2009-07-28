--Sites
DECLARE @SiteID uniqueidentifier
SET @SiteID = '4F36436B-0782-4a94-BB4C-FD3916734C03'

IF NOT EXISTS(SELECT * FROM oxite_Site WHERE SiteHost = 'http://localhost:30913')
BEGIN
	INSERT INTO
		oxite_Site
	(
		SiteID,
		SiteHost,
		SiteName,
		SiteDisplayName,
		SiteDescription,
		LanguageDefault,
		TimeZoneOffset,
		PageTitleSeparator,
		FavIconUrl,
		CommentStateDefault,
		IncludeOpenSearch,
		AuthorAutoSubscribe,
		PostEditTimeout,
		GravatarDefault,
		SkinsPath,
		SkinsScriptsPath,
		SkinsStylesPath,
		Skin,
		AdminSkin,
		ServiceRetryCountDefault,
		HasMultipleBlogs,
		RouteUrlPrefix,
		CommentingDisabled,
		PluginsPath
	)
	VALUES
	(
		@SiteID,
		'http://localhost:30913',
		'Oxite',
		'Oxite Sample',
		'This is the Oxite Sample description',
		'en',
		-8,
		' - ',
		'/Content/icons/flame.ico',
		'Normal',
		1,
		1,
		24,
		'http://mschnlnine.vo.llnwd.net/d1/oxite/gravatar.jpg',
		'/Skins',
		'/Scripts',
		'/Styles',
		'Default',
		'Admin',
		10,
		0,
		'',
		0,
		'/Plugins'
	)
END

--Modules
IF NOT EXISTS(SELECT * FROM oxite_Module WHERE ModuleName = 'AspNetCache')
BEGIN
	INSERT INTO
		oxite_Module
	(
		SiteID,
		ModuleName,
		ModuleOrder,
		ModuleType,
		Enabled,
		IsSystem
	)
	VALUES
	(
		@SiteID,
		'AspNetCache',
		0,
		'Oxite.Modules.AspNetCache.AspNetCacheModule, Oxite.AspNetCache',
		1,
		1
	)
END

IF NOT EXISTS(SELECT * FROM oxite_Module WHERE ModuleName = 'Membership')
BEGIN
	INSERT INTO
		oxite_Module
	(
		SiteID,
		ModuleName,
		ModuleOrder,
		ModuleType,
		Enabled,
		IsSystem
	)
	VALUES
	(
		@SiteID,
		'Membership',
		1,
		'Oxite.Modules.Membership.MembershipModule, Oxite.Membership',
		1,
		1
	)
END

IF NOT EXISTS(SELECT * FROM oxite_Module WHERE ModuleName = 'FormsAuthentication')
BEGIN
	INSERT INTO
		oxite_Module
	(
		SiteID,
		ModuleName,
		ModuleOrder,
		ModuleType,
		Enabled,
		IsSystem
	)
	VALUES
	(
		@SiteID,
		'FormsAuthentication',
		2,
		'Oxite.Modules.FormsAuthentication.FormsAuthenticationModule, Oxite.FormsAuthentication',
		1,
		0
	)
END

IF NOT EXISTS(SELECT * FROM oxite_Module WHERE ModuleName = 'Core')
BEGIN
	INSERT INTO
		oxite_Module
	(
		SiteID,
		ModuleName,
		ModuleOrder,
		ModuleType,
		Enabled,
		IsSystem
	)
	VALUES
	(
		@SiteID,
		'Core',
		3,
		'Oxite.Modules.Core.OxiteModule, Oxite.Core',
		1,
		1
	)
END

IF NOT EXISTS(SELECT * FROM oxite_Module WHERE ModuleName = 'Plugins')
BEGIN
	INSERT INTO
		oxite_Module
	(
		SiteID,
		ModuleName,
		ModuleOrder,
		ModuleType,
		Enabled,
		IsSystem
	)
	VALUES
	(
		@SiteID,
		'Plugins',
		4,
		'Oxite.Modules.Plugins.PluginsModule, Oxite.Plugins',
		1,
		1
	)
END

IF NOT EXISTS(SELECT * FROM oxite_Module WHERE ModuleName = 'Blogs')
BEGIN
	INSERT INTO
		oxite_Module
	(
		SiteID,
		ModuleName,
		ModuleOrder,
		ModuleType,
		Enabled,
		IsSystem
	)
	VALUES
	(
		@SiteID,
		'Blogs',
		5,
		'Oxite.Modules.Blogs.BlogsModule, Oxite.Blogs',
		1,
		1
	)
END

IF NOT EXISTS(SELECT * FROM oxite_Module WHERE ModuleName = 'CMS')
BEGIN
	INSERT INTO
		oxite_Module
	(
		SiteID,
		ModuleName,
		ModuleOrder,
		ModuleType,
		Enabled,
		IsSystem
	)
	VALUES
	(
		@SiteID,
		'CMS',
		6,
		'Oxite.Modules.CMS.CMSModule, Oxite.CMS',
		1,
		1
	)
END

IF NOT EXISTS(SELECT * FROM oxite_Module WHERE ModuleName = 'MetaWeblog')
BEGIN
	INSERT INTO
		oxite_Module
	(
		SiteID,
		ModuleName,
		ModuleOrder,
		ModuleType,
		Enabled,
		IsSystem
	)
	VALUES
	(
		@SiteID,
		'MetaWeblog',
		7,
		'Oxite.Modules.MetaWeblog.MetaWeblogModule, Oxite.MetaWeblog',
		1,
		1
	)
END

IF NOT EXISTS(SELECT * FROM oxite_Module WHERE ModuleName = 'Search')
BEGIN
	INSERT INTO
		oxite_Module
	(
		SiteID,
		ModuleName,
		ModuleOrder,
		ModuleType,
		Enabled,
		IsSystem
	)
	VALUES
	(
		@SiteID,
		'Search',
		8,
		'Oxite.Modules.Search.SearchModule, Oxite.Search',
		1,
		1
	)
END

IF NOT EXISTS(SELECT * FROM oxite_Module WHERE ModuleName = 'Setup')
BEGIN
	INSERT INTO
		oxite_Module
	(
		SiteID,
		ModuleName,
		ModuleOrder,
		ModuleType,
		Enabled,
		IsSystem
	)
	VALUES
	(
		@SiteID,
		'Setup',
		9,
		'Oxite.Modules.Setup.SetupModule, Oxite.Setup',
		1,
		1
	)
END

IF NOT EXISTS(SELECT * FROM oxite_Module WHERE ModuleName = 'BlogML')
BEGIN
	INSERT INTO
		oxite_Module
	(
		SiteID,
		ModuleName,
		ModuleOrder,
		ModuleType,
		Enabled,
		IsSystem
	)
	VALUES
	(
		@SiteID,
		'BlogML',
		10,
		'Oxite.Modules.BlogML.BlogMLModule, Oxite.BlogML',
		1,
		1
	)
END

IF NOT EXISTS(SELECT * FROM oxite_Module WHERE ModuleName = 'Site')
BEGIN
	INSERT INTO
		oxite_Module
	(
		SiteID,
		ModuleName,
		ModuleOrder,
		ModuleType,
		Enabled,
		IsSystem
	)
	VALUES
	(
		@SiteID,
		'Site',
		11,
		'OxiteSite.App_Code.OxiteSite.OxiteSiteModule, OxiteSite',
		1,
		0
	)
END

--PostViewType
IF NOT EXISTS(SELECT * FROM oxite_PostViewType WHERE PostViewTypeName = 'Post-Web')
BEGIN
	INSERT INTO
		oxite_PostViewType
	(
		PostViewTypeID,
		PostViewTypeName
	)
	VALUES
	(
		'A38FD696-22E2-4612-A392-E73FCAABB61D',
		'Post-Web'
	)
END

IF NOT EXISTS(SELECT * FROM oxite_PostViewType WHERE PostViewTypeName = 'Post-RSS')
BEGIN
	INSERT INTO
		oxite_PostViewType
	(
		PostViewTypeID,
		PostViewTypeName
	)
	VALUES
	(
		'6A945758-48C3-4e43-A1E3-AED601F5F022',
		'Post-RSS'
	)
END

IF NOT EXISTS(SELECT * FROM oxite_PostViewType WHERE PostViewTypeName = 'Post-ATOM')
BEGIN
	INSERT INTO
		oxite_PostViewType
	(
		PostViewTypeID,
		PostViewTypeName
	)
	VALUES
	(
		'E3080032-0203-42c4-8B36-76A8168FB474',
		'Post-ATOM'
	)
END

--Languages
DECLARE @LanguageID uniqueidentifier
SET @LanguageID = '2E0FDFBD-99DD-4970-BAED-2B9653672FC1'

IF NOT EXISTS(SELECT * FROM oxite_Language WHERE LanguageID = @LanguageID)
	INSERT INTO
		oxite_Language
	(
		LanguageID,
		LanguageName,
		LanguageDisplayName
	)
	VALUES
	(
		@LanguageID,
		'en',
		'English'
	)

--Users
DECLARE @User1ID uniqueidentifier
SET @User1ID = '655D3498-03DB-4075-A80E-6514EC6BB6E2'

IF NOT EXISTS(SELECT * FROM oxite_User WHERE UserID = @User1ID)
BEGIN
	INSERT INTO
		oxite_User
	(
		UserID,
		Username,
		DisplayName,
		Email,
		HashedEmail,
		DefaultLanguageID,
		Status
	)
	VALUES
	(
		@User1ID,
		'Admin',
		'Oxite Administrator',
		'',
		'8d33d9c3c448f2c14d722900c2bd6098',
		@LanguageID,
		1
	)
	
	INSERT INTO
		oxite_UserModuleData
	(
		UserID,
		ModuleName,
		Data
	)
	VALUES
	(
		@User1ID,
		'FormsAuthentication',
		'NaCl|BQWPtrSvaXLSzkU6vOM4XeV/080hsgtsVIjLEPFny7k='
	)
END

DECLARE @User2ID uniqueidentifier
SET @User2ID = 'C0981693-799A-4331-B2DD-C83084538669'

IF NOT EXISTS(SELECT * FROM oxite_User WHERE UserID = @User2ID)
	INSERT INTO
		oxite_User
	(
		UserID,
		Username,
		DisplayName,
		Email,
		HashedEmail,
		DefaultLanguageID,
		Status
	)
	VALUES
	(
		@User2ID,
		'Anonymous',
		'',
		'',
		'',
		@LanguageID,
		1
	)

--UserLanguages
IF NOT EXISTS(SELECT * FROM oxite_UserLanguage WHERE UserID = @User1ID AND LanguageID = @LanguageID)
	INSERT INTO
		oxite_UserLanguage
	(
		UserID,
		LanguageID
	)
	VALUES
	(
		@User1ID,
		@LanguageID
	)

IF NOT EXISTS(SELECT * FROM oxite_UserLanguage WHERE UserID = @User2ID AND LanguageID = @LanguageID)
	INSERT INTO
		oxite_UserLanguage
	(
		UserID,
		LanguageID
	)
	VALUES
	(
		@User2ID,
		@LanguageID
	)

--Roles
DECLARE @Role1ID uniqueidentifier
SET @Role1ID = '10C0B1FD-F284-4a7d-BBE0-38A671E2BD34'

IF NOT EXISTS(SELECT * FROM oxite_Role WHERE RoleName = 'Admin')
	INSERT INTO
		oxite_Role
	(
		GroupRoleID,
		RoleID,
		RoleName,
		RoleType
	)
	VALUES
	(
		@Role1ID,
		@Role1ID,
		'Admin',
		15
	)

--SiteRoleUserRelationships
IF NOT EXISTS(SELECT * FROM oxite_SiteRoleUserRelationship WHERE SiteID = @SiteID AND RoleID = @Role1ID AND UserID = @User1ID)
	INSERT INTO
		oxite_SiteRoleUserRelationship
	(
		SiteID,
		RoleID,
		UserID
	)
	VALUES
	(
		@SiteID,
		@Role1ID,
		@User1ID
	)

--Blogs
DECLARE @BlogID uniqueidentifier
SET @BlogID = '66F2AF76-8F03-4621-8114-CAA137AFF185'

IF NOT EXISTS(SELECT * FROM oxite_Blog WHERE BlogID = @BlogID)
	INSERT INTO
		oxite_Blog
	(
		SiteID,
		BlogID,
		BlogName,
		DisplayName,
		Description,
		CommentingDisabled,
		CreatedDate,
		ModifiedDate
	)
	VALUES
	(
		@SiteID,
		@BlogID,
		'Blog',
		'Oxite Sample',
		'This is the Oxite Sample description',
		0,
		getUtcDate(),
		getUtcDate()
	)

--Posts
DECLARE @PostID uniqueidentifier
SET @PostID = 'C5FA9D2A-24B2-45e2-955A-92E88A34260C'

IF NOT EXISTS(SELECT * FROM oxite_Post WHERE PostID = @PostID)
	INSERT INTO oxite_Post (BlogID, PostID, CreatorUserID, Title, Body, BodyShort, State, Slug, CommentingDisabled, CreatedDate, ModifiedDate, PublishedDate, SearchBody) VALUES
	(
		@BlogID,
		@PostID,
		@User1ID,
		'World.Hello()',
		'<p>Welcome to Oxite! This is a sample application targeting developers built on <a href="http://asp.net/mvc">ASP.NET MVC</a>. Make any changes you like. If you build a feature you think other developers would be interested in and would like to share your code go to the <a href="http://www.codeplex.com/oxite">Oxite Code Plex project</a> to see how you can contribute.</p><p>To get started, sign in with "Admin" and "pa$$w0rd" and click on the Admin tab.</p><p>For more information about <a href="http://oxite.net">Oxite</a> visit the default <a href="/About">About</a> page.</p>',
		'<p>Welcome to Oxite! This is a sample application targeting developers built on <a href="http://asp.net/mvc">ASP.NET MVC</a>. Make any changes you like. If you build a feature you think other developers would be interested in and would like to share your code go to the <a href="http://www.codeplex.com/oxite">Oxite Code Plex project</a> to see how you can contribute.</p><p>To get started, sign in with "Admin" and "pa$$w0rd" and click on the Admin tab.</p><p>For more information about <a href="http://oxite.net">Oxite</a> visit the default <a href="/About">About</a> page.</p>',
		1,
		'Hello-World',
		0,
		getUtcDate(),
		getUtcDate(),
		getUtcDate(),
		''
	)

DECLARE @TagID uniqueidentifier
SET @TagID = newid()

IF NOT EXISTS(SELECT * FROM oxite_Tag WHERE TagID = @TagID)
	INSERT INTO
		oxite_Tag
	(
		ParentTagID,
		TagID,
		TagName,
		CreatedDate
	)
	VALUES
	(
		@TagID,
		@TagID,
		'Oxite',
		getUtcDate()
	)

IF NOT EXISTS(SELECT * FROM oxite_PostTagRelationship WHERE TagID = @TagID AND PostID = @PostID)
	INSERT INTO
		oxite_PostTagRelationship
	(
		TagID,
		PostID
	)
	VALUES
	(
		@TagID,
		@PostID
	)

UPDATE
	oxite_Post
SET
	SearchBody = Title + ' ' + (SELECT DisplayName FROM oxite_User WHERE UserID = CreatorUserID) + ' ' + Body

DECLARE @PageID uniqueidentifier
SET @PageID = 'D2D69195-F49A-4f68-AA3C-D7B4CF69F695'

IF NOT EXISTS(SELECT * FROM oxite_Page WHERE PageID = @PageID)
	INSERT INTO oxite_Page (SiteID, ParentPageID, PageID, CreatorUserID, Title, Body, State, Slug, CreatedDate, ModifiedDate, PublishedDate) VALUES
	(
		@SiteID,
		@PageID,
		@PageID,
		@User1ID,
		'About',
		'<p>Welcome to the Oxite Sample! Since this is a sample, we thought we would use this handy about page to explain a few things about the code and about the thoughts that went into its creation.</p>
<h3>What is this?</h3>    
<p>This is a simple blog engine written using <a href="http://asp.net/mvc">ASP.NET MVC</a>, and is designed with two main goals:</p>
<ol class="normal">
	<li>To provide a sample of ''core blog functionality'' in a reusable fashion. &nbsp;Blogs are simple and well understood by many developers, but the set of basic functions that a blog needs to implement (trackbacks, rss, comments, etc.) are fairly complex.  Hopefully this code helps.</li>
	<li>To provide a real-world sample written using <a href="http://asp.net/mvc">ASP.NET MVC</a>.</li>
</ol>
<p>We aren''t a sample-building team (more on what we are in a bit). &nbsp;We couldn''t sit down and build this code base just to give out to folks, so this code is also the foundation of a real project of ours, <a href="http://visitmix.com">MIX Online</a>. &nbsp;We also created this project to be the base of our own personal blogs as well; check out <a href="http://www.codeplex.com/oxite/Wiki/View.aspx?title=oxitesites&amp;referringTitle=Home">this page on CodePlex to see a list of sites that use Oxite</a> (and use the comments area to tell us about your site).</p>
<h3>What this isn''t</h3>
<p>This is not an ''off the shelf'' blogging package. &nbsp;If you aren''t a developer and just want to get blogging then you should look at one of these great blogging products: <a href="http://graffiticms.com/">Graffiti</a>, <a href="http://subtextproject.com/">SubText</a>, <a href="http://www.dotnetblogengine.net/">Blog Engine .NET</a>, <a href="http://dasblog.info/">dasBlog</a> or a hosted service like <a href="http://wordpress.com/">Wordpress</a></p>
<p>Oxite is also not ready to be an enterprise blogging solution (for you and a thousand other bloggers at your company), although we did design it to be capable of hosting multiple blogs. &nbsp;For that type of solution, a set of provisioning tools to create new blogs would need to be added. &nbsp;Oxite is code though, so you can extend it and customize it to support whatever you need.</p>
<h3>Where to go from here (expanding on this sample)</h3>
<p>You can extend Oxite in whatever way you need or wish, but if you are looking for some ideas here are a few thoughts we''ve already had around new functionality:</p>
<ul>
	<li>Adding a rich-text-editor for post and page editing. &nbsp;We use <a href="http://writer.live.com">Windows Live Writer</a> to post and edit our blog posts, so this isn''t a real issue for our day to day use of the site, but adding an editor like <a href="http://developer.yahoo.com/yui/editor/">http://developer.yahoo.com/yui/editor/</a> would be great.</li>
	<li>Adding UI for managing the creation of new blogs, setting up users and user permissions, etc. &nbsp;If you decided to use Oxite to host many blogs together on one site, or to use the same Oxite database to run many sites (yes, it can do both of those!) then it would be nice to have some UI for managing all those contributors.</li>
	<li>And whatever great idea you have!</li>
</ul>
<h3>Getting the code, reporting bugs, and contributing to this project</h3>
<p><a href="http://codeplex.com/Oxite">Oxite is hosted on CodePlex</a>, so you can grab the latest code from there (you can <a href="http://www.codeplex.com/oxite/SourceControl/ListDownloadableCommits.aspx">see all of our check-ins</a> and also <a href="http://www.codeplex.com/oxite/Release/ProjectReleases.aspx">specific releases</a> when we feel like significant changes have been made), read discussions, file bugs and even submit suggestions for changes. &nbsp;If you''ve made some code changes that you feel should make it back into the Oxite code, then CodePlex is the place to tell us about it!</p>
<h3>About us</h3>
<p>Oxite is a project built by the team behind <a href="http://channel9.msdn.com/">Channel 9</a> (and <a href="http://channel8.msdn.com/">Channel 8</a>, <a href="http://on10.net/">Channel 10</a>, <a href="http://edge.technet.com/">TechNet Edge</a>, <a href="http://visitmix.com/">Mix Online</a>): Erik Porter, Nathan Heskew, Mike Sampson and Duncan Mackenzie. &nbsp;You can find out more about our team and our projects in our <a href="http://channel9.msdn.com/tags/evnet/">various posts and videos on Channel 9</a>.</p>
<p><a href="http://validator.w3.org/check?uri=referer"><img src="/Content/images/valid-xhtml10-blue.png" alt="Valid XHTML 1.0 Strict" height="31" width="88" /></a> <a href="http://jigsaw.w3.org/css-validator/"><img style="border:0;width:88px;height:31px" src="/Content/images/vcss-blue.gif" alt="Valid CSS!" /></a> <a href="http://validator.w3.org/feed/check.cgi"><img src="/Content/images/valid-rss.png" alt="[Valid RSS]" title="Validate my RSS feed" /></a></p>',
		1,
		'About',
		getUtcDate(),
		getUtcDate(),
		getUtcDate()
	)

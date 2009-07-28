﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4918
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Oxite.Modules.CMS.Repositories.SqlServer
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[System.Data.Linq.Mapping.DatabaseAttribute(Name="Oxite.Database")]
	public partial class OxiteCMSDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void Insertoxite_Page(oxite_Page instance);
    partial void Updateoxite_Page(oxite_Page instance);
    partial void Deleteoxite_Page(oxite_Page instance);
    partial void Insertoxite_Site(oxite_Site instance);
    partial void Updateoxite_Site(oxite_Site instance);
    partial void Deleteoxite_Site(oxite_Site instance);
    partial void Insertoxite_User(oxite_User instance);
    partial void Updateoxite_User(oxite_User instance);
    partial void Deleteoxite_User(oxite_User instance);
    partial void Insertoxite_Language(oxite_Language instance);
    partial void Updateoxite_Language(oxite_Language instance);
    partial void Deleteoxite_Language(oxite_Language instance);
    #endregion
		
		public OxiteCMSDataContext() : 
				base("Data Source=.\\SQLEXPRESS;Initial Catalog=Oxite.Database;Integrated Security=True", mappingSource)
		{
			OnCreated();
		}
		
		public OxiteCMSDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public OxiteCMSDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public OxiteCMSDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public OxiteCMSDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<oxite_Page> oxite_Pages
		{
			get
			{
				return this.GetTable<oxite_Page>();
			}
		}
		
		public System.Data.Linq.Table<oxite_Site> oxite_Sites
		{
			get
			{
				return this.GetTable<oxite_Site>();
			}
		}
		
		public System.Data.Linq.Table<oxite_User> oxite_Users
		{
			get
			{
				return this.GetTable<oxite_User>();
			}
		}
		
		public System.Data.Linq.Table<oxite_Language> oxite_Languages
		{
			get
			{
				return this.GetTable<oxite_Language>();
			}
		}
	}
	
	[Table(Name="dbo.oxite_Page")]
	public partial class oxite_Page : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private System.Guid _SiteID;
		
		private System.Guid _ParentPageID;
		
		private System.Guid _PageID;
		
		private System.Guid _CreatorUserID;
		
		private string _Title;
		
		private string _Body;
		
		private byte _State;
		
		private string _Slug;
		
		private System.DateTime _CreatedDate;
		
		private System.DateTime _ModifiedDate;
		
		private System.Nullable<System.DateTime> _PublishedDate;
		
		private EntitySet<oxite_Page> _oxite_Pages;
		
		private EntityRef<oxite_Page> _oxite_Page1;
		
		private EntityRef<oxite_Site> _oxite_Site;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnSiteIDChanging(System.Guid value);
    partial void OnSiteIDChanged();
    partial void OnParentPageIDChanging(System.Guid value);
    partial void OnParentPageIDChanged();
    partial void OnPageIDChanging(System.Guid value);
    partial void OnPageIDChanged();
    partial void OnCreatorUserIDChanging(System.Guid value);
    partial void OnCreatorUserIDChanged();
    partial void OnTitleChanging(string value);
    partial void OnTitleChanged();
    partial void OnBodyChanging(string value);
    partial void OnBodyChanged();
    partial void OnStateChanging(byte value);
    partial void OnStateChanged();
    partial void OnSlugChanging(string value);
    partial void OnSlugChanged();
    partial void OnCreatedDateChanging(System.DateTime value);
    partial void OnCreatedDateChanged();
    partial void OnModifiedDateChanging(System.DateTime value);
    partial void OnModifiedDateChanged();
    partial void OnPublishedDateChanging(System.Nullable<System.DateTime> value);
    partial void OnPublishedDateChanged();
    #endregion
		
		public oxite_Page()
		{
			this._oxite_Pages = new EntitySet<oxite_Page>(new Action<oxite_Page>(this.attach_oxite_Pages), new Action<oxite_Page>(this.detach_oxite_Pages));
			this._oxite_Page1 = default(EntityRef<oxite_Page>);
			this._oxite_Site = default(EntityRef<oxite_Site>);
			OnCreated();
		}
		
		[Column(Storage="_SiteID", DbType="UniqueIdentifier NOT NULL")]
		public System.Guid SiteID
		{
			get
			{
				return this._SiteID;
			}
			set
			{
				if ((this._SiteID != value))
				{
					if (this._oxite_Site.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnSiteIDChanging(value);
					this.SendPropertyChanging();
					this._SiteID = value;
					this.SendPropertyChanged("SiteID");
					this.OnSiteIDChanged();
				}
			}
		}
		
		[Column(Storage="_ParentPageID", DbType="UniqueIdentifier NOT NULL")]
		public System.Guid ParentPageID
		{
			get
			{
				return this._ParentPageID;
			}
			set
			{
				if ((this._ParentPageID != value))
				{
					if (this._oxite_Page1.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnParentPageIDChanging(value);
					this.SendPropertyChanging();
					this._ParentPageID = value;
					this.SendPropertyChanged("ParentPageID");
					this.OnParentPageIDChanged();
				}
			}
		}
		
		[Column(Storage="_PageID", DbType="UniqueIdentifier NOT NULL", IsPrimaryKey=true)]
		public System.Guid PageID
		{
			get
			{
				return this._PageID;
			}
			set
			{
				if ((this._PageID != value))
				{
					this.OnPageIDChanging(value);
					this.SendPropertyChanging();
					this._PageID = value;
					this.SendPropertyChanged("PageID");
					this.OnPageIDChanged();
				}
			}
		}
		
		[Column(Storage="_CreatorUserID", DbType="UniqueIdentifier NOT NULL")]
		public System.Guid CreatorUserID
		{
			get
			{
				return this._CreatorUserID;
			}
			set
			{
				if ((this._CreatorUserID != value))
				{
					this.OnCreatorUserIDChanging(value);
					this.SendPropertyChanging();
					this._CreatorUserID = value;
					this.SendPropertyChanged("CreatorUserID");
					this.OnCreatorUserIDChanged();
				}
			}
		}
		
		[Column(Storage="_Title", DbType="NVarChar(256) NOT NULL", CanBeNull=false)]
		public string Title
		{
			get
			{
				return this._Title;
			}
			set
			{
				if ((this._Title != value))
				{
					this.OnTitleChanging(value);
					this.SendPropertyChanging();
					this._Title = value;
					this.SendPropertyChanged("Title");
					this.OnTitleChanged();
				}
			}
		}
		
		[Column(Storage="_Body", DbType="NVarChar(MAX) NOT NULL", CanBeNull=false)]
		public string Body
		{
			get
			{
				return this._Body;
			}
			set
			{
				if ((this._Body != value))
				{
					this.OnBodyChanging(value);
					this.SendPropertyChanging();
					this._Body = value;
					this.SendPropertyChanged("Body");
					this.OnBodyChanged();
				}
			}
		}
		
		[Column(Storage="_State", DbType="TinyInt NOT NULL")]
		public byte State
		{
			get
			{
				return this._State;
			}
			set
			{
				if ((this._State != value))
				{
					this.OnStateChanging(value);
					this.SendPropertyChanging();
					this._State = value;
					this.SendPropertyChanged("State");
					this.OnStateChanged();
				}
			}
		}
		
		[Column(Storage="_Slug", DbType="NVarChar(256) NOT NULL", CanBeNull=false)]
		public string Slug
		{
			get
			{
				return this._Slug;
			}
			set
			{
				if ((this._Slug != value))
				{
					this.OnSlugChanging(value);
					this.SendPropertyChanging();
					this._Slug = value;
					this.SendPropertyChanged("Slug");
					this.OnSlugChanged();
				}
			}
		}
		
		[Column(Storage="_CreatedDate", DbType="DateTime NOT NULL")]
		public System.DateTime CreatedDate
		{
			get
			{
				return this._CreatedDate;
			}
			set
			{
				if ((this._CreatedDate != value))
				{
					this.OnCreatedDateChanging(value);
					this.SendPropertyChanging();
					this._CreatedDate = value;
					this.SendPropertyChanged("CreatedDate");
					this.OnCreatedDateChanged();
				}
			}
		}
		
		[Column(Storage="_ModifiedDate", DbType="DateTime NOT NULL")]
		public System.DateTime ModifiedDate
		{
			get
			{
				return this._ModifiedDate;
			}
			set
			{
				if ((this._ModifiedDate != value))
				{
					this.OnModifiedDateChanging(value);
					this.SendPropertyChanging();
					this._ModifiedDate = value;
					this.SendPropertyChanged("ModifiedDate");
					this.OnModifiedDateChanged();
				}
			}
		}
		
		[Column(Storage="_PublishedDate", DbType="DateTime")]
		public System.Nullable<System.DateTime> PublishedDate
		{
			get
			{
				return this._PublishedDate;
			}
			set
			{
				if ((this._PublishedDate != value))
				{
					this.OnPublishedDateChanging(value);
					this.SendPropertyChanging();
					this._PublishedDate = value;
					this.SendPropertyChanged("PublishedDate");
					this.OnPublishedDateChanged();
				}
			}
		}
		
		[Association(Name="oxite_Page_oxite_Page", Storage="_oxite_Pages", ThisKey="PageID", OtherKey="ParentPageID")]
		public EntitySet<oxite_Page> oxite_Pages
		{
			get
			{
				return this._oxite_Pages;
			}
			set
			{
				this._oxite_Pages.Assign(value);
			}
		}
		
		[Association(Name="oxite_Page_oxite_Page", Storage="_oxite_Page1", ThisKey="ParentPageID", OtherKey="PageID", IsForeignKey=true)]
		public oxite_Page oxite_Page1
		{
			get
			{
				return this._oxite_Page1.Entity;
			}
			set
			{
				oxite_Page previousValue = this._oxite_Page1.Entity;
				if (((previousValue != value) 
							|| (this._oxite_Page1.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._oxite_Page1.Entity = null;
						previousValue.oxite_Pages.Remove(this);
					}
					this._oxite_Page1.Entity = value;
					if ((value != null))
					{
						value.oxite_Pages.Add(this);
						this._ParentPageID = value.PageID;
					}
					else
					{
						this._ParentPageID = default(System.Guid);
					}
					this.SendPropertyChanged("oxite_Page1");
				}
			}
		}
		
		[Association(Name="oxite_Site_oxite_Page", Storage="_oxite_Site", ThisKey="SiteID", OtherKey="SiteID", IsForeignKey=true)]
		public oxite_Site oxite_Site
		{
			get
			{
				return this._oxite_Site.Entity;
			}
			set
			{
				oxite_Site previousValue = this._oxite_Site.Entity;
				if (((previousValue != value) 
							|| (this._oxite_Site.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._oxite_Site.Entity = null;
						previousValue.oxite_Pages.Remove(this);
					}
					this._oxite_Site.Entity = value;
					if ((value != null))
					{
						value.oxite_Pages.Add(this);
						this._SiteID = value.SiteID;
					}
					else
					{
						this._SiteID = default(System.Guid);
					}
					this.SendPropertyChanged("oxite_Site");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_oxite_Pages(oxite_Page entity)
		{
			this.SendPropertyChanging();
			entity.oxite_Page1 = this;
		}
		
		private void detach_oxite_Pages(oxite_Page entity)
		{
			this.SendPropertyChanging();
			entity.oxite_Page1 = null;
		}
	}
	
	[Table(Name="dbo.oxite_Site")]
	public partial class oxite_Site : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private System.Guid _SiteID;
		
		private string _SiteHost;
		
		private string _SiteName;
		
		private string _SiteDisplayName;
		
		private string _SiteDescription;
		
		private string _LanguageDefault;
		
		private double _TimeZoneOffset;
		
		private string _PageTitleSeparator;
		
		private string _FavIconUrl;
		
		private string _CommentStateDefault;
		
		private bool _IncludeOpenSearch;
		
		private bool _AuthorAutoSubscribe;
		
		private short _PostEditTimeout;
		
		private string _GravatarDefault;
		
		private string _SkinsPath;
		
		private string _SkinsScriptsPath;
		
		private string _SkinsStylesPath;
		
		private string _Skin;
		
		private string _AdminSkin;
		
		private byte _ServiceRetryCountDefault;
		
		private bool _HasMultipleBlogs;
		
		private string _RouteUrlPrefix;
		
		private bool _CommentingDisabled;
		
		private string _PluginsPath;
		
		private EntitySet<oxite_Page> _oxite_Pages;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnSiteIDChanging(System.Guid value);
    partial void OnSiteIDChanged();
    partial void OnSiteHostChanging(string value);
    partial void OnSiteHostChanged();
    partial void OnSiteNameChanging(string value);
    partial void OnSiteNameChanged();
    partial void OnSiteDisplayNameChanging(string value);
    partial void OnSiteDisplayNameChanged();
    partial void OnSiteDescriptionChanging(string value);
    partial void OnSiteDescriptionChanged();
    partial void OnLanguageDefaultChanging(string value);
    partial void OnLanguageDefaultChanged();
    partial void OnTimeZoneOffsetChanging(double value);
    partial void OnTimeZoneOffsetChanged();
    partial void OnPageTitleSeparatorChanging(string value);
    partial void OnPageTitleSeparatorChanged();
    partial void OnFavIconUrlChanging(string value);
    partial void OnFavIconUrlChanged();
    partial void OnCommentStateDefaultChanging(string value);
    partial void OnCommentStateDefaultChanged();
    partial void OnIncludeOpenSearchChanging(bool value);
    partial void OnIncludeOpenSearchChanged();
    partial void OnAuthorAutoSubscribeChanging(bool value);
    partial void OnAuthorAutoSubscribeChanged();
    partial void OnPostEditTimeoutChanging(short value);
    partial void OnPostEditTimeoutChanged();
    partial void OnGravatarDefaultChanging(string value);
    partial void OnGravatarDefaultChanged();
    partial void OnSkinsPathChanging(string value);
    partial void OnSkinsPathChanged();
    partial void OnSkinsScriptsPathChanging(string value);
    partial void OnSkinsScriptsPathChanged();
    partial void OnSkinsStylesPathChanging(string value);
    partial void OnSkinsStylesPathChanged();
    partial void OnSkinChanging(string value);
    partial void OnSkinChanged();
    partial void OnAdminSkinChanging(string value);
    partial void OnAdminSkinChanged();
    partial void OnServiceRetryCountDefaultChanging(byte value);
    partial void OnServiceRetryCountDefaultChanged();
    partial void OnHasMultipleBlogsChanging(bool value);
    partial void OnHasMultipleBlogsChanged();
    partial void OnRouteUrlPrefixChanging(string value);
    partial void OnRouteUrlPrefixChanged();
    partial void OnCommentingDisabledChanging(bool value);
    partial void OnCommentingDisabledChanged();
    partial void OnPluginsPathChanging(string value);
    partial void OnPluginsPathChanged();
    #endregion
		
		public oxite_Site()
		{
			this._oxite_Pages = new EntitySet<oxite_Page>(new Action<oxite_Page>(this.attach_oxite_Pages), new Action<oxite_Page>(this.detach_oxite_Pages));
			OnCreated();
		}
		
		[Column(Storage="_SiteID", DbType="UniqueIdentifier NOT NULL", IsPrimaryKey=true)]
		public System.Guid SiteID
		{
			get
			{
				return this._SiteID;
			}
			set
			{
				if ((this._SiteID != value))
				{
					this.OnSiteIDChanging(value);
					this.SendPropertyChanging();
					this._SiteID = value;
					this.SendPropertyChanged("SiteID");
					this.OnSiteIDChanged();
				}
			}
		}
		
		[Column(Storage="_SiteHost", DbType="VarChar(100) NOT NULL", CanBeNull=false)]
		public string SiteHost
		{
			get
			{
				return this._SiteHost;
			}
			set
			{
				if ((this._SiteHost != value))
				{
					this.OnSiteHostChanging(value);
					this.SendPropertyChanging();
					this._SiteHost = value;
					this.SendPropertyChanged("SiteHost");
					this.OnSiteHostChanged();
				}
			}
		}
		
		[Column(Storage="_SiteName", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string SiteName
		{
			get
			{
				return this._SiteName;
			}
			set
			{
				if ((this._SiteName != value))
				{
					this.OnSiteNameChanging(value);
					this.SendPropertyChanging();
					this._SiteName = value;
					this.SendPropertyChanged("SiteName");
					this.OnSiteNameChanged();
				}
			}
		}
		
		[Column(Storage="_SiteDisplayName", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string SiteDisplayName
		{
			get
			{
				return this._SiteDisplayName;
			}
			set
			{
				if ((this._SiteDisplayName != value))
				{
					this.OnSiteDisplayNameChanging(value);
					this.SendPropertyChanging();
					this._SiteDisplayName = value;
					this.SendPropertyChanged("SiteDisplayName");
					this.OnSiteDisplayNameChanged();
				}
			}
		}
		
		[Column(Storage="_SiteDescription", DbType="NVarChar(250) NOT NULL", CanBeNull=false)]
		public string SiteDescription
		{
			get
			{
				return this._SiteDescription;
			}
			set
			{
				if ((this._SiteDescription != value))
				{
					this.OnSiteDescriptionChanging(value);
					this.SendPropertyChanging();
					this._SiteDescription = value;
					this.SendPropertyChanged("SiteDescription");
					this.OnSiteDescriptionChanged();
				}
			}
		}
		
		[Column(Storage="_LanguageDefault", DbType="VarChar(8) NOT NULL", CanBeNull=false)]
		public string LanguageDefault
		{
			get
			{
				return this._LanguageDefault;
			}
			set
			{
				if ((this._LanguageDefault != value))
				{
					this.OnLanguageDefaultChanging(value);
					this.SendPropertyChanging();
					this._LanguageDefault = value;
					this.SendPropertyChanged("LanguageDefault");
					this.OnLanguageDefaultChanged();
				}
			}
		}
		
		[Column(Storage="_TimeZoneOffset", DbType="Float NOT NULL")]
		public double TimeZoneOffset
		{
			get
			{
				return this._TimeZoneOffset;
			}
			set
			{
				if ((this._TimeZoneOffset != value))
				{
					this.OnTimeZoneOffsetChanging(value);
					this.SendPropertyChanging();
					this._TimeZoneOffset = value;
					this.SendPropertyChanged("TimeZoneOffset");
					this.OnTimeZoneOffsetChanged();
				}
			}
		}
		
		[Column(Storage="_PageTitleSeparator", DbType="NVarChar(10) NOT NULL", CanBeNull=false)]
		public string PageTitleSeparator
		{
			get
			{
				return this._PageTitleSeparator;
			}
			set
			{
				if ((this._PageTitleSeparator != value))
				{
					this.OnPageTitleSeparatorChanging(value);
					this.SendPropertyChanging();
					this._PageTitleSeparator = value;
					this.SendPropertyChanged("PageTitleSeparator");
					this.OnPageTitleSeparatorChanged();
				}
			}
		}
		
		[Column(Storage="_FavIconUrl", DbType="VarChar(50) NOT NULL", CanBeNull=false)]
		public string FavIconUrl
		{
			get
			{
				return this._FavIconUrl;
			}
			set
			{
				if ((this._FavIconUrl != value))
				{
					this.OnFavIconUrlChanging(value);
					this.SendPropertyChanging();
					this._FavIconUrl = value;
					this.SendPropertyChanged("FavIconUrl");
					this.OnFavIconUrlChanged();
				}
			}
		}
		
		[Column(Storage="_CommentStateDefault", DbType="VarChar(25) NOT NULL", CanBeNull=false)]
		public string CommentStateDefault
		{
			get
			{
				return this._CommentStateDefault;
			}
			set
			{
				if ((this._CommentStateDefault != value))
				{
					this.OnCommentStateDefaultChanging(value);
					this.SendPropertyChanging();
					this._CommentStateDefault = value;
					this.SendPropertyChanged("CommentStateDefault");
					this.OnCommentStateDefaultChanged();
				}
			}
		}
		
		[Column(Storage="_IncludeOpenSearch", DbType="Bit NOT NULL")]
		public bool IncludeOpenSearch
		{
			get
			{
				return this._IncludeOpenSearch;
			}
			set
			{
				if ((this._IncludeOpenSearch != value))
				{
					this.OnIncludeOpenSearchChanging(value);
					this.SendPropertyChanging();
					this._IncludeOpenSearch = value;
					this.SendPropertyChanged("IncludeOpenSearch");
					this.OnIncludeOpenSearchChanged();
				}
			}
		}
		
		[Column(Storage="_AuthorAutoSubscribe", DbType="Bit NOT NULL")]
		public bool AuthorAutoSubscribe
		{
			get
			{
				return this._AuthorAutoSubscribe;
			}
			set
			{
				if ((this._AuthorAutoSubscribe != value))
				{
					this.OnAuthorAutoSubscribeChanging(value);
					this.SendPropertyChanging();
					this._AuthorAutoSubscribe = value;
					this.SendPropertyChanged("AuthorAutoSubscribe");
					this.OnAuthorAutoSubscribeChanged();
				}
			}
		}
		
		[Column(Storage="_PostEditTimeout", DbType="SmallInt NOT NULL")]
		public short PostEditTimeout
		{
			get
			{
				return this._PostEditTimeout;
			}
			set
			{
				if ((this._PostEditTimeout != value))
				{
					this.OnPostEditTimeoutChanging(value);
					this.SendPropertyChanging();
					this._PostEditTimeout = value;
					this.SendPropertyChanged("PostEditTimeout");
					this.OnPostEditTimeoutChanged();
				}
			}
		}
		
		[Column(Storage="_GravatarDefault", DbType="VarChar(100) NOT NULL", CanBeNull=false)]
		public string GravatarDefault
		{
			get
			{
				return this._GravatarDefault;
			}
			set
			{
				if ((this._GravatarDefault != value))
				{
					this.OnGravatarDefaultChanging(value);
					this.SendPropertyChanging();
					this._GravatarDefault = value;
					this.SendPropertyChanged("GravatarDefault");
					this.OnGravatarDefaultChanged();
				}
			}
		}
		
		[Column(Storage="_SkinsPath", DbType="VarChar(50) NOT NULL", CanBeNull=false)]
		public string SkinsPath
		{
			get
			{
				return this._SkinsPath;
			}
			set
			{
				if ((this._SkinsPath != value))
				{
					this.OnSkinsPathChanging(value);
					this.SendPropertyChanging();
					this._SkinsPath = value;
					this.SendPropertyChanged("SkinsPath");
					this.OnSkinsPathChanged();
				}
			}
		}
		
		[Column(Storage="_SkinsScriptsPath", DbType="VarChar(50) NOT NULL", CanBeNull=false)]
		public string SkinsScriptsPath
		{
			get
			{
				return this._SkinsScriptsPath;
			}
			set
			{
				if ((this._SkinsScriptsPath != value))
				{
					this.OnSkinsScriptsPathChanging(value);
					this.SendPropertyChanging();
					this._SkinsScriptsPath = value;
					this.SendPropertyChanged("SkinsScriptsPath");
					this.OnSkinsScriptsPathChanged();
				}
			}
		}
		
		[Column(Storage="_SkinsStylesPath", DbType="VarChar(50) NOT NULL", CanBeNull=false)]
		public string SkinsStylesPath
		{
			get
			{
				return this._SkinsStylesPath;
			}
			set
			{
				if ((this._SkinsStylesPath != value))
				{
					this.OnSkinsStylesPathChanging(value);
					this.SendPropertyChanging();
					this._SkinsStylesPath = value;
					this.SendPropertyChanged("SkinsStylesPath");
					this.OnSkinsStylesPathChanged();
				}
			}
		}
		
		[Column(Storage="_Skin", DbType="VarChar(50) NOT NULL", CanBeNull=false)]
		public string Skin
		{
			get
			{
				return this._Skin;
			}
			set
			{
				if ((this._Skin != value))
				{
					this.OnSkinChanging(value);
					this.SendPropertyChanging();
					this._Skin = value;
					this.SendPropertyChanged("Skin");
					this.OnSkinChanged();
				}
			}
		}
		
		[Column(Storage="_AdminSkin", DbType="VarChar(50) NOT NULL", CanBeNull=false)]
		public string AdminSkin
		{
			get
			{
				return this._AdminSkin;
			}
			set
			{
				if ((this._AdminSkin != value))
				{
					this.OnAdminSkinChanging(value);
					this.SendPropertyChanging();
					this._AdminSkin = value;
					this.SendPropertyChanged("AdminSkin");
					this.OnAdminSkinChanged();
				}
			}
		}
		
		[Column(Storage="_ServiceRetryCountDefault", DbType="TinyInt NOT NULL")]
		public byte ServiceRetryCountDefault
		{
			get
			{
				return this._ServiceRetryCountDefault;
			}
			set
			{
				if ((this._ServiceRetryCountDefault != value))
				{
					this.OnServiceRetryCountDefaultChanging(value);
					this.SendPropertyChanging();
					this._ServiceRetryCountDefault = value;
					this.SendPropertyChanged("ServiceRetryCountDefault");
					this.OnServiceRetryCountDefaultChanged();
				}
			}
		}
		
		[Column(Storage="_HasMultipleBlogs", DbType="Bit NOT NULL")]
		public bool HasMultipleBlogs
		{
			get
			{
				return this._HasMultipleBlogs;
			}
			set
			{
				if ((this._HasMultipleBlogs != value))
				{
					this.OnHasMultipleBlogsChanging(value);
					this.SendPropertyChanging();
					this._HasMultipleBlogs = value;
					this.SendPropertyChanged("HasMultipleBlogs");
					this.OnHasMultipleBlogsChanged();
				}
			}
		}
		
		[Column(Storage="_RouteUrlPrefix", DbType="VarChar(20) NOT NULL", CanBeNull=false)]
		public string RouteUrlPrefix
		{
			get
			{
				return this._RouteUrlPrefix;
			}
			set
			{
				if ((this._RouteUrlPrefix != value))
				{
					this.OnRouteUrlPrefixChanging(value);
					this.SendPropertyChanging();
					this._RouteUrlPrefix = value;
					this.SendPropertyChanged("RouteUrlPrefix");
					this.OnRouteUrlPrefixChanged();
				}
			}
		}
		
		[Column(Storage="_CommentingDisabled", DbType="Bit NOT NULL")]
		public bool CommentingDisabled
		{
			get
			{
				return this._CommentingDisabled;
			}
			set
			{
				if ((this._CommentingDisabled != value))
				{
					this.OnCommentingDisabledChanging(value);
					this.SendPropertyChanging();
					this._CommentingDisabled = value;
					this.SendPropertyChanged("CommentingDisabled");
					this.OnCommentingDisabledChanged();
				}
			}
		}
		
		[Column(Storage="_PluginsPath", DbType="VarChar(50) NOT NULL", CanBeNull=false)]
		public string PluginsPath
		{
			get
			{
				return this._PluginsPath;
			}
			set
			{
				if ((this._PluginsPath != value))
				{
					this.OnPluginsPathChanging(value);
					this.SendPropertyChanging();
					this._PluginsPath = value;
					this.SendPropertyChanged("PluginsPath");
					this.OnPluginsPathChanged();
				}
			}
		}
		
		[Association(Name="oxite_Site_oxite_Page", Storage="_oxite_Pages", ThisKey="SiteID", OtherKey="SiteID")]
		public EntitySet<oxite_Page> oxite_Pages
		{
			get
			{
				return this._oxite_Pages;
			}
			set
			{
				this._oxite_Pages.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_oxite_Pages(oxite_Page entity)
		{
			this.SendPropertyChanging();
			entity.oxite_Site = this;
		}
		
		private void detach_oxite_Pages(oxite_Page entity)
		{
			this.SendPropertyChanging();
			entity.oxite_Site = null;
		}
	}
	
	[Table(Name="dbo.oxite_User")]
	public partial class oxite_User : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private System.Guid _UserID;
		
		private string _Username;
		
		private string _DisplayName;
		
		private string _Email;
		
		private string _HashedEmail;
		
		private System.Guid _DefaultLanguageID;
		
		private byte _Status;
		
		private EntityRef<oxite_Language> _oxite_Language;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnUserIDChanging(System.Guid value);
    partial void OnUserIDChanged();
    partial void OnUsernameChanging(string value);
    partial void OnUsernameChanged();
    partial void OnDisplayNameChanging(string value);
    partial void OnDisplayNameChanged();
    partial void OnEmailChanging(string value);
    partial void OnEmailChanged();
    partial void OnHashedEmailChanging(string value);
    partial void OnHashedEmailChanged();
    partial void OnDefaultLanguageIDChanging(System.Guid value);
    partial void OnDefaultLanguageIDChanged();
    partial void OnStatusChanging(byte value);
    partial void OnStatusChanged();
    #endregion
		
		public oxite_User()
		{
			this._oxite_Language = default(EntityRef<oxite_Language>);
			OnCreated();
		}
		
		[Column(Storage="_UserID", DbType="UniqueIdentifier NOT NULL", IsPrimaryKey=true)]
		public System.Guid UserID
		{
			get
			{
				return this._UserID;
			}
			set
			{
				if ((this._UserID != value))
				{
					this.OnUserIDChanging(value);
					this.SendPropertyChanging();
					this._UserID = value;
					this.SendPropertyChanged("UserID");
					this.OnUserIDChanged();
				}
			}
		}
		
		[Column(Storage="_Username", DbType="NVarChar(256) NOT NULL", CanBeNull=false)]
		public string Username
		{
			get
			{
				return this._Username;
			}
			set
			{
				if ((this._Username != value))
				{
					this.OnUsernameChanging(value);
					this.SendPropertyChanging();
					this._Username = value;
					this.SendPropertyChanged("Username");
					this.OnUsernameChanged();
				}
			}
		}
		
		[Column(Storage="_DisplayName", DbType="NVarChar(256) NOT NULL", CanBeNull=false)]
		public string DisplayName
		{
			get
			{
				return this._DisplayName;
			}
			set
			{
				if ((this._DisplayName != value))
				{
					this.OnDisplayNameChanging(value);
					this.SendPropertyChanging();
					this._DisplayName = value;
					this.SendPropertyChanged("DisplayName");
					this.OnDisplayNameChanged();
				}
			}
		}
		
		[Column(Storage="_Email", DbType="NVarChar(256) NOT NULL", CanBeNull=false)]
		public string Email
		{
			get
			{
				return this._Email;
			}
			set
			{
				if ((this._Email != value))
				{
					this.OnEmailChanging(value);
					this.SendPropertyChanging();
					this._Email = value;
					this.SendPropertyChanged("Email");
					this.OnEmailChanged();
				}
			}
		}
		
		[Column(Storage="_HashedEmail", DbType="NVarChar(100) NOT NULL", CanBeNull=false)]
		public string HashedEmail
		{
			get
			{
				return this._HashedEmail;
			}
			set
			{
				if ((this._HashedEmail != value))
				{
					this.OnHashedEmailChanging(value);
					this.SendPropertyChanging();
					this._HashedEmail = value;
					this.SendPropertyChanged("HashedEmail");
					this.OnHashedEmailChanged();
				}
			}
		}
		
		[Column(Storage="_DefaultLanguageID", DbType="UniqueIdentifier NOT NULL")]
		public System.Guid DefaultLanguageID
		{
			get
			{
				return this._DefaultLanguageID;
			}
			set
			{
				if ((this._DefaultLanguageID != value))
				{
					if (this._oxite_Language.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnDefaultLanguageIDChanging(value);
					this.SendPropertyChanging();
					this._DefaultLanguageID = value;
					this.SendPropertyChanged("DefaultLanguageID");
					this.OnDefaultLanguageIDChanged();
				}
			}
		}
		
		[Column(Storage="_Status", DbType="TinyInt NOT NULL")]
		public byte Status
		{
			get
			{
				return this._Status;
			}
			set
			{
				if ((this._Status != value))
				{
					this.OnStatusChanging(value);
					this.SendPropertyChanging();
					this._Status = value;
					this.SendPropertyChanged("Status");
					this.OnStatusChanged();
				}
			}
		}
		
		[Association(Name="oxite_Language_oxite_User", Storage="_oxite_Language", ThisKey="DefaultLanguageID", OtherKey="LanguageID", IsForeignKey=true)]
		public oxite_Language oxite_Language
		{
			get
			{
				return this._oxite_Language.Entity;
			}
			set
			{
				oxite_Language previousValue = this._oxite_Language.Entity;
				if (((previousValue != value) 
							|| (this._oxite_Language.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._oxite_Language.Entity = null;
						previousValue.oxite_Users.Remove(this);
					}
					this._oxite_Language.Entity = value;
					if ((value != null))
					{
						value.oxite_Users.Add(this);
						this._DefaultLanguageID = value.LanguageID;
					}
					else
					{
						this._DefaultLanguageID = default(System.Guid);
					}
					this.SendPropertyChanged("oxite_Language");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[Table(Name="dbo.oxite_Language")]
	public partial class oxite_Language : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private System.Guid _LanguageID;
		
		private string _LanguageName;
		
		private string _LanguageDisplayName;
		
		private EntitySet<oxite_User> _oxite_Users;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnLanguageIDChanging(System.Guid value);
    partial void OnLanguageIDChanged();
    partial void OnLanguageNameChanging(string value);
    partial void OnLanguageNameChanged();
    partial void OnLanguageDisplayNameChanging(string value);
    partial void OnLanguageDisplayNameChanged();
    #endregion
		
		public oxite_Language()
		{
			this._oxite_Users = new EntitySet<oxite_User>(new Action<oxite_User>(this.attach_oxite_Users), new Action<oxite_User>(this.detach_oxite_Users));
			OnCreated();
		}
		
		[Column(Storage="_LanguageID", DbType="UniqueIdentifier NOT NULL", IsPrimaryKey=true)]
		public System.Guid LanguageID
		{
			get
			{
				return this._LanguageID;
			}
			set
			{
				if ((this._LanguageID != value))
				{
					this.OnLanguageIDChanging(value);
					this.SendPropertyChanging();
					this._LanguageID = value;
					this.SendPropertyChanged("LanguageID");
					this.OnLanguageIDChanged();
				}
			}
		}
		
		[Column(Storage="_LanguageName", DbType="VarChar(8) NOT NULL", CanBeNull=false)]
		public string LanguageName
		{
			get
			{
				return this._LanguageName;
			}
			set
			{
				if ((this._LanguageName != value))
				{
					this.OnLanguageNameChanging(value);
					this.SendPropertyChanging();
					this._LanguageName = value;
					this.SendPropertyChanged("LanguageName");
					this.OnLanguageNameChanged();
				}
			}
		}
		
		[Column(Storage="_LanguageDisplayName", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string LanguageDisplayName
		{
			get
			{
				return this._LanguageDisplayName;
			}
			set
			{
				if ((this._LanguageDisplayName != value))
				{
					this.OnLanguageDisplayNameChanging(value);
					this.SendPropertyChanging();
					this._LanguageDisplayName = value;
					this.SendPropertyChanged("LanguageDisplayName");
					this.OnLanguageDisplayNameChanged();
				}
			}
		}
		
		[Association(Name="oxite_Language_oxite_User", Storage="_oxite_Users", ThisKey="LanguageID", OtherKey="DefaultLanguageID")]
		public EntitySet<oxite_User> oxite_Users
		{
			get
			{
				return this._oxite_Users;
			}
			set
			{
				this._oxite_Users.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_oxite_Users(oxite_User entity)
		{
			this.SendPropertyChanging();
			entity.oxite_Language = this;
		}
		
		private void detach_oxite_Users(oxite_User entity)
		{
			this.SendPropertyChanging();
			entity.oxite_Language = null;
		}
	}
}
#pragma warning restore 1591

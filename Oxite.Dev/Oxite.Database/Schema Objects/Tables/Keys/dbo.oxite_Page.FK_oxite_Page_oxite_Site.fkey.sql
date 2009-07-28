ALTER TABLE [dbo].[oxite_Page] ADD
CONSTRAINT [FK_oxite_Page_oxite_Site] FOREIGN KEY ([SiteID]) REFERENCES [dbo].[oxite_Site] ([SiteID])



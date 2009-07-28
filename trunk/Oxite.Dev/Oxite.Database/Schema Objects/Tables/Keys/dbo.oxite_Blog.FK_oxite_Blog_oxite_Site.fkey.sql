ALTER TABLE [dbo].[oxite_Blog] ADD
CONSTRAINT [FK_oxite_Blog_oxite_Site] FOREIGN KEY ([SiteID]) REFERENCES [dbo].[oxite_Site] ([SiteID])



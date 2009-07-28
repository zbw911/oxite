ALTER TABLE [dbo].[oxite_Page] ADD
CONSTRAINT [FK_oxite_Page_oxite_Page] FOREIGN KEY ([ParentPageID]) REFERENCES [dbo].[oxite_Page] ([PageID])



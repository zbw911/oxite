ALTER TABLE [dbo].[oxite_PageRoleUserRelationship] ADD
CONSTRAINT [FK_oxite_PageRoleUserRelationship_oxite_User] FOREIGN KEY ([UserID]) REFERENCES [dbo].[oxite_User] ([UserID])



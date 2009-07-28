ALTER TABLE [dbo].[oxite_PostRoleUserRelationship] ADD
CONSTRAINT [FK_oxite_PostRoleUserRelationship_oxite_User] FOREIGN KEY ([UserID]) REFERENCES [dbo].[oxite_User] ([UserID])



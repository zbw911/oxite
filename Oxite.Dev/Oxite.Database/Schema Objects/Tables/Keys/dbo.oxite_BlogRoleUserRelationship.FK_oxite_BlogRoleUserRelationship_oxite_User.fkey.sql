ALTER TABLE [dbo].[oxite_BlogRoleUserRelationship] ADD
CONSTRAINT [FK_oxite_BlogRoleUserRelationship_oxite_User] FOREIGN KEY ([UserID]) REFERENCES [dbo].[oxite_User] ([UserID])



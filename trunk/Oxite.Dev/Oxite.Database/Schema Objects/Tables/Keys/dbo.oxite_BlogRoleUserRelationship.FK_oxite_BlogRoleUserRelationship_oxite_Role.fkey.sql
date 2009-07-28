ALTER TABLE [dbo].[oxite_BlogRoleUserRelationship] ADD
CONSTRAINT [FK_oxite_BlogRoleUserRelationship_oxite_Role] FOREIGN KEY ([RoleID]) REFERENCES [dbo].[oxite_Role] ([RoleID])



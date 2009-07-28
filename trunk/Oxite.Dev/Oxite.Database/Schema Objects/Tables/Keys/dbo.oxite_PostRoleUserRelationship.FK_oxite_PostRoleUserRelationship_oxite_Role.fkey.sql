ALTER TABLE [dbo].[oxite_PostRoleUserRelationship] ADD
CONSTRAINT [FK_oxite_PostRoleUserRelationship_oxite_Role] FOREIGN KEY ([RoleID]) REFERENCES [dbo].[oxite_Role] ([RoleID])



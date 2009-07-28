ALTER TABLE [dbo].[oxite_BlogRoleUserRelationship] ADD
CONSTRAINT [FK_oxite_BlogRoleUserRelationship_oxite_Blog] FOREIGN KEY ([BlogID]) REFERENCES [dbo].[oxite_Blog] ([BlogID])



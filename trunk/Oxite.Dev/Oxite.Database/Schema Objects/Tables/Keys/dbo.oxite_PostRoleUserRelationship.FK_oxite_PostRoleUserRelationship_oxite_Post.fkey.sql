ALTER TABLE [dbo].[oxite_PostRoleUserRelationship] ADD
CONSTRAINT [FK_oxite_PostRoleUserRelationship_oxite_Post] FOREIGN KEY ([PostID]) REFERENCES [dbo].[oxite_Post] ([PostID])



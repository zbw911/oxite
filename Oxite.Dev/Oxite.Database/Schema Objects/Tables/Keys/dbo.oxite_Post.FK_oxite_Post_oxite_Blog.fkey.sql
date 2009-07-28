ALTER TABLE [dbo].[oxite_Post] ADD
CONSTRAINT [FK_oxite_Post_oxite_Blog] FOREIGN KEY ([BlogID]) REFERENCES [dbo].[oxite_Blog] ([BlogID])



ALTER TABLE [dbo].[oxite_Conferences_SpeakerItemSpeakerRelationship] ADD
CONSTRAINT [FK_oxite_Conferences_SpeakerItemSpeakerRelationship_oxite_Conferences_Speaker] FOREIGN KEY ([SpeakerID]) REFERENCES [dbo].[oxite_Conferences_Speaker] ([SpeakerID])



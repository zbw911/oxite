ALTER TABLE [dbo].[oxite_Conferences_SpeakerItemSpeakerRelationship] ADD
CONSTRAINT [FK_oxite_Conferences_SpeakerItemSpeakerRelationship_oxite_Conferences_ScheduleItem] FOREIGN KEY ([ScheduleItemID]) REFERENCES [dbo].[oxite_Conferences_ScheduleItem] ([ScheduleItemID])



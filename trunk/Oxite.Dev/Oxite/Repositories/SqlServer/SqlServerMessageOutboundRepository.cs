﻿//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Oxite.Models;
using Oxite.Repositories;

namespace Oxite.Repositories.SqlServer
{
    public class SqlServerMessageOutboundRepository : IMessageOutboundRepository
    {
        private OxiteDataContext context;

        public SqlServerMessageOutboundRepository(OxiteDataContext context)
        {
            this.context = context;
        }

        #region IMessageOutboundRepository Members

        public void Save(IEnumerable<MessageOutbound> messages)
        {
            if (messages != null && messages.Count() > 0)
            {
                context.oxite_MessageOutbounds.InsertAllOnSubmit(
                    messages.Select(
                        m =>
                            new oxite_MessageOutbound
                            {
                                MessageOutboundID = Guid.NewGuid(),
                                MessageTo = m.To,
                                MessageSubject = m.Subject,
                                MessageBody = m.Body,
                                RemainingRetryCount = (byte)m.RemainingRetryCount,
                                LastAttemptDate = null,
                                SentDate = null,
                                IsSending = false
                            }
                        )
                    );

                context.SubmitChanges();
            }
        }

        public IEnumerable<MessageOutbound> GetNext(bool executeOnAll, TimeSpan interval)
        {
            IEnumerable<MessageOutbound> messages = Enumerable.Empty<MessageOutbound>();

            using (TransactionScope transaction = new TransactionScope())
            {
                IQueryable<oxite_MessageOutbound> query =
                    from mo in context.oxite_MessageOutbounds
                    where !mo.IsSending && mo.RemainingRetryCount > 0 && (!mo.LastAttemptDate.HasValue || mo.LastAttemptDate.Value.Add(interval) <= DateTime.Now.ToUniversalTime())
                    select mo;

                if (!executeOnAll)
                {
                    query = query.Take(1);
                }

                messages = query.Select(
                    m => new MessageOutbound()
                    {
                        ID = m.MessageOutboundID,
                        To = m.MessageTo,
                        Subject = m.MessageSubject,
                        Body = m.MessageBody,
                        RemainingRetryCount = m.RemainingRetryCount
                    }
                    ).ToList();

                if (query.Count() > 0)
                    query.ToList().ForEach(mo => mo.IsSending = true);

                context.SubmitChanges();

                transaction.Complete();
            }

            return messages;
        }

        public void Save(MessageOutbound message)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                oxite_MessageOutbound m = (
                    from mo in context.oxite_MessageOutbounds
                    where mo.MessageOutboundID == message.ID
                    select mo
                    ).First();

                m.SentDate = message.Sent;
                m.RemainingRetryCount = (byte)message.RemainingRetryCount;
                m.IsSending = false;
                m.LastAttemptDate = DateTime.Now.ToUniversalTime();

                context.SubmitChanges();

                transaction.Complete();
            }
        }

        #endregion
    }
}

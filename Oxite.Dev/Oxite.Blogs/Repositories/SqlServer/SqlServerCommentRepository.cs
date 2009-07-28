//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Linq;
using Oxite.Models;

namespace Oxite.Modules.Blogs.Repositories.SqlServer
{
    public class SqlServerCommentRepository : ICommentRepository
    {
        private readonly OxiteBlogsDataContext context;

        public SqlServerCommentRepository(OxiteBlogsDataContext context)
        {
            this.context = context;
        }

        #region ICommentRepository Members

        public Comment GetComment(Guid commentID)
        {
            return projectComments(context.oxite_Comments.Where(c => c.CommentID == commentID)).FirstOrDefault();
        }

        public Comment GetComment(Guid siteID, string blogName, string postSlug, string commentSlug)
        {
            var query =
                from c in context.oxite_Comments
                join p in context.oxite_Posts on c.PostID equals p.PostID
                join b in context.oxite_Blogs on p.BlogID equals b.BlogID
                where b.SiteID == siteID && string.Compare(b.BlogName, blogName, true) == 0 && p.Slug == postSlug && c.Slug == commentSlug
                select c;

            return projectComments(query).FirstOrDefault();
        }

        public IQueryable<Comment> GetComments(Guid siteID, bool includePending, bool sortDescending)
        {
            var query =
                from b in context.oxite_Blogs
                join p in context.oxite_Posts on b.BlogID equals p.BlogID
                join c in context.oxite_Comments on p.PostID equals c.PostID
                where b.SiteID == siteID && p.PublishedDate != null && p.PublishedDate <= DateTime.UtcNow && p.State == (byte)EntityState.Normal
                select c;

            query = includePending
                ? query.Where(c => c.State == (byte)EntityState.Normal || c.State == (byte)EntityState.PendingApproval)
                : query.Where(c => c.State == (byte)EntityState.Normal);

            query = sortDescending
                ? query.OrderByDescending(c => c.CreatedDate)
                : query.OrderBy(c => c.CreatedDate);

            return projectComments(query);
        }

        public IQueryable<Comment> GetCommentsByBlog(Guid blogID)
        {
            IQueryable<oxite_Comment> commentsByBlog =
                from c in context.oxite_Comments
                join p in context.oxite_Posts on c.PostID equals p.BlogID
                where p.BlogID == blogID && c.State == (byte)EntityState.Normal && p.PublishedDate != null && p.PublishedDate <= DateTime.UtcNow && p.State == (byte)EntityState.Normal
                orderby c.CreatedDate descending
                select c;

            return projectComments(commentsByBlog);
        }

        public IQueryable<Comment> GetCommentsByPost(Guid postID, bool includeUnapproved)
        {
            IQueryable<oxite_Comment> commentsByPost =
                from c in context.oxite_Comments
                where c.PostID == postID
                orderby c.CreatedDate ascending
                select c;

            if (includeUnapproved)
                commentsByPost = commentsByPost.Where(c => c.State == (byte)EntityState.Normal || c.State == (byte)EntityState.PendingApproval);
            else
                commentsByPost = commentsByPost.Where(c => c.State == (byte)EntityState.Normal);

            return projectComments(commentsByPost);
        }

        public IQueryable<Comment> GetCommentsByTag(Guid siteID, Guid tagID)
        {
            IQueryable<oxite_Comment> commentsByTag =
                from b in context.oxite_Blogs
                join p in context.oxite_Posts on b.BlogID equals p.BlogID
                join c in context.oxite_Comments on p.PostID equals c.PostID
                join ptr in context.oxite_PostTagRelationships on p.PostID equals ptr.PostID
                where b.SiteID == siteID && ptr.TagID == tagID && c.State == (byte)EntityState.Normal && p.PublishedDate != null && p.PublishedDate <= DateTime.UtcNow && p.State == (byte)EntityState.Normal
                orderby c.CreatedDate descending
                select c;

            return projectComments(commentsByTag);
        }

        public void ChangeState(Guid commentID, EntityState state)
        {
            oxite_Comment comment = context.oxite_Comments.FirstOrDefault(c => c.CommentID == commentID);

            if (comment != null)
                comment.State = (byte)state;

            context.SubmitChanges();
        }

        public Comment Save(Comment comment, Guid siteID, string blogName, string postSlug)
        {
            oxite_Comment commentToSave = null;

            if (comment.ID != Guid.Empty)
                commentToSave = context.oxite_Comments.FirstOrDefault(c => c.CommentID == comment.ID);

            if (commentToSave == null)
            {
                commentToSave = new oxite_Comment();

                commentToSave.CommentID = comment.ID != Guid.Empty ? comment.ID : Guid.NewGuid();
                commentToSave.CreatedDate = commentToSave.ModifiedDate = DateTime.UtcNow;

                context.oxite_Comments.InsertOnSubmit(commentToSave);
            }
            else
                commentToSave.ModifiedDate = DateTime.UtcNow;

            oxite_Post post = (from p in context.oxite_Posts join b in context.oxite_Blogs on p.BlogID equals b.BlogID where b.SiteID == siteID && string.Compare(b.BlogName, blogName, true) == 0 && p.Slug == postSlug select p).FirstOrDefault();
            if (post == null) throw new InvalidOperationException(string.Format("Post in blog {0} at slug {1} could not be found to add the comment to", blogName, postSlug));
            commentToSave.PostID = post.PostID;

            commentToSave.ParentCommentID = comment.Parent != null && comment.Parent.ID != Guid.Empty ? comment.Parent.ID : commentToSave.CommentID;
            commentToSave.Body = comment.Body;
            commentToSave.CreatorIP = comment.CreatorIP;
            commentToSave.State = (byte)comment.State;
            commentToSave.UserAgent = comment.CreatorUserAgent;
            commentToSave.oxite_Language = context.oxite_Languages.Where(l => l.LanguageName == comment.Language.Name).FirstOrDefault();
            commentToSave.Slug = comment.Slug;

            if (comment.CreatorUserID != Guid.Empty)
                commentToSave.CreatorUserID = comment.CreatorUserID;
            else
            {
                oxite_User anonymousUser = context.oxite_Users.FirstOrDefault(u => u.Username == "Anonymous");
                if (anonymousUser == null) throw new InvalidOperationException("Could not find anonymous user");
                commentToSave.CreatorUserID = anonymousUser.UserID;

                commentToSave.CreatorName = comment.CreatorName;
                commentToSave.CreatorEmail = comment.CreatorEmail;
                commentToSave.CreatorHashedEmail = comment.CreatorEmailHash;
                commentToSave.CreatorUrl = comment.CreatorUrl;
            }

            context.SubmitChanges();

            return GetComment(commentToSave.CommentID);
        }

        #endregion

        #region Private Methods

        private IQueryable<Comment> projectComments(IQueryable<oxite_Comment> comments)
        {
            return from c in comments
                   join u in context.oxite_Users on c.CreatorUserID equals u.UserID
                   join p in context.oxite_Posts on c.PostID equals p.PostID
                   join b in context.oxite_Blogs on p.BlogID equals b.BlogID
                   where p.State != (byte)EntityState.Removed
                   select getComment(c, p, b, u);
        }

        private Comment getComment(oxite_Comment comment, oxite_Post post, oxite_Blog blog, oxite_User user)
        {
            PostSmall postSmall = new PostSmall(post.PostID, blog.BlogName, post.Slug, post.Title);
            CommentSmall parent = comment.ParentCommentID != comment.CommentID ? getParentComment(comment.ParentCommentID) : null;
            Language language = new Language(comment.oxite_Language.LanguageID)
            {
                DisplayName = comment.oxite_Language.LanguageDisplayName,
                Name = comment.oxite_Language.LanguageName
            };

            if (user.Username != "Anonymous")
                return new Comment(comment.Body, comment.CreatedDate, getUserAuthenticated(comment, user), comment.CreatorIP, comment.UserAgent, comment.CommentID, language, comment.ModifiedDate, parent, postSmall, comment.Slug, (EntityState)comment.State);
            else
                return new Comment(comment.Body, comment.CreatedDate, getUserAnonymous(comment, user), comment.CreatorIP, comment.UserAgent, comment.CommentID, language, comment.ModifiedDate, parent, postSmall, comment.Slug, (EntityState)comment.State);
        }

        private CommentSmall getParentComment(Guid commentID)
        {
            return (
                from c in context.oxite_Comments
                join u in context.oxite_Users on c.CreatorUserID equals u.UserID
                where c.State != (byte)EntityState.Removed && c.CommentID == commentID
                select projectCommentSmall(c, u)
                ).FirstOrDefault();
        }

        private static UserAuthenticated getUserAuthenticated(oxite_Comment comment, oxite_User user)
        {
            return new UserAuthenticated(user.UserID, user.Username, user.DisplayName, user.Email, user.HashedEmail, (EntityState)user.Status);
        }

        private static UserAnonymous getUserAnonymous(oxite_Comment comment, oxite_User user)
        {
            return new UserAnonymous(comment.CreatorName, comment.CreatorEmail, comment.CreatorHashedEmail, comment.CreatorUrl);
        }

        private static CommentSmall projectCommentSmall(oxite_Comment comment, oxite_User user)
        {
            if (user.Username != "Anonymous")
                return new CommentSmall(comment.CommentID, comment.CreatedDate, getUserAuthenticated(comment, user), comment.Slug);
            else
                return new CommentSmall(comment.CommentID, comment.CreatedDate, getUserAnonymous(comment, user), comment.Slug);
        }

        #endregion
    }
}

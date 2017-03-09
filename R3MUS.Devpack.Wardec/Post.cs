namespace R3MUS.Devpack.Wardec
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Post")]
    public partial class Post
    {
        public Guid Id { get; set; }

        public Guid MembershipUser_Id { get; set; }

        [Required]
        public string PostContent { get; set; }

        public DateTime DateCreated { get; set; }

        public int VoteCount { get; set; }

        public Guid Topic_Id { get; set; }

        public DateTime? DateEdited { get; set; }

        public bool IsSolution { get; set; }

        public bool? IsTopicStarter { get; set; }

        public bool? FlaggedAsSpam { get; set; }

        [StringLength(50)]
        public string IpAddress { get; set; }

        public bool? Pending { get; set; }

        public virtual MembershipUser MembershipUser { get; set; }

        public virtual Topic Topic { get; set; }
    }
}

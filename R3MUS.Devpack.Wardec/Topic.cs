namespace R3MUS.Devpack.Wardec
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Topic")]
    public partial class Topic
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Topic()
        {
            Posts = new HashSet<Post>();
        }

        public Guid Id { get; set; }

        [Required]
        [StringLength(450)]
        public string Name { get; set; }

        public DateTime CreateDate { get; set; }

        public Guid MembershipUser_Id { get; set; }

        public bool Solved { get; set; }

        public Guid Category_Id { get; set; }

        public Guid? Post_Id { get; set; }

        [Required]
        [StringLength(450)]
        public string Slug { get; set; }

        public int? Views { get; set; }

        public bool IsSticky { get; set; }

        public bool IsLocked { get; set; }

        public Guid? Poll_Id { get; set; }

        public bool? Pending { get; set; }

        public virtual Category Category { get; set; }

        public virtual MembershipUser MembershipUser { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Post> Posts { get; set; }
    }
}

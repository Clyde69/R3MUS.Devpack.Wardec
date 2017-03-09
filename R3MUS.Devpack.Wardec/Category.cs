namespace R3MUS.Devpack.Wardec
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Category")]
    public partial class Category
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Category()
        {
            Topics = new HashSet<Topic>();
        }

        public Guid Id { get; set; }

        [Required]
        [StringLength(450)]
        public string Name { get; set; }

        public string Description { get; set; }

        public Guid? Category_Id { get; set; }

        public DateTime DateCreated { get; set; }

        [Required]
        [StringLength(450)]
        public string Slug { get; set; }

        public int SortOrder { get; set; }

        public bool IsLocked { get; set; }

        public bool ModerateTopics { get; set; }

        public bool ModeratePosts { get; set; }

        [StringLength(80)]
        public string PageTitle { get; set; }

        [StringLength(200)]
        public string MetaDescription { get; set; }

        [StringLength(2500)]
        public string Path { get; set; }

        [StringLength(50)]
        public string Colour { get; set; }

        [StringLength(200)]
        public string Image { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Topic> Topics { get; set; }
    }
}

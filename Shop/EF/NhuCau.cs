namespace Shop.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NhuCau")]
    public partial class NhuCau
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NhuCau()
        {
            Dienthoais = new HashSet<Dienthoai>();
        }

        [Key]
        public int manhucau { get; set; }

        [Required]
        [StringLength(50)]
        public string tennhucau { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Dienthoai> Dienthoais { get; set; }
    }
}

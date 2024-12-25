namespace Shop.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MetaDienthoai")]
    public partial class MetaDienthoai
    {
        [Key]
        public int mameta { get; set; }

        [StringLength(255)]
        public string keymeta { get; set; }

        [StringLength(255)]
        public string valuemeta { get; set; }

        public int? madienthoai { get; set; }

        public virtual Dienthoai Dienthoai { get; set; }
    }
}

namespace Shop.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DanhGia")]
    public partial class DanhGia
    {
        [Key]
        public int madanhgia { get; set; }

        [StringLength(50)]
        public string ten { get; set; }

        [Column(TypeName = "ntext")]
        public string noidung { get; set; }

        public int? vote { get; set; }

        public DateTime? ngaydanhgia { get; set; }

        public int? madienthoai { get; set; }

        public bool? trangthai { get; set; }

        public virtual Dienthoai Dienthoai { get; set; }
    }
}

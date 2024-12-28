namespace Shop.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Dienthoai")]
    public partial class Dienthoai
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Dienthoai()
        {
            ChiTietDonHangs = new HashSet<ChiTietDonHang>();
            DanhGias = new HashSet<DanhGia>();
            MetaDienthoais = new HashSet<MetaDienthoai>();
        }

        [Key]
        public int madienthoai { get; set; }

        [Required]
        [StringLength(100)]
        public string tendienthoai { get; set; }

        public decimal? giaban { get; set; }

        [Column(TypeName = "ntext")]
        public string mota { get; set; }

        [StringLength(70)]
        public string hinh { get; set; }

        public int? mahang { get; set; }

        public int? manhucau { get; set; }

        [StringLength(100)]
        public string cpu { get; set; }

        [StringLength(100)]
        public string gpu { get; set; }

        [StringLength(100)]
        public string rom { get; set; }

        [StringLength(100)]
        public string ram { get; set; }

        [StringLength(100)]
        public string hedieuhanh { get; set; }

        [StringLength(100)]
        public string manhinh { get; set; }

        public DateTime? ngaycapnhat { get; set; }

        public int? soluongton { get; set; }

        [StringLength(100)]
        public string pin { get; set; }

        public bool? trangthai { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DanhGia> DanhGias { get; set; }

        public virtual Hang Hang { get; set; }

        public virtual NhuCau NhuCau { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MetaDienthoai> MetaDienthoais { get; set; }
    }
}

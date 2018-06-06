namespace AuctionsModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TokenOrder")]
    public partial class TokenOrder
    {
        public Guid ID { get; set; }

        [Required]
        [StringLength(128)]
        public string UserID { get; set; }

        public int TokenCount { get; set; }

        public decimal Price { get; set; }

        [Required]
        [StringLength(20)]
        public string State { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }
    }
}

namespace AuctionsModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Bid")]
    public partial class Bid
    {
        [Key]
        [Column(Order = 0)]
        public string UserID { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid AuctionID { get; set; }

        [Key]
        [Column(Order = 2)]
        public DateTime TimeOfBidding { get; set; }

        public int TokensOffered { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual Auction Auction { get; set; }
    }
}

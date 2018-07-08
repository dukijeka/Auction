namespace AuctionsModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;

    [Table("Auction")]
    public partial class Auction
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Auction()
        {
            Bids = new HashSet<Bid>();
        }

        public Guid ID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Column(TypeName = "image")]
        public byte[] Image { get; set; }

        public int Duration { get; set; }

        public decimal StartingPrice { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime OppenedOn { get; set; }

        public DateTime ClosedOn { get; set; }

        //[Required]
        [StringLength(20)]
        public string State { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Bid> Bids { get; set; }

        public Bid GetLatestBid()
        {
            if (Bids.Count == 0)
            {
                return null;
            }

            Bid latestBid = Bids.OrderByDescending(x => x.TimeOfBidding).FirstOrDefault();
            return latestBid;
        }
    }
}

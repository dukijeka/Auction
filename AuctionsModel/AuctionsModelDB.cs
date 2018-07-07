namespace AuctionsModel
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class AuctionsModelDB : DbContext
    {
        //public AuctionsModelDB()
        //    : base("name=AuctionsModelConfig")
        //{
        //}

        public AuctionsModelDB()
        : base("name=DefaultConnection")
        {
        }
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<Auction> Auctions { get; set; }
        public virtual DbSet<Bid> Bids { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<TokenOrder> TokenOrders { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRole>()
                .HasMany(e => e.AspNetUsers)
                .WithMany(e => e.AspNetRoles)
                .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.Bids)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.TokenOrders)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Auction>()
                .Property(e => e.StartingPrice)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Auction>()
                .HasMany(e => e.Bids)
                .WithRequired(e => e.Auction)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TokenOrder>()
                .Property(e => e.Price)
                .HasPrecision(18, 0);
        }
    }
}

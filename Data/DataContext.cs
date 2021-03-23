using Blog_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_API.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        //public void Configure(EntityTypeBuilder<Post> builder)
        //{
        //    builder.ToTable("Posts");
        //    builder.HasOne(m => m.Author)
        //      .WithOne()
        //      .HasForeignKey<User>(a => a.UID);
        //}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany<Post>(s => s.Posts)
                .WithOne(x => x.Author);

            modelBuilder.Entity<Post>()
                .HasMany<Comment>(c => c.Comments)
                .WithOne(p => p.Post)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne<User>(u => u.Author)
                .WithOne()
                .OnDelete(DeleteBehavior.Restrict);

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }

    }
}

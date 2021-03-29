﻿// <auto-generated />
using System;
using Blog_API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Blog_API.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20210329190725_initial")]
    partial class initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Blog_API.Models.Comment", b =>
                {
                    b.Property<int>("CommentId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AuthorId");

                    b.Property<int?>("CommentParentId");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(300);

                    b.Property<DateTime>("Created");

                    b.Property<int>("Dislike");

                    b.Property<int>("Like");

                    b.Property<int>("PostId");

                    b.Property<bool>("Public");

                    b.HasKey("CommentId");

                    b.HasIndex("AuthorId");

                    b.HasIndex("PostId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("Blog_API.Models.Post", b =>
                {
                    b.Property<int>("PostId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AuthorId");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(2000);

                    b.Property<string>("CoverImagePath")
                        .HasMaxLength(100);

                    b.Property<DateTime>("Created");

                    b.Property<bool>("Deleted");

                    b.Property<string>("Excerpt")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<bool>("Public");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<DateTime>("Updated");

                    b.Property<int>("Views");

                    b.HasKey("PostId");

                    b.HasIndex("AuthorId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("Blog_API.Models.User", b =>
                {
                    b.Property<int>("UID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created");

                    b.Property<byte[]>("PasswordHash");

                    b.Property<byte[]>("PasswordSalt");

                    b.Property<string>("Uemail")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<string>("Ufname")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<string>("Uimg")
                        .HasMaxLength(50);

                    b.Property<string>("Ulname")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.HasKey("UID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Blog_API.Models.Comment", b =>
                {
                    b.HasOne("Blog_API.Models.User", "Author")
                        .WithMany("Comments")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Blog_API.Models.Post", "Post")
                        .WithMany("Comments")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Blog_API.Models.Post", b =>
                {
                    b.HasOne("Blog_API.Models.User", "Author")
                        .WithMany("Posts")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
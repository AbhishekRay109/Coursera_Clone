using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Scaf.Models;

namespace Kitana.Infrastructure
{
    public partial class SkillForgeDBContext : DbContext
    {
        public SkillForgeDBContext()
        {
        }

        public SkillForgeDBContext(DbContextOptions<SkillForgeDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Assessment> Assessments { get; set; }

        public virtual DbSet<CartItem> CartItems { get; set; }

        public virtual DbSet<Certificate> Certificates { get; set; }

        public virtual DbSet<Course> Courses { get; set; }

        public virtual DbSet<Favorites> Favorites { get; set; }

        public virtual DbSet<Lesson> Lessons { get; set; }

        public virtual DbSet<Note> Notes { get; set; }

        public virtual DbSet<Notification> Notifications { get; set; }

        public virtual DbSet<Review> Reviews { get; set; }

        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }

        public virtual DbSet<Thumbnail> Thumbnails { get; set; }

        public virtual DbSet<SFTransaction> Transactions { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<UserCourse> UserCourses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Paste connection string)
            .EnableSensitiveDataLogging();
        }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Assessment>(entity =>
            {
                entity.HasKey(e => e.AssessmentId).HasName("PK__Assessme__3D2BF81E8B390055");

                entity.Property(e => e.CorrectAnswer)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Question).HasColumnType("text");
                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Lesson).WithMany(p => p.Assessments)
                    .HasForeignKey(d => d.LessonId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_4");
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => e.CartItemId).HasName("PK__CartItem__488B0B0AA80B0045");

                entity.Property(e => e.Discount).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.Quantity).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Cart).WithMany(p => p.CartItems)
                    .HasForeignKey(d => d.CartId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_7");

                entity.HasOne(d => d.Course).WithMany(p => p.CartItems)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_8");
            });

            modelBuilder.Entity<Certificate>(entity =>
            {
                entity.HasKey(e => e.CertificateId).HasName("PK__Certific__BBF8A7C14B2A7785");

                entity.Property(e => e.IssueDate)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Course).WithMany(p => p.Certificates)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_21");

                entity.HasOne(d => d.User).WithMany(p => p.Certificates)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_20");
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.CourseId).HasName("PK__Courses__C92D71A7A773BF94");

                entity.Property(e => e.City)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.Country)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Description).HasColumnType("text");
                entity.Property(e => e.EnrollmentStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('open')");
                entity.Property(e => e.State)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.Title)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Instructor).WithMany(p => p.Courses)
                    .HasForeignKey(d => d.InstructorId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_1");
            });

            modelBuilder.Entity<Favorites>(entity =>
            {
                entity.HasKey(e => e.FavoriteId).HasName("PK__Favorite__CE74FAD591AEDBB3");

                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Course).WithMany(p => p.Favorites)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_12");

                entity.HasOne(d => d.User).WithMany(p => p.Favorites)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_11");
            });

            modelBuilder.Entity<Lesson>(entity =>
            {
                entity.HasKey(e => e.LessonId).HasName("PK__Lessons__B084ACD085243B19");

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Description).HasColumnType("text");
                entity.Property(e => e.Title)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Course).WithMany(p => p.Lessons)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_2");
            });

            modelBuilder.Entity<Note>(entity =>
            {
                entity.HasKey(e => e.NotesId).HasName("PK__Notes__35AB5BAA1A58436B");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E125FAFFF91");

                entity.Property(e => e.IsRead).HasDefaultValueSql("((0))");
                entity.Property(e => e.NotificationText).HasColumnType("text");
                entity.Property(e => e.Timestamp)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_15");
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.ReviewId).HasName("PK__Reviews__74BC79CE88E30592");

                entity.Property(e => e.ReviewText).HasColumnType("text");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ReviewText).HasColumnType("text");
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Course).WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_14");

                entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_13");
            });

            modelBuilder.Entity<ShoppingCart>(entity =>
            {
                entity.HasKey(e => e.CartId).HasName("PK__Shopping__51BCD7B72CCC8BC7");

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.User).WithMany(p => p.ShoppingCarts)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_6");
            });

            modelBuilder.Entity<Thumbnail>(entity =>
            {
                entity.HasKey(e => e.ThumbnailId).HasName("PK__Thumbnai__A6FB547F43BA734B");

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ThumbnailImage)
                    .IsRequired()
                    .HasColumnType("varbinary(max)");
                entity.Property(e => e.Active).HasColumnName("Active");
                entity.HasOne(d => d.Video).WithMany(p => p.Thumbnails)
                    .HasForeignKey(d => d.LessonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_5");
            });

            modelBuilder.Entity<SFTransaction>(entity =>
            {
                entity.HasKey(e => e.TransactionId).HasName("PK__Transact__55433A6BB8E809F0");

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.TransactionStatus)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.CourseIds).HasColumnType("text");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C6C216ACD");

                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.City)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.Country)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.FullName)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false);
                entity.Property(e => e.PostalCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);
                entity.Property(e => e.Role)
                    .HasMaxLength(20)
                    .IsUnicode(false);
                entity.Property(e => e.State)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserCourse>(entity =>
            {
                entity.HasKey(e => e.UserCourseId).HasName("PK__UserCour__58886ED40362E2E0");

                entity.ToTable("UserCourse");

                entity.HasOne(d => d.Course).WithMany(p => p.UserCourses)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_22");

                entity.HasOne(d => d.User).WithMany(p => p.UserCourses)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_17");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

﻿// <auto-generated />
using System;
using Kitana.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Kitana.Api.Migrations
{
    [DbContext(typeof(SkillForgeDBContext))]
    partial class SkillForgeDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Scaf.Models.Assessment", b =>
                {
                    b.Property<int>("AssessmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AssessmentId"));

                    b.Property<string>("CorrectAnswer")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int>("LessonId")
                        .HasColumnType("int");

                    b.Property<string>("Question")
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.HasKey("AssessmentId")
                        .HasName("PK__Assessme__3D2BF81E8B390055");

                    b.HasIndex("LessonId");

                    b.ToTable("Assessments");
                });

            modelBuilder.Entity("Scaf.Models.CartItem", b =>
                {
                    b.Property<int>("CartItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CartItemId"));

                    b.Property<int>("CartId")
                        .HasColumnType("int");

                    b.Property<int>("CourseId")
                        .HasColumnType("int");

                    b.Property<decimal>("Discount")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<int>("Quantity")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValueSql("((1))");

                    b.HasKey("CartItemId")
                        .HasName("PK__CartItem__488B0B0AA80B0045");

                    b.HasIndex("CartId");

                    b.HasIndex("CourseId");

                    b.ToTable("CartItems");
                });

            modelBuilder.Entity("Scaf.Models.Certificate", b =>
                {
                    b.Property<int>("CertificateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CertificateId"));

                    b.Property<byte[]>("CertificateFile")
                        .HasColumnType("varbinary(max)");

                    b.Property<int>("CourseId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("IssueDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("CertificateId")
                        .HasName("PK__Certific__BBF8A7C14B2A7785");

                    b.HasIndex("CourseId");

                    b.HasIndex("UserId");

                    b.ToTable("Certificates");
                });

            modelBuilder.Entity("Scaf.Models.Course", b =>
                {
                    b.Property<int>("CourseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CourseId"));

                    b.Property<int>("ActiveTime")
                        .HasColumnType("int");

                    b.Property<string>("City")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Country")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("EnrollmentStatus")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)")
                        .HasDefaultValueSql("('open')");

                    b.Property<int>("InstructorId")
                        .HasColumnType("int");

                    b.Property<int?>("Price")
                        .HasColumnType("int");

                    b.Property<string>("State")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Title")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.HasKey("CourseId")
                        .HasName("PK__Courses__C92D71A7A773BF94");

                    b.HasIndex("InstructorId");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("Scaf.Models.Favorites", b =>
                {
                    b.Property<int>("FavoriteId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FavoriteId"));

                    b.Property<int>("CourseId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("FavoriteId")
                        .HasName("PK__Favorite__CE74FAD591AEDBB3");

                    b.HasIndex("CourseId");

                    b.HasIndex("UserId");

                    b.ToTable("Favorites");
                });

            modelBuilder.Entity("Scaf.Models.Lesson", b =>
                {
                    b.Property<int>("LessonId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LessonId"));

                    b.Property<int>("CourseId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("VideoUrl")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("LessonId")
                        .HasName("PK__Lessons__B084ACD085243B19");

                    b.HasIndex("CourseId");

                    b.ToTable("Lessons");
                });

            modelBuilder.Entity("Scaf.Models.Note", b =>
                {
                    b.Property<int>("NotesId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("NotesId"));

                    b.Property<int>("LessonId")
                        .HasColumnType("int");

                    b.Property<string>("NoteLink")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("NotesId")
                        .HasName("PK__Notes__35AB5BAA1A58436B");

                    b.ToTable("Notes");
                });

            modelBuilder.Entity("Scaf.Models.Notification", b =>
                {
                    b.Property<int>("NotificationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("NotificationId"));

                    b.Property<bool?>("IsRead")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("((0))");

                    b.Property<string>("NotificationText")
                        .HasColumnType("text");

                    b.Property<DateTime?>("Timestamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("NotificationId")
                        .HasName("PK__Notifica__20CF2E125FAFFF91");

                    b.HasIndex("UserId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("Scaf.Models.Review", b =>
                {
                    b.Property<int>("ReviewId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ReviewId"));

                    b.Property<int>("CourseId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int?>("RatingValue")
                        .HasColumnType("int");

                    b.Property<string>("ReviewText")
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("ReviewId")
                        .HasName("PK__Reviews__74BC79CE88E30592");

                    b.HasIndex("CourseId");

                    b.HasIndex("UserId");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("Scaf.Models.SFTransaction", b =>
                {
                    b.Property<int>("TransactionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TransactionId"));

                    b.Property<int?>("CartId")
                        .HasColumnType("int");

                    b.Property<string>("CourseIds")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<decimal?>("TotalAmount")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<string>("TransactionStatus")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.HasKey("TransactionId")
                        .HasName("PK__Transact__55433A6BB8E809F0");

                    b.HasIndex("CartId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("Scaf.Models.ShoppingCart", b =>
                {
                    b.Property<int>("CartId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CartId"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("CartId")
                        .HasName("PK__Shopping__51BCD7B72CCC8BC7");

                    b.HasIndex("UserId");

                    b.ToTable("ShoppingCarts");
                });

            modelBuilder.Entity("Scaf.Models.Thumbnail", b =>
                {
                    b.Property<int>("ThumbnailId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ThumbnailId"));

                    b.Property<bool>("Active")
                        .HasColumnType("bit")
                        .HasColumnName("Active");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<int>("LessonId")
                        .HasColumnType("int");

                    b.Property<byte[]>("ThumbnailImage")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varbinary(255)")
                        .HasColumnName("ThumbnailImage");

                    b.HasKey("ThumbnailId")
                        .HasName("PK__Thumbnai__A6FB547F43BA734B");

                    b.HasIndex("LessonId");

                    b.ToTable("Thumbnails");
                });

            modelBuilder.Entity("Scaf.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<string>("Address")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("City")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Country")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Email")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("FullName")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("PasswordHash")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)");

                    b.Property<string>("PostalCode")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)");

                    b.Property<string>("Role")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)");

                    b.Property<byte[]>("Salt")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("State")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Username")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.HasKey("UserId")
                        .HasName("PK__Users__1788CC4C6C216ACD");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Scaf.Models.UserCourse", b =>
                {
                    b.Property<int>("UserCourseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserCourseId"));

                    b.Property<DateTime>("AvailableTill")
                        .HasColumnType("datetime2");

                    b.Property<int>("CompletedPercent")
                        .HasColumnType("int");

                    b.Property<int>("CourseId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("UserCourseId")
                        .HasName("PK__UserCour__58886ED40362E2E0");

                    b.HasIndex("CourseId");

                    b.HasIndex("UserId");

                    b.ToTable("UserCourse", (string)null);
                });

            modelBuilder.Entity("Scaf.Models.Assessment", b =>
                {
                    b.HasOne("Scaf.Models.Lesson", "Lesson")
                        .WithMany("Assessments")
                        .HasForeignKey("LessonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_4");

                    b.Navigation("Lesson");
                });

            modelBuilder.Entity("Scaf.Models.CartItem", b =>
                {
                    b.HasOne("Scaf.Models.ShoppingCart", "Cart")
                        .WithMany("CartItems")
                        .HasForeignKey("CartId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_7");

                    b.HasOne("Scaf.Models.Course", "Course")
                        .WithMany("CartItems")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_8");

                    b.Navigation("Cart");

                    b.Navigation("Course");
                });

            modelBuilder.Entity("Scaf.Models.Certificate", b =>
                {
                    b.HasOne("Scaf.Models.Course", "Course")
                        .WithMany("Certificates")
                        .HasForeignKey("CourseId")
                        .IsRequired()
                        .HasConstraintName("fk_21");

                    b.HasOne("Scaf.Models.User", "User")
                        .WithMany("Certificates")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_20");

                    b.Navigation("Course");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Scaf.Models.Course", b =>
                {
                    b.HasOne("Scaf.Models.User", "Instructor")
                        .WithMany("Courses")
                        .HasForeignKey("InstructorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_1");

                    b.Navigation("Instructor");
                });

            modelBuilder.Entity("Scaf.Models.Favorites", b =>
                {
                    b.HasOne("Scaf.Models.Course", "Course")
                        .WithMany("Favorites")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_12");

                    b.HasOne("Scaf.Models.User", "User")
                        .WithMany("Favorites")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("fk_11");

                    b.Navigation("Course");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Scaf.Models.Lesson", b =>
                {
                    b.HasOne("Scaf.Models.Course", "Course")
                        .WithMany("Lessons")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_2");

                    b.Navigation("Course");
                });

            modelBuilder.Entity("Scaf.Models.Notification", b =>
                {
                    b.HasOne("Scaf.Models.User", "User")
                        .WithMany("Notifications")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_15");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Scaf.Models.Review", b =>
                {
                    b.HasOne("Scaf.Models.Course", "Course")
                        .WithMany("Reviews")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_14");

                    b.HasOne("Scaf.Models.User", "User")
                        .WithMany("Reviews")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("fk_13");

                    b.Navigation("Course");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Scaf.Models.SFTransaction", b =>
                {
                    b.HasOne("Scaf.Models.ShoppingCart", "Cart")
                        .WithMany()
                        .HasForeignKey("CartId");

                    b.Navigation("Cart");
                });

            modelBuilder.Entity("Scaf.Models.ShoppingCart", b =>
                {
                    b.HasOne("Scaf.Models.User", "User")
                        .WithMany("ShoppingCarts")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("fk_6");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Scaf.Models.Thumbnail", b =>
                {
                    b.HasOne("Scaf.Models.Lesson", "Video")
                        .WithMany("Thumbnails")
                        .HasForeignKey("LessonId")
                        .IsRequired()
                        .HasConstraintName("fk_5");

                    b.Navigation("Video");
                });

            modelBuilder.Entity("Scaf.Models.UserCourse", b =>
                {
                    b.HasOne("Scaf.Models.Course", "Course")
                        .WithMany("UserCourses")
                        .HasForeignKey("CourseId")
                        .IsRequired()
                        .HasConstraintName("fk_22");

                    b.HasOne("Scaf.Models.User", "User")
                        .WithMany("UserCourses")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("fk_17");

                    b.Navigation("Course");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Scaf.Models.Course", b =>
                {
                    b.Navigation("CartItems");

                    b.Navigation("Certificates");

                    b.Navigation("Favorites");

                    b.Navigation("Lessons");

                    b.Navigation("Reviews");

                    b.Navigation("UserCourses");
                });

            modelBuilder.Entity("Scaf.Models.Lesson", b =>
                {
                    b.Navigation("Assessments");

                    b.Navigation("Thumbnails");
                });

            modelBuilder.Entity("Scaf.Models.ShoppingCart", b =>
                {
                    b.Navigation("CartItems");
                });

            modelBuilder.Entity("Scaf.Models.User", b =>
                {
                    b.Navigation("Certificates");

                    b.Navigation("Courses");

                    b.Navigation("Favorites");

                    b.Navigation("Notifications");

                    b.Navigation("Reviews");

                    b.Navigation("ShoppingCarts");

                    b.Navigation("UserCourses");
                });
#pragma warning restore 612, 618
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LibraryMgmt.Models;

public partial class LibraryMgmtContext : DbContext
{
    public LibraryMgmtContext()
    {
    }

    public LibraryMgmtContext(DbContextOptions<LibraryMgmtContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<BookStatus> BookStatuses { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=library_mgmt;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.BookId).HasName("PK__books__490D1AE126FCBE5C");

            entity.ToTable("books");

            entity.Property(e => e.BookId).HasColumnName("book_id");
            entity.Property(e => e.Author)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("author");
            entity.Property(e => e.Isbn)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("isbn");
            entity.Property(e => e.NoCopies).HasColumnName("no_copies");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("title");
        });

        modelBuilder.Entity<BookStatus>(entity =>
        {
            entity.HasKey(e => e.BookStatusId).HasName("PK__book_sta__79A6A668E5CD44EB");

            entity.ToTable("book_status", tb => tb.HasTrigger("trg_checkout_book"));

            entity.HasIndex(e => new { e.BookId, e.StudentId }, "uq_book_student_active")
                .IsUnique()
                .HasFilter("([date_returned] IS NULL)");

            entity.Property(e => e.BookStatusId).HasColumnName("book_status_id");
            entity.Property(e => e.BookId).HasColumnName("book_id");
            entity.Property(e => e.DateCheckout)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("date_checkout");
            entity.Property(e => e.DateReturned)
                .HasColumnType("datetime")
                .HasColumnName("date_returned");
            entity.Property(e => e.StudentId).HasColumnName("student_id");

            entity.HasOne(d => d.Book).WithMany(p => p.BookStatuses)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("FK__book_stat__book___4D94879B");

            entity.HasOne(d => d.Student).WithMany(p => p.BookStatuses)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__book_stat__stude__4E88ABD4");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__courses__8F1EF7AE0C12B53E");

            entity.ToTable("courses");

            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.CourseName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("course_name");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__students__2A33069AE014C0CD");

            entity.ToTable("students");

            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.EmailAddress)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("email_address");
            entity.Property(e => e.FirstName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("last_name");

            entity.HasOne(d => d.Course).WithMany(p => p.Students)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__students__course__49C3F6B7");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

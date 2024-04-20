using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BankApplication.Models
{
    public partial class BankDBContext : DbContext
    {
        public BankDBContext()
        {
        }

        public BankDBContext(DbContextOptions<BankDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<TransactionTbl> TransactionTbls { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=BankDB;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.AccNumber)
                    .HasName("PK__Account__A24910AB0D6594AC");

                entity.ToTable("Account");

                entity.HasIndex(e => new { e.AccType, e.CustomerId }, "unique_acc_record")
                    .IsUnique();

                entity.Property(e => e.AccNumber).HasColumnName("acc_number");

                entity.Property(e => e.AccBalance).HasColumnName("acc_balance");

                entity.Property(e => e.AccCreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("acc_created_at")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.AccType)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("acc_type");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("is_active")
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Account__custome__04E4BC85");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer");

                entity.HasIndex(e => e.MobileNumber, "UQ__Customer__30462B0F92DE480B")
                    .IsUnique();

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.City)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("city");

                entity.Property(e => e.CustomerImage)
                    .HasMaxLength(2000)
                    .HasColumnName("customer_image");

                entity.Property(e => e.Dob)
                    .HasColumnType("date")
                    .HasColumnName("dob");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("first_name");

                entity.Property(e => e.LastName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("last_name");

                entity.Property(e => e.MobileNumber)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("mobile_number");
            });

            modelBuilder.Entity<TransactionTbl>(entity =>
            {
                entity.HasKey(e => e.TId)
                    .HasName("PK__Transact__E579775F3C7A2394");

                entity.ToTable("TransactionTbl");

                entity.Property(e => e.TId).HasColumnName("t_id");

                entity.Property(e => e.AccNumber).HasColumnName("acc_number");

                entity.Property(e => e.Summary)
                    .IsUnicode(false)
                    .HasColumnName("summary");

                entity.Property(e => e.TAmount).HasColumnName("t_amount");

                entity.Property(e => e.TTime)
                    .HasColumnType("datetime")
                    .HasColumnName("t_time");

                entity.Property(e => e.TType)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("t_type");

                entity.HasOne(d => d.AccNumberNavigation)
                    .WithMany(p => p.TransactionTbls)
                    .HasForeignKey(d => d.AccNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Transacti__acc_n__37703C52");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.MobileNumber, "UQ__Users__30462B0F29383F13")
                    .IsUnique();

                entity.HasIndex(e => e.Email, "UQ__Users__AB6E61640C93B5AD")
                    .IsUnique();

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.Email)
                    .HasMaxLength(1000)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.MobileNumber)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("mobile_number");

                entity.Property(e => e.Password)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.Role)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("role");

                entity.Property(e => e.Salt)
                    .IsUnicode(false)
                    .HasColumnName("salt");

                entity.Property(e => e.Username)
                    .HasMaxLength(1000)
                    .IsUnicode(false)
                    .HasColumnName("username");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

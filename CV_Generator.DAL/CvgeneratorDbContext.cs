using System;
using System.Collections.Generic;
using CV_Generator.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CV_Generator.DAL;

public partial class CvgeneratorDbContext : DbContext
{
    public CvgeneratorDbContext()
    {
    }

    public CvgeneratorDbContext(DbContextOptions<CvgeneratorDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<CurriculumVitae> CurriculumVitaes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

        => optionsBuilder.UseSqlServer(GetConnectionString());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Account__3213E83FCD0B3A55");

            entity.ToTable("Account");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("fullName");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Role).HasColumnName("role");
        });

        modelBuilder.Entity<CurriculumVitae>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Curricul__3213E83F52C730E5");

            entity.ToTable("CurriculumVitae");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("createAt");
            entity.Property(e => e.CreateBy).HasColumnName("createBy");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");

            entity.HasOne(d => d.CreateByNavigation).WithMany(p => p.CurriculumVitaes)
                .HasForeignKey(d => d.CreateBy)
                .HasConstraintName("FK__Curriculu__creat__398D8EEE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    private string GetConnectionString()
    {
        IConfiguration config = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, true)
                    .Build();
        var strConn = config["ConnectionStrings:DefaultConnectionStringDB"];
        //coi chừng 9 triêu: dấu cách gây nên 2 attri k khớp file json -> k đọc được chuỗi connection

        return strConn;
    }
}

using System;
using System.Collections.Generic;
using ImageCompress.Image.DBModels.ImageCompress;
using Microsoft.EntityFrameworkCore;

namespace ImageCompress.Image.DBContents;

public partial class PostgresContext : DbContext
{
    public PostgresContext(DbContextOptions<PostgresContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ImageInfo> ImageInfo { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ImageInfo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Image_pkey");

            entity.ToTable("ImageInfo", "imgcps_process");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CompressFinishDate).HasColumnType("timestamp without time zone");
            entity.Property(e => e.ContentType).HasMaxLength(50);
            entity.Property(e => e.OriginFileName).HasMaxLength(254);
            entity.Property(e => e.UploadDate).HasColumnType("timestamp without time zone");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

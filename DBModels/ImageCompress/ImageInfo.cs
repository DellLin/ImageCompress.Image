using System;
using System.Collections.Generic;

namespace ImageCompress.Image.DBModels.ImageCompress;

public partial class ImageInfo
{
    public Guid Id { get; set; }

    public Guid? AccountId { get; set; }

    public string? OriginFileName { get; set; }

    public string? FilePath { get; set; }

    public int? State { get; set; }

    public DateTime? CreateDate { get; set; }

    public Guid? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    public int? OriginSize { get; set; }

    public int? CompressedSize { get; set; }

    public string? ContentType { get; set; }
}

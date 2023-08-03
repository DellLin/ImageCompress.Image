using System;
using System.Collections.Generic;

namespace ImageCompress.Image.DBModels.ImageCompress;

public partial class ImageInfo
{
    public Guid Id { get; set; }

    public Guid? AccountId { get; set; }

    public string? OriginFileName { get; set; }

    public int? OriginSize { get; set; }

    public int? CompressedSize { get; set; }

    public string? ContentType { get; set; }

    public int? Quality { get; set; }

    public int? State { get; set; }

    public DateTime? UploadDate { get; set; }

    public DateTime? CompressFinishDate { get; set; }
}

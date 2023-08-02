using System.Net.Mime;

using Google.Cloud.Storage.V1;
using Grpc.Core;
using ImageCompress;
using ImageCompress.Image.DBContents;
using ImageCompress.Image.DBModels.ImageCompress;
using Microsoft.EntityFrameworkCore;

public class ImageService : ImageCompress.ImageService.ImageServiceBase
{
    private const string BUCKET_NAME = "image-compress-upload-image";
    private readonly ILogger<ImageService> _logger;
    private readonly PostgresContext _postgresContext;
    private readonly StorageClient _storageClient;

    public ImageService(ILogger<ImageService> logger,
        PostgresContext postgresContext
        )
    {
        _logger = logger;
        _postgresContext = postgresContext;
        _storageClient = StorageClient.Create();
    }

    public override async Task<GetImageInfoResponse> GetImageInfo(GetImageInfoRequest request, ServerCallContext context)
    {
        var response = new GetImageInfoResponse();
        var imageList = await _postgresContext.ImageInfo.Where(t => t.AccountId == Guid.Parse(request.AccountId) && t.State >= 0).ToListAsync();
        foreach (var image in imageList)
        {
            response.Images.Add(new ImageInfoItem
            {
                Id = image.Id.ToString(),
                FileName = image.OriginFileName,
                OriginSize = image.OriginSize ?? 0,
                CompressedSize = image.CompressedSize ?? 0,
                ContentType = image.ContentType ?? "",
                State = image.State ?? 0,
            });
        }
        return response;

    }
    public override async Task<UploadResponse> UploadImage(UploadRequest request, ServerCallContext context)
    {
        var response = new UploadResponse();
        var fileId = Guid.NewGuid();
        var fileByteArray = request.FileContent.ToByteArray();
        var imageInfo = new ImageInfo
        {
            Id = fileId,
            AccountId = Guid.Parse(request.AccountId),
            OriginFileName = request.FileName,
            OriginSize = fileByteArray.Length,
            ContentType = request.ContentType,
            State = 0,
            CreateDate = DateTime.Now,
            CreateBy = Guid.Parse(request.AccountId),
        };
        _postgresContext.Add(imageInfo);
        _storageClient.UploadObject(BUCKET_NAME, fileId.ToString(), request.ContentType, new MemoryStream(fileByteArray));

        await _postgresContext.SaveChangesAsync();
        var imageInfoItem = new ImageInfoItem
        {
            Id = imageInfo.Id.ToString(),
            FileName = imageInfo.OriginFileName,
            OriginSize = imageInfo.OriginSize ?? 0,
            CompressedSize = imageInfo.CompressedSize ?? 0,
            ContentType = imageInfo.ContentType,
            State = imageInfo.State ?? 0,
        };
        response.Success = true;
        response.Image = imageInfoItem;
        return response;
    }

    public override async Task<DownloadResponse> DownloadImage(DownloadRequest request, ServerCallContext context)
    {
        var response = new DownloadResponse();
        var imageInfo = _postgresContext.ImageInfo.FirstOrDefault(t => t.Id == Guid.Parse(request.FileId));
        if (imageInfo == null)
            return response;
        using MemoryStream stream = new();
        await _storageClient.DownloadObjectAsync(BUCKET_NAME, request.FileId, stream);
        response.FileContent = Google.Protobuf.ByteString.CopyFrom(stream.ToArray());
        response.FileName = imageInfo.OriginFileName;
        response.ContentType = imageInfo.ContentType;
        return response;
    }
    public override async Task<DeleteResponse> DeleteImage(DeleteRequest request, ServerCallContext context)
    {
        var response = new DeleteResponse();
        var fileId = request.Image.Id;
        var deleteImage = _postgresContext.ImageInfo.Find(Guid.Parse(fileId));
        if (deleteImage == null)
        {
            response.Success = false;
            response.Message = "Image not find.";
            return response;
        }
        deleteImage.State = -1;
        _postgresContext.SaveChanges();
        response.Success = true;
        return response;

    }
}
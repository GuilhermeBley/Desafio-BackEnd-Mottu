using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Bl.Mottu.Maintenance.Core.Repository;
using Bl.Mottu.Maintenance.Infrastructure.Config;
using Microsoft.Extensions.Options;

namespace Bl.Mottu.Maintenance.Infrastructure.Repository;

internal class StreamFileRepository : IStreamFileRepository
{
    private const string Container = "default-files";
    private readonly BlobContainerClient _container;
    private bool _containerCreated;

    public StreamFileRepository(IOptions<StorageAccountOption> options)
    {
        var blobClient = new BlobServiceClient(options.Value.ConnectionString);
        _container = blobClient.GetBlobContainerClient(Container);
    }

    public async Task<Uri> UploadAsync(Stream stream, string fileName, CancellationToken cancellationToken = default)
    {
        if (_containerCreated is false)
        {
            _containerCreated = true;
            await _container.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob, cancellationToken: cancellationToken);
        }

        var blobClient = _container.GetBlobClient(fileName);

        var blobHttpHeaders = new BlobHttpHeaders
        {
            ContentType = "image/png"
        };

        await blobClient.UploadAsync(
            stream,
            new BlobUploadOptions
            {
                HttpHeaders = blobHttpHeaders,
                Conditions = null // This allows overwriting existing files
            },
            cancellationToken);

        return blobClient.Uri;
    }
}

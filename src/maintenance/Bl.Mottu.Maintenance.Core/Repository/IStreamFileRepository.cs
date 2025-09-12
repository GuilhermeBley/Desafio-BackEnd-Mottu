namespace Bl.Mottu.Maintenance.Core.Repository;

public interface IStreamFileRepository
{
    /// <summary>
    /// Upload a new file, if it already exists, the old file will be replaced.
    /// </summary>
    Task<Uri> UploadAsync(Stream stream, string fileName, CancellationToken cancellationToken = default);
}

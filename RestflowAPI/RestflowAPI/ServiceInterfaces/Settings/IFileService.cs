namespace RestflowAPI.ServiceInterfaces.Settings
{
	public interface IFileService
	{
		Task<string> UploadFileAsync(IFormFile file, string folderName, CancellationToken cancellationToken);
		void DeleteFile(string? filePath);
	}
}

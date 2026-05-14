using RestflowAPI.ServiceInterfaces.Settings;

namespace RestflowAPI.Services.Settings
{
	public class FileService : IFileService
	{
		private readonly IWebHostEnvironment _environment;
		public FileService(IWebHostEnvironment webHostEnvironment)
		{
			_environment = webHostEnvironment;
		}
		public void DeleteFile(string? filePath)
		{
			if (string.IsNullOrEmpty(filePath)) return;
			var webRootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
			var fullPath = Path.Combine(webRootPath, filePath.TrimStart('/'));
			if (File.Exists(fullPath))
			{
				File.Delete(fullPath);
			}
		}

		public async Task<string> UploadFileAsync(IFormFile file, string folderName, CancellationToken cancellationToken)
		{
			if (file == null || file.Length == 0)
				throw new ArgumentException("File is empty.");

			var webRootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
			var uploadsFolder = Path.Combine(webRootPath, "uploads", folderName);

			if (!Directory.Exists(uploadsFolder))
				Directory.CreateDirectory(uploadsFolder);

			var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
			var filePath = Path.Combine(uploadsFolder, fileName);

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(stream, cancellationToken);
			}

			return Path.Combine("uploads", folderName, fileName).Replace("\\", "/");
		}
	}
}

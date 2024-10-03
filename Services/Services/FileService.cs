using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Services.Services;

public class FileService : IScopedDependency, IFileService
{
    private readonly string _uploadFolder;
    private readonly ILogger<FileService> _logger;

    public FileService(IHostEnvironment hostEnvironment, ILogger<FileService> logger)
    {
        _uploadFolder = Path.Combine(hostEnvironment.ContentRootPath, "Uploads");
        if (!Directory.Exists(_uploadFolder))
            Directory.CreateDirectory(_uploadFolder);
        _logger = logger;
    }
    public string Save(InputImage file, string id, ImageType imageType)
    {
        var fileName = GetFile($"{id}_{imageType}");
        if (fileName is not null)
        {
            File.Delete(fileName);
        }
        var ext = Path.GetExtension(file.Name);
        string filePath = Path.Combine(_uploadFolder, $"{id}_{imageType}{ext}");
        File.WriteAllBytes(filePath, file.FileContent);

        return filePath;
    }
    public (byte[] content, string fileName) Get(string imageName)
    {
        var filePath = GetFile(imageName);
        if (filePath is null)
            return (default, null);
        using var fileStream = new FileStream(filePath, FileMode.Open);
        using var memoryStream = new MemoryStream();
        fileStream.CopyTo(memoryStream);
        var imageBytes = memoryStream.ToArray();

        return (imageBytes, Path.GetFileName(filePath));
    }

    public void Delete(string imageName)
    {
        var file = GetFile(imageName);
        if (file is not null)
        {
            File.Delete(file);
        }
    }

    private string GetFile(string imageName)
    {
        var files = Directory.EnumerateFiles(_uploadFolder, "*.*", SearchOption.AllDirectories).ToList();
        if (files.Any())
        {
            var names = files.Select(Path.GetFileName).ToList();

            var find = names.SingleOrDefault(s => s.StartsWith(imageName, System.StringComparison.CurrentCultureIgnoreCase));
            if (find is not null)
                return Path.Combine(_uploadFolder, find);
            else return null;
        }
        else
            return null;
    }
    public IEnumerable<string> GetFiles(string imageId)
    {
        var files = Directory.GetFiles(_uploadFolder, "*.*", SearchOption.AllDirectories);
        var names = files.Select(Path.GetFileName);

        var finds = names.Where(s => s.StartsWith(imageId, System.StringComparison.CurrentCultureIgnoreCase));
        return finds.Select(i => Path.Combine(_uploadFolder, i));
    }

    //public T Save<T>(T image, InputImage file, ImageType imageType) where T : IImage
    //{
    //    if (file != null)
    //    {
    //        if (image.Id == Guid.Empty)
    //            image.Id = Guid.NewGuid();

    //        var uniqueFileName = $"{image.Id}_{imageType}";
    //        //var uniqueFileName = $"{image.Id}_{DateTime.UtcNow.ConvertTimeZone():yyMMddHHmmss}";
    //        Directory.CreateDirectory(_uploadFolder);
    //        var ext = Path.GetExtension(file.Name);
    //        string filePath = Path.Combine(_uploadFolder, $"{uniqueFileName}{ext}");

    //        //image.Name = uniqueFileName;
    //        image.PhysicalPath = filePath;
    //        //image.FileType = Path.GetExtension(file.Name);

    //        File.WriteAllBytes(filePath, file.FileContent);
    //    }

    //    return image;
    //}
}
public enum ImageType
{
    Logo,
    FirstScreen,
    LastScreen,
    Thumbnail,
    Background,
    SwipeCard,
    printLogo,
    ServiceThumbnail,


}
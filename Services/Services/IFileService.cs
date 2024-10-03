using Common;
using System.Collections.Generic;

namespace Services.Services;

public interface IFileService
{
    //T Save<T>(T itemImage, InputImage file, ImageType imageType) where T : IImage;
    public string Save(InputImage file, string id, ImageType imageType);
    public (byte[] content, string fileName) Get(string imageName);
    public IEnumerable<string> GetFiles(string imageId);
    void Delete(string itemImage);
}
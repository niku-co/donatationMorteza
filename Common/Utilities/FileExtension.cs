using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Http;

namespace Common.Utilities;

public static class FileExtension
{
    public static string CalcMd5(this string filename)
    {
        using var md5 = MD5.Create();
        using var stream = System.IO.File.OpenRead(filename);
        var md5Hash = md5.ComputeHash(stream);

        return string.Concat(md5Hash.Select(x => x.ToString("X2")));
    }
    public static async Task<InputImage> GetInputImageAsync(this IFormFile formFile, CancellationToken cancellationToken)
    {
        await using var memoryStream = new MemoryStream();
        await formFile.CopyToAsync(memoryStream, cancellationToken);
        var inputImage = new InputImage()
        {
            FileContent = memoryStream.ToArray(),
            FileType = Path.GetExtension(formFile.FileName),
            Name = formFile.FileName
        };
        return inputImage;
    }
}

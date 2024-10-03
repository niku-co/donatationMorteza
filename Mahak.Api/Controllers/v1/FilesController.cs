using Mahak.Api.Models;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;

namespace Mahak.Api.Controllers.v1;

[ApiVersion("1")]
//[AllowAnonymous]
public class FilesController : BaseController
{
    private readonly ILogger<FilesController> _logger;
    private readonly IWebHostEnvironment _env;
    public FilesController(IWebHostEnvironment webHostEnvironment, ILogger<FilesController> logger)
    {
        _env = webHostEnvironment;
        _logger = logger;
        if (string.IsNullOrWhiteSpace(_env.WebRootPath))
        {
            _env.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        }
    }

    //[HttpGet]
    //[Authorize(AuthenticationSchemes = "BasicAuthentication")]
    //public ApiResult<string> GetApkFile()
    //{
    //    //FTP Server URL.
    //    string ftp = "ftp://ftp.nikukiosk.com:3035/";

    //    //FTP Folder name. Leave blank if you want to list files from root folder.
    //    string ftpFolder = "files/110/APK";

    //    //Create FTP Request.
    //    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftp + ftpFolder);
    //    request.Method = Ftp.ListDirectoryDetails;

    //    //Enter FTP Server credentials.
    //    request.Credentials = new NetworkCredential("nikuftp", "niku@niku");
    //    request.UsePassive = true;
    //    request.UseBinary = true;
    //    request.EnableSsl = false;

    //    //Fetch the Response and read it using StreamReader.
    //    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
    //    //List<string> entries = new List<string>();
    //    string fileName;
    //    using (StreamReader reader = new(response.GetResponseStream()))
    //    {
    //        //entries = reader.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
    //        fileName = reader.ReadLine().Split(' ').Last();
    //    }
    //    response.Close();
    //    return fileName;
    //}

    [HttpGet]
    //[Authorize(AuthenticationSchemes = "BasicAuthentication")]
    public ApiResult<FileInfoDto> GetApkFileInfo()
    {
        //string ftp = "ftp://ftp.nikukiosk.com/";
        //string ftpFolder = "files/110/APK";

        //FtpConfig ftpConfig = new();
        //ftpConfig.EncryptionMode = FtpEncryptionMode.None;
        //var client = new FtpClient(ftp, "nikuftp", "niku@niku", 3035, ftpConfig);
        //client.AutoConnect();
        FileInfoDto fileInfoDto = null;
        //foreach (FtpListItem item in client.GetListing(ftpFolder))
        //{
        //    if (item.Type == FtpObjectType.File && item.Name.StartsWith("donation", StringComparison.InvariantCultureIgnoreCase))
        //    {
        //        long size = client.GetFileSize(item.FullName);
        //        return new FileInfoDto
        //        {
        //            FileName = item.Name,
        //            FileSize = size
        //        };
        //        //// calculate a hash for the file on the server side (default algorithm)
        //        //FtpHash hash = client.GetChecksum(item.FullName);
        //    }

        //    // get modified date/time of the file or folder
        //    DateTime time = client.GetModifiedTime(item.FullName);
        //}
        //return fileInfoDto;

        //var path = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\assets\apks"}"; ;// Path.Combine(Directory.GetCurrentDirectory(), "/Apks");

        _logger.LogInformation("WebRootPath: {0}", _env.WebRootPath);
        var path = Path.Combine(_env.WebRootPath, "assets", "apks");
        _logger.LogInformation("Apk path: {0}", path);

        var dirInfo = new DirectoryInfo(path);

        if (!dirInfo.Exists)
        {
            return fileInfoDto;
        }

        var files = dirInfo.GetFiles();
        foreach (var file in files)
        {
            if (file.Name.StartsWith("donation", StringComparison.InvariantCultureIgnoreCase))
            {
                long size = file.Length;
                return new FileInfoDto
                {
                    FileName = file.Name,
                    FileSize = size
                };
            }
        }
        return fileInfoDto;

    }

    //[HttpGet("[Action]")]
    //[Authorize(AuthenticationSchemes = "BasicAuthentication")]
    //public IActionResult GetApkFile(string fileName)
    //{

    //    var path = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\assets\apks"}";

    //    if (!Directory.Exists(path))
    //    {
    //        return null;
    //    }

    //    var files = Directory.GetFiles(path);

    //    foreach (var file in files)
    //    {
    //        if (file.Contains("donation", StringComparison.InvariantCultureIgnoreCase))
    //        {
    //            var fullPath = file;
    //            byte[] apkfBytes = System.IO.File.ReadAllBytes(fullPath);
    //            //MemoryStream ms = new MemoryStream(pdfBytes);
    //            return File(apkfBytes, "application/vnd.android.package-archive", fileName);// "application/octet-stream"
    //            //return PhysicalFile(fullPath, "application/vnd.android.package-archive", fileName);                                                                       // return File(pdfBytes, "image/png", fileName);// "application/octet-stream"
    //        }
    //    }
    //    return null;

    //}

    //[HttpGet("download")]
    //public async Task<IActionResult> DownloadApk()
    //{
    //    var httpClient = new HttpClient();
    //    string url = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value+ "//assets//apks//donation.apk";
    //    var response = await httpClient.GetAsync(url);

    //    if (response.IsSuccessStatusCode)
    //    {
    //        var apkData = await response.Content.ReadAsByteArrayAsync();
    //        return File(apkData, "application/vnd.android.package-archive", "your-apk-file.apk");
    //    }

    //    return NotFound();
    //}

    //[HttpGet("[Action]")]
    //public HttpResponseMessage GetApkFileByStream()
    //{
    //    var path = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\assets\apks"}";

    //    if (!Directory.Exists(path))
    //    {
    //        return null;
    //    }

    //    var files = Directory.GetFiles(path);

    //    foreach (var file in files)
    //    {
    //        if (file.Contains("donation", StringComparison.InvariantCultureIgnoreCase))
    //        {
    //            string apkFilePath = file;

    //            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
    //            response.Content = new StreamContent(new FileStream(apkFilePath, FileMode.Open, FileAccess.Read));
    //            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
    //            response.Content.Headers.ContentDisposition.FileName = "donation.apk";
    //            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.android.package-archive");

    //            return response;                                                                       // return File(pdfBytes, "image/png", fileName);// "application/octet-stream"
    //        }
    //    }
    //    return null;


    //}
}

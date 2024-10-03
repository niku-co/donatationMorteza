using Microsoft.AspNetCore.Http;

namespace Services.Common
{
    public class ImageInput
    {
        public string Name { get; set; }
        public string FileType { get; set; }
        public byte[] FileContent { get; set; }
    }

    public class Utils
    {

    }

}
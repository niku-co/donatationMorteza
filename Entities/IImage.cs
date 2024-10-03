using System;

namespace Entities
{
    public interface IImage
    {
        Guid Id { set; get; }
        //string FileType { get; set; }
        //string Name { get; set; }
        string PhysicalPath { get; set; }
    }
}
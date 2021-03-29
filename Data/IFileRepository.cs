using Castle.Core.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_API.Data
{
    public interface IFileRepository
    {
        string uploadImg(IFormFile file, string directory, string filename, bool compress, int sizeInKB, bool thumbnail);
        bool deleteFileByName(string directory, string filename);
    }
}

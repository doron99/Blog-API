using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Blog_API.Data
{
    public class FileRepository : IFileRepository
    {
        private readonly IHostingEnvironment _appEnvironment;

        private readonly IConfiguration _config;
        public FileRepository(IConfiguration config, IHostingEnvironment appEnvironment)
        {
            _config = config;
            _appEnvironment = appEnvironment;
        }
        public string uploadImg(IFormFile file, string directory, string filename, bool compress, int sizeInKB, bool thumbnail)
        {
            string ret = "";
            //root
            string webRootPath = _appEnvironment.WebRootPath + "/data/";

            if (file != null)
            {
                List<string> ImageExtensions = new List<string> { ".JPG", ".JPE", ".BMP", ".GIF", ".PNG" };
                var extension = Path.GetExtension(file.FileName);
                var size = file.Length;

                if (size > (5 * 1024 * 1024))
                    return "";

                if (!ImageExtensions.Contains(Path.GetExtension(extension).ToUpperInvariant()))
                    return "";

                //root+directory
                string FullPath = Path.Combine(webRootPath + directory, "");

                if (!Directory.Exists(FullPath))
                    Directory.CreateDirectory(FullPath);

                try
                {
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        Image returnImage = Image.FromStream(ms);
                        string[] fn = filename.Split(".");
                        reduceIMGproccessingFromImage(returnImage, directory, filename, sizeInKB);
                        ret = filename;
                    }

                }
                catch (Exception ex)
                {
                    return "";
                }

            }
            return ret;
        }
        public Image reduceIMGproccessingFromImage(Image img, string directory, string filename, int kb)
        {
            string webRootPath = _appEnvironment.WebRootPath;
            //string filepathToSave = webRootPath + "/data" + "/" + subDirectory + "/" + filename;


            var i = img;
            long legnth = 0;
            byte[] imageByte = new byte[0];

            float percentages = 1;
            int _width = i.Width;
            int _height = i.Height;
            do
            {
                percentages = (percentages - (float)0.1);
                _width = (int)(_width * percentages);
                _height = (int)(_height * percentages);

                var newImage = new Bitmap(_width, _height);
                using (var g = Graphics.FromImage(newImage))
                {
                    g.DrawImage(i, 0, 0, _width, _height);
                }
                imageByte = imageToByteArray(newImage);
                legnth = imageByte.Length;

            } while (legnth / 1024 > kb);
            Image x = (Bitmap)((new ImageConverter()).ConvertFrom(imageByte));

            x.Save(webRootPath + "/data" + "/" + directory + "/" + filename);
            return x;


        }
        public static byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();
        }
        public bool deleteFileByName(string directory, string filename)
        {
            string webRootPath = _appEnvironment.WebRootPath + "/data/";
            string filepath = webRootPath + "/" + directory + "/" + filename;
            //filepath = filepath.Replace("/", @"\");
            bool ret = true;
            string[] ret1 = Convert.ToString(filepath).Split(new string[] { "wwwroot" }, StringSplitOptions.None);

            if (System.IO.File.Exists(filepath))
            {
                try
                {
                    FileStream s = new FileStream(filepath, FileMode.Open); //openning stream, them file in use by a process
                    s.Close();
                    s.Dispose();
                    File.Delete(filepath);
                }
                catch (System.IO.IOException e)
                {
                    Console.WriteLine(e.Message);
                    ret = false;
                }
            }
            return ret;
        }


    }
}

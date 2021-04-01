using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace Blog_API.Controllers
{
    public class FallbackController : Controller
    {
        public IActionResult Index()
        {
            return PhysicalFile( Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/ClientApp", "index.html"), "text/HTML");
        }
    }
}
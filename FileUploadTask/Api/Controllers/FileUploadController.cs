using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Api.Controllers
{
    [Route("/api/[controller]")]
    public sealed class FileUploadController : ControllerBase
    {

        public readonly string UploadPath =  $"{Directory.GetCurrentDirectory()}/uploads";
     /*   [HttpPost]
        public IActionResult Upload(List<IFormFile> files) // I could use IFormFile it's easy , but for understanding theme better i won't use it
        {

            if(files.Count == 0)
                return  BadRequest("No files selected");
        }*/

     [HttpPost]
     public IActionResult Upload()
     {
    
         var boundary = HeaderUtilities.RemoveQuotes(MediaTypeHeaderValue)
     }
     
}
}

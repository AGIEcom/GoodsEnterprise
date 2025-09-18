using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace GoodsEnterprise.Web.Controller
{
    /// <summary>
    /// Controller to serve images from outside wwwroot directory
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ImageController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Serves images from the configured upload path
        /// </summary>
        /// <param name="folder">The folder name (e.g., Brand, Category, Product)</param>
        /// <param name="filename">The image filename</param>
        /// <returns>The image file</returns>
        [HttpGet("{folder}/{filename}")]
        public IActionResult GetImage(string folder, string filename)
        {
            try
            {
                string uploadPath = _configuration["Application:UploadPath"];
                string imagePath = Path.Combine(uploadPath, folder, filename);

                // Security check - ensure the path is within the upload directory
                string normalizedUploadPath = Path.GetFullPath(uploadPath);
                string normalizedImagePath = Path.GetFullPath(imagePath);
                
                if (!normalizedImagePath.StartsWith(normalizedUploadPath))
                {
                    return BadRequest("Invalid file path");
                }

                if (!System.IO.File.Exists(imagePath))
                {
                    return NotFound("Image not found");
                }

                var fileBytes = System.IO.File.ReadAllBytes(imagePath);
                var contentType = GetContentType(filename);

                return File(fileBytes, contentType);
            }
            catch
            {
                return NotFound("Image not found");
            }
        }

        /// <summary>
        /// Gets the content type based on file extension
        /// </summary>
        /// <param name="filename">The filename</param>
        /// <returns>The MIME content type</returns>
        private string GetContentType(string filename)
        {
            var extension = Path.GetExtension(filename).ToLowerInvariant();
            
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".gif":
                    return "image/gif";
                case ".bmp":
                    return "image/bmp";
                case ".webp":
                    return "image/webp";
                case ".svg":
                    return "image/svg+xml";
                default:
                    return "application/octet-stream";
            }
        }
    }
}

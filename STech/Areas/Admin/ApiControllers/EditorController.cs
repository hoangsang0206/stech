using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STech.Filters;

namespace STech.Areas.Admin.ApiControllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [AdminAuthorize]
    public class EditorController : ControllerBase
    {
        private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Areas", "Admin", "wwwroot/editorjs/image");

        private readonly HttpClient _httpClient;

        public EditorController(HttpClient httpClient)
        {
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }

            _httpClient = httpClient;
        }

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest(new { error = "No file uploaded" });
            }

            string filePath = Path.Combine(_uploadPath, image.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            string fileUrl = $"/admin/editorjs/image/{image.FileName}";

            return Ok(new
            {
                success = 1,
                file = new
                {
                    url = fileUrl
                }
            });
        }

        [HttpGet("fetch-url")]
        public async Task<IActionResult> FetchUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest(new { error = "URL is required" });
            }

            try
            {
                string response = await _httpClient.GetStringAsync(url);
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(response);

                string title = htmlDocument.DocumentNode.SelectSingleNode("//title")?.InnerText ?? "";
                string description = htmlDocument.DocumentNode.SelectSingleNode("//meta[@name='description']")?.GetAttributeValue("content", "") ?? "";
                string image = htmlDocument.DocumentNode.SelectSingleNode("//meta[@property='og:image']")?.GetAttributeValue("content", "") ?? "";

                return Ok(new
                {
                    success = 1,
                    meta = new
                    {
                        title,
                        description,
                        image
                    }
                });
            }
            catch
            {
                return StatusCode(500, new { error = "Failed to fetch URL" });
            }
        }
    }
}

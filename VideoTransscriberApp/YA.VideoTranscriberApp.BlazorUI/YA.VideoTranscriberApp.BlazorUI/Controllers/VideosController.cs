using Microsoft.AspNetCore.Mvc;
using Xabe.FFmpeg;

namespace YA.VideoTranscriberApp.BlazorUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideosController : ControllerBase
    {
        private readonly IWebHostEnvironment env;

        public VideosController(IWebHostEnvironment env)
        {
            this.env = env;
        }

        [HttpPost]
        public async Task<IActionResult> UploadAsync(VideoUploadRequest request, CancellationToken token)
        {
            if (request.Video.Length > 0)
            {
                var uploadPath = Path.Combine(this.env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var videoFileName = $"{Guid.NewGuid().ToString()}-{Path.GetExtension(request.Video.FileName)}";
                var videoFilePath = Path.Combine(uploadPath, videoFileName);

                await using var stream = new FileStream(videoFilePath, FileMode.Create);
                await request.Video.CopyToAsync(stream);

                string audioFilePath = Path.ChangeExtension(videoFilePath, "mp3");

                FFmpeg.SetExecutablesPath("D:\\Eğitimlerim\\ffmpeg");

                var conversion = await FFmpeg.Conversions.FromSnippet.ExtractAudio(videoFilePath, audioFilePath);

                await conversion.Start(token);

                Console.WriteLine("Conversion completed.");

                Console.WriteLine(audioFilePath);
            }

            return Ok();
        }
    }

    public class VideoUploadRequest()
    {
        public IFormFile Video { get; set; }
        public string[] Languages { get; set; }
    }
}

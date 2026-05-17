using CarPark.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarPark.Controllers
{
    [ApiController]
    [Route("api/batch")]
    [AllowAnonymous]
    public class BatchController : ControllerBase
    {
        private readonly IBatchImportService _batchService;
        private readonly IWebHostEnvironment _env;

        public BatchController(IBatchImportService batchService, IWebHostEnvironment env)
        {
            _batchService = batchService;
            _env = env;
        }

        /// <summary>
        /// Trigger batch import of a CSV file.
        /// File must exist in the app's root folder.
        /// </summary>
        [HttpPost("import/{fileName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Import(string fileName)
        {
            var filePath = Path.Combine(_env.ContentRootPath, fileName);

            if (!System.IO.File.Exists(filePath))
                return BadRequest(new { message = $"File '{fileName}' not found." });

            await _batchService.ImportAsync(filePath);
            return Ok(new { message = $"Import triggered for '{fileName}'." });
        }
    }
}
using AutoMapper;
using MenuServiceAPI.Models;
using MenuServiceAPI.Models.DTO;
using MenuServiceAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MenuServiceAPI.Controllers
{
    [Route("api/MenuItem")]
    [Authorize(Roles = SD.Role_Admin)]
    [ApiController]
    public class MenuItemController : ControllerBase
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<MenuItemController> _logger;
        private readonly ApiResponse _response;

        public MenuItemController(
            IMenuRepository menuRepository,
            IMapper mapper,
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            ILogger<MenuItemController> logger)
        {
            _menuRepository = menuRepository;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
            _config = config;
            _logger = logger;
            _response = new ApiResponse();
        }

        // GET api/menuitem
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _response.Result = await _menuRepository.GetAllAsync();
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        // GET api/menuitem/{id}
        [HttpGet("{id:int}", Name = "GetMenuItem")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("Id must be greater than 0.");
                return BadRequest(_response);
            }

            var item = await _menuRepository.GetByIdAsync(id);
            if (item is null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add($"MenuItem with id {id} not found.");
                return NotFound(_response);
            }

            _response.Result = item;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        // POST api/menuitem
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] MenuItemCreateDTO createDTO)
        {
            if (!ModelState.IsValid)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            if (createDTO.File is null || createDTO.File.Length == 0)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("Image file is required.");
                return BadRequest(_response);
            }

            // Upload to ImageService → Cloudinary
            var uploadResult = await UploadToImageServiceAsync(createDTO.File);
            if (uploadResult is null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadGateway;
                _response.ErrorMessages.Add("Image upload failed. ImageService may be unavailable.");
                return StatusCode(502, _response);
            }

            var menuItem = _mapper.Map<MenuItem>(createDTO);
            menuItem.ImageUrl = uploadResult.ImageUrl;
            menuItem.ImagePublicId = uploadResult.PublicId;

            var created = await _menuRepository.CreateAsync(menuItem);

            _response.Result = created;
            _response.StatusCode = HttpStatusCode.Created;
            return CreatedAtRoute("GetMenuItem", new { id = created.Id }, _response);
        }

        // PUT api/menuitem/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromForm] MenuItemUpdateDTO updateDTO)
        {
            if (!ModelState.IsValid || updateDTO.Id != id)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("Id mismatch or invalid model state.");
                return BadRequest(_response);
            }

            var menuItem = _mapper.Map<MenuItem>(updateDTO);

            // Replace image only if a new file was provided
            if (updateDTO.File is { Length: > 0 })
            {
                // Get existing public_id so we can delete old image
                var existing = await _menuRepository.GetByIdAsync(id);
                if (existing is not null && !string.IsNullOrWhiteSpace(existing.ImagePublicId))
                    _ = DeleteFromImageServiceAsync(existing.ImagePublicId);

                var uploadResult = await UploadToImageServiceAsync(updateDTO.File);
                if (uploadResult is null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadGateway;
                    _response.ErrorMessages.Add("Image upload failed. ImageService may be unavailable.");
                    return StatusCode(502, _response);
                }

                menuItem.ImageUrl = uploadResult.ImageUrl;
                menuItem.ImagePublicId = uploadResult.PublicId;
            }

            var updated = await _menuRepository.UpdateAsync(id, menuItem);
            if (updated is null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add($"MenuItem with id {id} not found.");
                return NotFound(_response);
            }

            _response.Result = updated;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        // DELETE api/menuitem/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("Id must be greater than 0.");
                return BadRequest(_response);
            }

            // Fetch public_id before deleting DB record
            var existing = await _menuRepository.GetByIdAsync(id);
            if (existing is null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add($"MenuItem with id {id} not found.");
                return NotFound(_response);
            }

            if (!string.IsNullOrWhiteSpace(existing.ImagePublicId))
                _ = DeleteFromImageServiceAsync(existing.ImagePublicId);

            await _menuRepository.DeleteAsync(id);

            _response.StatusCode = HttpStatusCode.NoContent;
            return Ok(_response);
        }

        // ── ImageService HTTP helpers ─────────────────────────────────────────

        private async Task<ImageUploadResultDTO?> UploadToImageServiceAsync(IFormFile file)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ImageService");

                using var content = new MultipartFormDataContent();
                using var fileStream = file.OpenReadStream();
                using var fileContent = new StreamContent(fileStream);

                fileContent.Headers.ContentType =
                    new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");

                content.Add(fileContent, "file", file.FileName);

                var response = await client.PostAsync("api/image", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("ImageService returned {Status} on upload.", response.StatusCode);
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<ImageUploadResultDTO>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "ImageService unreachable during upload.");
                return null;
            }
        }

        private async Task DeleteFromImageServiceAsync(string publicId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ImageService");
                var encoded = Uri.EscapeDataString(publicId);
                await client.DeleteAsync($"api/image?publicId={encoded}");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "ImageService unreachable during delete. PublicId: {Id}", publicId);
            }
        }
    }
}
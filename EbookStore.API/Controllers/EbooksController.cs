using AutoMapper;
using EbookStore.Application.DtoModels.Ebooks;
using EbookStore.Domain.Entities;
using EbookStore.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace EBookStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EbooksController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<EbooksController> logger;
        private readonly IMapper mapper;

        public EbooksController(IUnitOfWork unitOfWork, ILogger<EbooksController> logger, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? searchTerm,
            [FromQuery] bool includeCategory = false)
        {
            try
            {
                logger.LogInformation("Getting all non-deleted ebooks.");

                var includes = new List<Expression<Func<Ebook, object>>>();
                if (includeCategory)
                {
                    includes.Add(e => e.Category!);
                }

                Expression<Func<Ebook, bool>> filter = e => e.DeletedBy == null;

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    filter = e => (e.Title.Contains(searchTerm) || e.Author.Contains(searchTerm))
                                  && e.DeletedBy == null;
                }

                var ebooks = await unitOfWork.GetRepository<Ebook>().GetAllAsync(
                    filter: filter,
                    includes: includes.ToArray()
                );

                if (!ebooks.Any())
                {
                    logger.LogWarning("No ebooks found.");
                    return NotFound(new { message = "No ebooks available." });
                }

                var ebookDtos = mapper.Map<List<EbookDto>>(ebooks);

                return Ok(ebookDtos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while getting all ebooks.");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(
            Guid id,
            [FromQuery] bool includeCategory = false)
        {
            try
            {
                logger.LogInformation("Getting non-deleted ebook with id {Id}.", id);

                var includes = new List<Expression<Func<Ebook, object>>>();
                if (includeCategory)
                {
                    includes.Add(e => e.Category!);
                }

                var ebook = await unitOfWork.GetRepository<Ebook>().GetByIdAsync(id, includes.ToArray());

                if (ebook == null || ebook.DeletedBy != null)
                {
                    logger.LogWarning("Ebook with id {Id} not found or is deleted.", id);
                    return NotFound(new { message = "Ebook not found." });
                }

                var ebookDto = mapper.Map<EbookDto>(ebook);

                return Ok(ebookDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while getting ebook with id {Id}.", id);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EbookCreateDto ebookDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var ebook = mapper.Map<Ebook>(ebookDto);
                ebook.Id = Guid.NewGuid();
                ebook.CreatedAt = DateTime.UtcNow.AddHours(4);

                await unitOfWork.GetRepository<Ebook>().AddAsync(ebook);
                await unitOfWork.CompleteAsync();

                var responseDto = mapper.Map<EbookDto>(ebook);
                return CreatedAtAction(nameof(GetById), new { id = responseDto.Id }, responseDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while creating an Ebook.");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] EbookUpdateDto ebookDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                logger.LogInformation("Updating ebook with id {Id}.", id);

                var ebook = await unitOfWork.GetRepository<Ebook>().GetByIdAsync(id);
                if (ebook == null)
                {
                    logger.LogWarning("Ebook with id {Id} not found.", id);
                    return NotFound(new { message = "Ebook not found." });
                }

                mapper.Map(ebookDto, ebook);
                ebook.UpdatedAt = DateTime.UtcNow.AddHours(4);

                await unitOfWork.GetRepository<Ebook>().UpdateAsync(ebook);
                await unitOfWork.CompleteAsync();


                logger.LogInformation("Ebook with id {Id} updated successfully.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while updating ebook with id {Id}.", id);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                logger.LogInformation("Deleting ebook with id {Id}.", id);

                var result = await unitOfWork.GetRepository<Ebook>().DeleteAsync(id);
                if (!result)
                {
                    logger.LogWarning("Ebook with id {Id} not found.", id);
                    return NotFound(new { message = "Ebook not found." });
                }

                await unitOfWork.CompleteAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while deleting ebook with id {Id}.", id);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpDelete("soft-delete/{id}")]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            try
            {
                logger.LogInformation("Soft deleting ebook with id {Id}.", id);

                var result = await unitOfWork.GetRepository<Ebook>().SoftDeleteAsync(id);
                if (!result)
                {
                    logger.LogWarning("Ebook with id {Id} not found.", id);
                    return NotFound(new { message = "Ebook not found." });
                }

                await unitOfWork.CompleteAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while soft deleting ebook with id {Id}.", id);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}
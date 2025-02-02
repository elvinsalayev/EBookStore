using AutoMapper;
using EbookStore.Application.DtoModels.Categories;
using EbookStore.Domain.Entities;
using EbookStore.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace EBookStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<CategoriesController> logger;
        private readonly IMapper mapper;

        public CategoriesController(
            IUnitOfWork unitOfWork,
            ILogger<CategoriesController> logger,
            IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? searchTerm,
            [FromQuery] bool includeEbooks = false)
        {
            try
            {
                logger.LogInformation("Getting all non-deleted categories.");

                var includes = new List<Expression<Func<Category, object>>>();
                if (includeEbooks)
                {
                    includes.Add(c => c.Ebooks!);
                }

                Expression<Func<Category, bool>> filter = c => c.DeletedBy == null;

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    filter = c => c.Name.Contains(searchTerm) && c.DeletedBy == null;
                }

                var categories = await unitOfWork.GetRepository<Category>().GetAllAsync(
                    filter: filter,
                    includes: includes.ToArray()
                );

                if (!categories.Any())
                {
                    logger.LogWarning("No categories found.");
                    return NotFound(new { message = "No categories available." });
                }

                var categoryDtos = mapper.Map<List<CategoryDto>>(categories);
                return Ok(categoryDtos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while getting all categories.");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(
            Guid id,
            [FromQuery] bool includeEbooks = false)
        {
            try
            {
                logger.LogInformation("Getting non-deleted category with id {Id}.", id);

                var includes = new List<Expression<Func<Category, object>>>();
                if (includeEbooks)
                {
                    includes.Add(c => c.Ebooks!);
                }

                var category = await unitOfWork.GetRepository<Category>().GetByIdAsync(id, includes.ToArray());

                if (category == null || category.DeletedBy != null)
                {
                    logger.LogWarning("Category with id {Id} not found or is deleted.", id);
                    return NotFound(new { message = "Category not found." });
                }

                var categoryDto = mapper.Map<CategoryDto>(category);
                return Ok(categoryDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while getting category with id {Id}.", id);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var category = mapper.Map<Category>(categoryDto);
                category.Id = Guid.NewGuid();
                category.CreatedAt = DateTime.UtcNow.AddHours(4);

                await unitOfWork.GetRepository<Category>().AddAsync(category);
                await unitOfWork.CompleteAsync();

                var responseDto = mapper.Map<CategoryDto>(category);
                return CreatedAtAction(nameof(GetById), new { id = responseDto.Id }, responseDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while creating a Category.");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CategoryUpdateDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                logger.LogInformation("Updating category with id {Id}.", id);

                var category = await unitOfWork.GetRepository<Category>().GetByIdAsync(id);
                if (category == null)
                {
                    logger.LogWarning("Category with id {Id} not found.", id);
                    return NotFound(new { message = "Category not found." });
                }

                mapper.Map(categoryDto, category);
                category.UpdatedAt = DateTime.UtcNow.AddHours(4);

                await unitOfWork.GetRepository<Category>().UpdateAsync(category);
                await unitOfWork.CompleteAsync();

                logger.LogInformation("Category with id {Id} updated successfully.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while updating category with id {Id}.", id);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                logger.LogInformation("Deleting category with id {Id}.", id);

                var result = await unitOfWork.GetRepository<Category>().DeleteAsync(id);
                if (!result)
                {
                    logger.LogWarning("Category with id {Id} not found.", id);
                    return NotFound(new { message = "Category not found." });
                }

                await unitOfWork.CompleteAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while deleting category with id {Id}.", id);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpDelete("soft-delete/{id}")]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            try
            {
                logger.LogInformation("Soft deleting category with id {Id}.", id);

                var result = await unitOfWork.GetRepository<Category>().SoftDeleteAsync(id);
                if (!result)
                {
                    logger.LogWarning("Category with id {Id} not found.", id);
                    return NotFound(new { message = "Category not found." });
                }

                await unitOfWork.CompleteAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while soft deleting category with id {Id}.", id);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}
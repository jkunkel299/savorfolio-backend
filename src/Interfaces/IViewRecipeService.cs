using Newtonsoft.Json.Linq;
using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.Interfaces;

public interface IViewRecipeService
{
    Task<FullRecipeDTO> CompileRecipeAsync(int recipeId);
}
using Newtonsoft.Json.Linq;
using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.Interfaces;

public interface IAddRecipeService
{
    Task<OperationResult<int>> AddRecipeManuallyAsync(JObject newRecipeContent, int userId);
}

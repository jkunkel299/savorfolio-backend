// using Microsoft.AspNetCore.Mvc;
// using savorfolio_backend.Interfaces;
// using savorfolio_backend.Models.DTOs;

// namespace savorfolio_backend.API;

// public static class AuthEndpoints
// {
//     public static void MapAuthEndpoints(this WebApplication app)
//     {
//         var group = app.MapGroup("/api/auth");

//         group.MapPost(
//             "/register",
//             async ([FromBody] UserRegisterDTO userBody, IAuthManager authManager) =>
//             {
//                 var result = await authManager.RegisterUserAsync(userBody);
//                 if (result.Success == true)
//                 {
//                     return Results.Ok(result.Message);
//                 }
//                 else
//                 {
//                     return Results.Problem(result.Message);
//                 }
//             }
//         );

//         group.MapPost(
//             "/login",
//             async ([FromBody] UserLoginDTO userBody, IAuthManager authManager) =>
//             {
//                 var result = await authManager.LoginUserAsync(userBody);
//                 if (result.Success == true)
//                 {
//                     return result.Data;
//                 }
//                 else
//                 {
//                     return result.Data;
//                 }
//             }
//         );
//     }
// }

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using savorfolio_backend.Interfaces;
using savorfolio_backend.Models.DTOs;

namespace savorfolio_backend.API;

public static class AuthEndpoints
{
    public static void MapRegister(this WebApplication app)
    {
        app.MapPost(
            "/api/auth/register",
            async ([FromBody] UserRegisterDTO userBody, IAuthManager authManager) =>
            {
                var result = await authManager.RegisterUserAsync(userBody);
                if (result.Success == true)
                {
                    return Results.Ok(result.Message);
                }
                else
                {
                    return Results.BadRequest(result.Message);
                }
            }
        );
    }

    public static void MapLogIn(this WebApplication app)
    {
        app.MapPost(
            "/api/auth/login",
            async (
                HttpContext context,
                [FromBody] UserLoginDTO userBody,
                IAuthManager authManager
            ) =>
            {
                var result = await authManager.LoginUserAsync(userBody, context);
                return result.Data;
            }
        );
    }

    public static void MapLogOut(this WebApplication app)
    {
        app.MapPost(
            "/api/auth/logout",
            async (HttpContext context) =>
            {
                context.Response.Cookies.Delete("auth_token");
                await context.SignOutAsync();
                return Results.Ok("logged out");
            }
        );
    }

    public static void MapFetchUser(this WebApplication app)
    {
        app.MapGet(
                "/api/auth/me",
                (HttpContext context) =>
                {
                    if (context.User.Identity?.IsAuthenticated == true)
                    {
                        return Results.Ok(
                            new
                            {
                                id = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                                email = context.User.FindFirst(ClaimTypes.Email)?.Value,
                            }
                        );
                    }

                    return Results.Unauthorized();
                }
            )
            .RequireAuthorization("cookies");
    }
}

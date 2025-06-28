using OneApp_minimalApi.Contracts;
using OneApp_minimalApi.Contracts.Identity;
using OneApp_minimalApi.Models.Identity;

namespace OneApp_minimalApi.Interfaces;

public interface IIdentityService
{
    Task<ApiResponse<string>> Signup(SignupModelDto model);
    Task<ApiResponse<string>> Login(LoginModelDto model);
    Task<ApiResponse<string>> RefreshToken(TokenModelDto model);
    Task<ApiResponse<string>> RevokeToken(string username);
}
using OneApp_minimalApi.Contracts;
using OneApp_minimalApi.Contracts.Identity;

namespace OneApp_minimalApi.Interfaces;

public interface IIdentityService
{
    Task<ApiResponse<string>> Signup(SignupModelDto model);
    Task<ApiResponse<string>> Login(LoginModelDto model);
}
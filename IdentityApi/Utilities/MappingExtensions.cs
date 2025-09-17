using IdentityApi.Commands;

using LoginRequest    = IdentityApi.Requests.LoginRequest;
using RegisterRequest = IdentityApi.Requests.RegisterRequest;

namespace IdentityApi.Mapping;

internal static class MappingExtensions
{
    public static LoginCommand ToCommand(this LoginRequest request) 
        => new(request.Email, request.Password);
    
    public static RegisterCommand ToCommand(this RegisterRequest request)
        => new(request.Email, request.Username, request.Password);
}
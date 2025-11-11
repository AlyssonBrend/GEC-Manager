using GECManager.Shared.Models;

namespace GECManager.Api.Services;

public interface ITokenService
{
    string CreateToken(User user);
}
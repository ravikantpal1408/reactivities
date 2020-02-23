using Domain;

namespace Application.Interfaces
{
    public interface IJwtGenerator
    {
        // get JWT token back
        string CreateToken(AppUser user);
    }
}
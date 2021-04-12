using CoreLibrary.Domain;

namespace CoreLibrary.Web.Interfaces
{
    public interface IJwtGenerator
    {
        string CreateToken(ApplicationUser user);
    }
}
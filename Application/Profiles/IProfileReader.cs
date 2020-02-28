using System.Threading.Tasks;

namespace Application.Profiles
{
    public interface IProfileReader
    {
        Task<Profiles> ReadProfile(string username);
    }
}
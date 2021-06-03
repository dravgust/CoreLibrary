using System.Threading.Tasks;
using CoreLibrary.CQRS;

namespace CorLibrary.Test.Stubs
{
    public class SimpleAsyncQuery : IAsyncQuery<string, string>
    {
        public async Task<string> Ask(string spec)
        {
            return await Task.FromResult(spec);
        }
    }
}

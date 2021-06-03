using CoreLibrary.CQRS;

namespace CorLibrary.Test.Stubs
{
    public class SimpleQuery : IQuery<string, string>
    {
        public string Ask(string spec)
        {
            return spec;
        }
    }
}

using CoreLibrary.CQRS;

namespace CorLibrary.Test.Stubs
{
    public class SimpleCommandHandler : ICommandHandler<string, string>
    {
        public string Handle(string input)
        {
            return input;
        }
    }

    public class SimpleCommandHandler2 : ICommandHandler<string>
    {
        public void Handle(string input)
        {
        }
    }
}

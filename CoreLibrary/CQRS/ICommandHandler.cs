using System.Threading.Tasks;

namespace CoreLibrary.CQRS
{
    public interface ICommandHandler<in TInput>
    {
        void Handle(TInput input);
    }

    public interface ICommandHandler<in TInput, out TOutput>
    {
        TOutput Handle(TInput input);
    }

    public interface IAsyncCommandHandler<in TInput>
        : ICommandHandler<TInput, Task>
    {
    }

    public interface IAsyncCommandHandler<in TInput, TOutput>
        : ICommandHandler<TInput, Task<TOutput>>
    {
    }
}

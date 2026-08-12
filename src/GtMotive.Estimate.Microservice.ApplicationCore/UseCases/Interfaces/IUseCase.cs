using System.Threading.Tasks;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Interfaces
{
    /// <summary>
    /// Interface for the handler of an use case.
    /// </summary>
    /// <typeparam name="TUseCaseInput">Tyoe of the input message.</typeparam>
    public interface IUseCase<in TUseCaseInput>
        where TUseCaseInput : IUseCaseInput
    {
        /// <summary>
        /// Executes the Use Case.
        /// </summary>
        /// <param name="input">Input Message.</param>
        /// <returns>Task.</returns>
        Task Execute(TUseCaseInput input);
    }

    /// <summary>
    /// Defines a use case that does not require an input message.
    /// </summary>
    public interface IUseCase
    {
        /// <summary>
        /// Executes the use case.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous execution of the use case.
        /// </returns>
        Task Execute();
    }
}

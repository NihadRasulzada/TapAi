using TapAi.Shared.Application.ResponseObject.Concreate;

namespace TapAi.Shared.Application.Abstraction;

public interface ICommand<TResponse> where TResponse : Response { }
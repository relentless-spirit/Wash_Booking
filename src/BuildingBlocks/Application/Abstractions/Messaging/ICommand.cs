using System.Diagnostics.CodeAnalysis;
using BuildingBlocks.Domain;
using MediatR;

namespace BuildingBlocks.Application.Abstractions.Messaging;

[SuppressMessage("Design", "CA1040:Avoid empty interfaces", Justification = "Marker interface used for generic constraints.")]
public interface ICommand : IRequest<Result>, IBaseCommand;

[SuppressMessage("Design", "CA1040:Avoid empty interfaces", Justification = "Marker interface used for generic constraints.")]
public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand
    where TResponse : notnull
;

[SuppressMessage("Design", "CA1040:Avoid empty interfaces", Justification = "Marker interface used for generic constraints.")]
public interface IBaseCommand;
using System.Diagnostics.CodeAnalysis;
using BuildingBlocks.Domain;
using MediatR;

namespace BuildingBlocks.Application.Abstractions.Messaging;

[SuppressMessage("Design", "CA1040:Avoid empty interfaces", Justification = "Marker interface used for generic constraints.")]
public interface IQuery<TResponse> : IRequest<Result<TResponse>> 
    where TResponse : notnull
;
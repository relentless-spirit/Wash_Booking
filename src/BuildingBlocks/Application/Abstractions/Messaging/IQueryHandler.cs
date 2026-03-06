using BuildingBlocks.Domain;
using MediatR;

namespace BuildingBlocks.Application.Abstractions.Messaging;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
    where TResponse : notnull;
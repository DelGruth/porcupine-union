using Microsoft.Extensions.Caching.Hybrid;
using UserAccessSystem.Contract;
using UserAccessSystem.Contract.Dtos;
using UserAccessSystem.Domain.Common;
using UserAccessSystem.Internal.Application.Peristence;

namespace UserAccessSystem.Internal.Infrastructure.Service;

public abstract class BaseService<TEntity, TResponseDto>(
    HybridCache cache,
    IRepository<TEntity> repository
)
    where TEntity : BaseDomainObj
    where TResponseDto : class
{
    private readonly TimeSpan DefaultCacheExpiration = TimeSpan.FromMilliseconds(150);

    protected abstract TResponseDto MapToDto(TEntity entity);

    public virtual async Task<Response<IEnumerable<TResponseDto>>> GetAllAsync(
        CancellationToken ctx = default
    )
    {
        var cacheKey = $"GetAll_{typeof(TEntity).Name}";
        return await cache.GetOrCreateAsync<Response<IEnumerable<TResponseDto>>>(
            cacheKey,
            async ctx =>
            {
                var response = await repository.GetAllAsync();
                if (!response.Success)
                    return new Response<IEnumerable<TResponseDto>>(
                        response.ErrorCode,
                        response.Message
                    );

                var dtos = response.Data?.Select(MapToDto) ?? Enumerable.Empty<TResponseDto>();
                return new Response<IEnumerable<TResponseDto>>(dtos);
            },
            options: new HybridCacheEntryOptions { Expiration = DefaultCacheExpiration },
            cancellationToken: ctx
        );
    }

    public virtual async Task<Response<TResponseDto>> GetByIdAsync(
        Guid id,
        CancellationToken ctx = default
    )
    {
        var cacheKey = $"GetById_{typeof(TEntity).Name}_{id}";
        return await cache.GetOrCreateAsync<Response<TResponseDto>>(
            cacheKey,
            async ctx =>
            {
                var response = await repository.GetByIdAsync(id);
                return !response.Success
                    ? new Response<TResponseDto>(response.ErrorCode, response.Message)
                    : new Response<TResponseDto>(MapToDto(response.Data));
            },
            options: new HybridCacheEntryOptions { Expiration = DefaultCacheExpiration },
            cancellationToken: ctx
        );
    }

    public virtual async Task<Response<bool>> DeleteAsync(Guid id, CancellationToken ctx = default)
    {
        var entity = await repository.GetByIdAsync(id, ctx);
        if (!entity.Success)
            return new Response<bool>(entity.ErrorCode, entity.Message);

        return await repository.DeleteAsync(entity.Data, ctx);
    }
}

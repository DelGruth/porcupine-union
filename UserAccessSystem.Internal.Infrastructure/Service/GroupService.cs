using Microsoft.Extensions.Caching.Hybrid;
using UserAccessSystem.Contract;
using UserAccessSystem.Contract.Dtos;
using UserAccessSystem.Contract.Requests;
using UserAccessSystem.Contract.Responses;
using UserAccessSystem.Domain.Group;
using UserAccessSystem.Internal.Application.Infrastructure;
using UserAccessSystem.Internal.Application.Peristence;

namespace UserAccessSystem.Internal.Infrastructure.Service;

public class GroupService(HybridCache cache, IGroupRepository groupRepository)
    : BaseService<Group, GroupDto>(cache, groupRepository),
        IGroupService
{
    protected override GroupDto MapToDto(Group entity) => new(entity);

    public async Task<Response<GroupListResponse>> GetAllGroupsAsync(
        CancellationToken ctx = default
    )
    {
        var result = await groupRepository.GetAllAsync(ctx);
        return !result.Success
            ? new Response<GroupListResponse>(result.ErrorCode, result.Message)
            : new Response<GroupListResponse>(
                new GroupListResponse { Groups = result.Data.Select(MapToDto) }
            );
    }

    public override async Task<Response<GroupDto>> GetByIdAsync(
        Guid id,
        CancellationToken ctx = default
    )
    {
        var result = await groupRepository.GetByIdAsync(id, ctx);
        return !result.Success
            ? new Response<GroupDto>(result.ErrorCode, result.Message)
            : new Response<GroupDto>(MapToDto(result.Data));
    }

    public async Task<Response<GroupResponse>> GetGroupByIdAsync(
        Guid id,
        CancellationToken ctx = default
    )
    {
        var result = await GetByIdAsync(id, ctx);
        return !result.Success
            ? new Response<GroupResponse>(result.ErrorCode, result.Message)
            : new Response<GroupResponse>(new GroupResponse { Group = result.Data });
    }

    public async Task<Response<GroupResponse>> CreateAsync(
        CreateGroupRequest request,
        CancellationToken ctx = default
    )
    {
        var group = new Group { Name = request.Name, Description = request.Description };

        var result = await groupRepository.AddAsync(group, ctx);
        return !result.Success
            ? new Response<GroupResponse>(result.ErrorCode, result.Message)
            : new Response<GroupResponse>(new GroupResponse { Group = MapToDto(result.Data) });
    }

    public async Task<Response<bool>> UpdateAsync(
        UpdateGroupRequest request,
        CancellationToken ctx = default
    )
    {
        var getResult = await groupRepository.GetByIdAsync(request.Id, ctx);
        if (!getResult.Success)
            return new Response<bool>(getResult.ErrorCode, getResult.Message);

        var group = getResult.Data;
        group.Name = request.Name;
        group.Description = request.Description;

        return await groupRepository.UpdateAsync(group, ctx);
    }

    public async Task<Response<bool>> AddUserToGroupAsync(
        Guid userId,
        Guid groupId,
        CancellationToken ctx = default
    )
    {
        var result = await groupRepository.AddUserToGroupAsync(userId, groupId, ctx);
        if (result.Success)
        {
            await InvalidateGroupCache(groupId);
        }
        return result;
    }

    public async Task<Response<bool>> RemoveUserFromGroupAsync(
        Guid userId,
        Guid groupId,
        CancellationToken ctx = default
    )
    {
        var result = await groupRepository.RemoveUserFromGroupAsync(userId, groupId, ctx);
        if (result.Success)
        {
            await InvalidateGroupCache(groupId);
        }
        return result;
    }

    public async Task<Response<bool>> AddPermissionToGroupAsync(
        Guid permissionId,
        Guid groupId,
        CancellationToken ctx = default
    )
    {
        var result = await groupRepository.AddPermissionToGroupAsync(permissionId, groupId, ctx);
        if (result.Success)
        {
            await InvalidateGroupCache(groupId);
        }
        return result;
    }

    public async Task<Response<bool>> RemovePermissionFromGroupAsync(
        Guid permissionId,
        Guid groupId,
        CancellationToken ctx = default
    )
    {
        var result = await groupRepository.RemovePermissionFromGroupAsync(
            permissionId,
            groupId,
            ctx
        );
        if (result.Success)
        {
            await InvalidateGroupCache(groupId);
        }
        return result;
    }

    public async Task<Response<GroupUserDistributionResponse>> GetUsersPerGroupCountAsync(
        CancellationToken ctx = default
    )
    {
        var result = await groupRepository.GetAllAsync(ctx);
        if (!result.Success)
            return new Response<GroupUserDistributionResponse>(result.ErrorCode, result.Message);

        var distribution = result.Data.ToDictionary(g => g.Name, g => g.Users.Count());
        return new Response<GroupUserDistributionResponse>(
            new GroupUserDistributionResponse { Distribution = distribution }
        );
    }

    private async Task InvalidateGroupCache(Guid groupId)
    {
        var cacheKey = $"GetById_Group_{groupId}";
        await cache.RemoveAsync(cacheKey);
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace UserAccessSystem.Internal.Infrastructure.Middleware;

public class AuthAttributeFilter(
    string requiredGroup = "SU",
    bool requireRead = true,
    bool requireWrite = false
) : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var userGroupQuery = new
        {
            GroupName = requiredGroup,
            UserId = "Jwt.name",
            Read = true,
            Write = true,
            UserGroupPermissions = new[]
            {
                new
                {
                    GroupName = requiredGroup,
                    Read = true,
                    Write = true,
                },
            },
        };

        var hasGroupAccess =
            userGroupQuery.GroupName.Equals(requiredGroup)
            || userGroupQuery.UserGroupPermissions.Any(p => p.GroupName.Equals(requiredGroup));

        if (!hasGroupAccess)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var hasReadAccess =
            !requireRead
            || userGroupQuery.Read && userGroupQuery.GroupName.Equals(requiredGroup)
            || userGroupQuery.UserGroupPermissions.Any(p =>
                p.Read && p.GroupName.Equals(requiredGroup)
            );

        var hasWriteAccess =
            !requireWrite
            || userGroupQuery.Write && userGroupQuery.GroupName.Equals(requiredGroup)
            || userGroupQuery.UserGroupPermissions.Any(p =>
                p.Write && p.GroupName.Equals(requiredGroup)
            );

        if ((!requireRead || hasReadAccess) && (!requireWrite || hasWriteAccess))
            return;
        context.Result = new UnauthorizedResult();
    }
}

public class AuthAttribute : TypeFilterAttribute
{
    public AuthAttribute(
        string requiredGroup = "SU",
        bool requireRead = true,
        bool requireWrite = false
    )
        : base(typeof(AuthAttributeFilter))
    {
        Arguments = [requiredGroup, requireRead, requireWrite];
    }
}

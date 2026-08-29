using Microsoft.AspNetCore.Authorization;

namespace ClinicFlow.Application.Common.Authorization
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,PermissionRequirement requirement)
        {

            var permissionClaim = context.User.FindFirst("Permissions");

            if (permissionClaim == null)
                return Task.CompletedTask;


            var userPermissions = long.Parse(permissionClaim.Value);


            if ((userPermissions & (long)requirement.Permission) == (long)requirement.Permission)
            {
                context.Succeed(requirement);
            }


            return Task.CompletedTask;
        }

    }
}

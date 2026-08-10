using ClinicFlow.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace ClinicFlow.Application.Common.Authorization
{
    public class PermissionRequirement : IAuthorizationRequirement
    {

        public PermissionEnum Permission { get; }

        public PermissionRequirement(PermissionEnum permission)
        {
            Permission = permission;
        }

    }
}

using ClinicFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicFlow.Application.Features.Clinics.DTOs.Requests
{
    public sealed record CreateClinicDtoRequest
    {

        public string Name { get; set; } = null!;

        public string? LogoUrl { get; set; }

        public string Phone { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Address { get; set; } = null!;

        public string? Description { get; set; }


    }
}

using ClinicFlow.Application.Features.Specialties.DTOs.Requests;
using ClinicFlow.Domain.Entities;

namespace ClinicFlow.UnitTests.Common.Builders
{
    public static class SpecialityBuilder
    {
        public static Specialty CreateSpecialtyEntity(int id = 1, string name = "Cardiology", bool isActive = true)
        {
            return new Specialty
            {
                Id = id,
                Name = name,
                IsActive = isActive
            };
        }

        public static GetAllSpecialityDtoRequest CreateGetAllSpecialityDtoRequest(int id = 1, string name = "Cardiology", bool isActive = true)
        {
            return new GetAllSpecialityDtoRequest
            {
                Id = id,
                Name = name,
                IsActive = isActive
            };
        }
    }
}

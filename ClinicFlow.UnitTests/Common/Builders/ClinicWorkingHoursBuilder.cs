using ClinicFlow.Application.Features.ClinicWorkingHours.DTOs.Requests;
using ClinicFlow.Domain.Entities;

namespace ClinicFlow.UnitTests.Common.Builders
{
    public static class ClinicWorkingHoursBuilder
    {
        public static List<CreateClinicWorkingHourDtoRequest> CreateRequestList()
        {
            return new List<CreateClinicWorkingHourDtoRequest>
            {
                new CreateClinicWorkingHourDtoRequest
                {
                    Day = DayOfWeek.Sunday,
                    OpenTime = new TimeOnly(9, 0),
                    CloseTime = new TimeOnly(17, 0),
                    IsClosed = true
                },
                new CreateClinicWorkingHourDtoRequest
                {
                   Day = DayOfWeek.Monday,
                   OpenTime = new TimeOnly(9, 0),
                   CloseTime = new TimeOnly(17, 0),
                   IsClosed = true
                }
            };
        }

        public static List<ClinicWorkingHour> CreateEntitiesList(int clinicId)
        {
            return new List<ClinicWorkingHour>
            {
                new ClinicWorkingHour
                {
                    Id = 1,
                    ClinicId = clinicId,
                    Day = DayOfWeek.Sunday,
                    OpenTime = new TimeOnly(9, 0),
                    CloseTime = new TimeOnly(17, 0),
                    IsClosed = true
                },
                new ClinicWorkingHour
                {
                    Id = 2,
                    ClinicId = clinicId,
                    Day = DayOfWeek.Monday,
                    OpenTime = new TimeOnly(9, 0),
                    CloseTime = new TimeOnly(17, 0),
                    IsClosed = true
                }
            };
        }
    }
}
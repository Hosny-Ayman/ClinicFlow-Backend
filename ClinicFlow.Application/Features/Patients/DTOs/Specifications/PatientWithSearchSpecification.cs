using ClinicFlow.Application.Common.Specifications;
using ClinicFlow.Application.Features.Patients.DTOs.Requests;
using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Application.Features.Patients.DTOs.Specifications
{
    public class PatientWithSearchSpecification : BaseSpecification<Patient>
    {
        public PatientWithSearchSpecification(PatientSearchDtoRequest request, int clinicId)
        {
            AddCriteria(p => p.ClinicPatients.Any(cp => cp.ClinicId == clinicId));

            if (!string.IsNullOrWhiteSpace(request.FullNameSearch))
            {
                var search = request.FullNameSearch.Trim();
                AddCriteria(p =>
                    p.Person.FirstName.Contains(search) ||
                    p.Person.LastName.Contains(search) ||
                    (p.Person.FirstName + " " + p.Person.LastName).Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.PhoneNumberSearch))
            {
                AddCriteria(p => p.Person.PhoneNumber != null && p.Person.PhoneNumber.Contains(request.PhoneNumberSearch));
            }

            if (!string.IsNullOrWhiteSpace(request.NationalIdSearch))
            {
                AddCriteria(p => p.NationalId != null && p.NationalId.Contains(request.NationalIdSearch));
            }

            if (request.Gender.HasValue)
            {
                AddCriteria(p => p.Gender == request.Gender);
            }

            if (request.BloodType.HasValue)
            {
                AddCriteria(p => p.BloodType == request.BloodType);
            }

            if (!string.IsNullOrEmpty(request.SortField))
            {
                bool isDescending = request.SortOrder == -1;

                switch (request.SortField.ToLower())
                {
                    case "fullname":
                        if (isDescending) AddOrderByDescending(p => p.Person.FirstName);
                        else AddOrderBy(p => p.Person.FirstName);
                        break;

                    case "dateofbirth":
                        if (isDescending) AddOrderByDescending(p => p.DateOfBirth);
                        else AddOrderBy(p => p.DateOfBirth);
                        break;

                    case "gender":
                        if (isDescending) AddOrderByDescending(p => p.Gender);
                        else AddOrderBy(p => p.Gender);
                        break;

                    case "createdat":
                        if (isDescending) AddOrderByDescending(p => p.CreatedAt);
                        else AddOrderBy(p => p.CreatedAt);
                        break;

                    case "status":
                        if (isDescending) AddOrderByDescending(p => p.ClinicPatients.Any(cp => cp.ClinicId == clinicId && cp.IsActive));
                        else AddOrderBy(p => p.ClinicPatients.Any(cp => cp.ClinicId == clinicId && cp.IsActive));
                        break;

                    default:
                        AddOrderByDescending(p => p.Id);
                        break;
                }
            }
            else
            {
                AddOrderByDescending(p => p.Id);
            }
        }
    }
}
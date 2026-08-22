using ClinicFlow.Application.Common.Specifications;
using ClinicFlow.Application.Features.DoctorVacations.DTOs.Requests;
using ClinicFlow.Domain.Entities; 

namespace ClinicFlow.Application.Features.DoctorVacations.DTOs.Specifications
{
    public class DoctorVacationWithSearchSpecification : BaseSpecification<DoctorVacation>
    {
        public DoctorVacationWithSearchSpecification(DoctorVacationSearchDtoRequest request, int clinicId)
        {
            AddCriteria(v => v.Doctor.ClinicId == clinicId);

            if (!string.IsNullOrWhiteSpace(request.FullNameSearch))
            {
                var search = request.FullNameSearch.Trim();
                AddCriteria(v =>
                    v.Doctor.User.Person.FirstName.Contains(search) ||
                    v.Doctor.User.Person.LastName.Contains(search) ||
                    (v.Doctor.User.Person.FirstName + " " + v.Doctor.User.Person.LastName).Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.EmailSearch))
            {
                AddCriteria(v => v.Doctor.User.Person.Email != null && v.Doctor.User.Person.Email.Contains(request.EmailSearch));
            }

            if (!string.IsNullOrWhiteSpace(request.PhoneNumberSearch))
            {
                AddCriteria(v => v.Doctor.User.Person.PhoneNumber != null && v.Doctor.User.Person.PhoneNumber.Contains(request.PhoneNumberSearch));
            }

            if (request.Gender.HasValue)
            {
                AddCriteria(v => v.Doctor.Gender == request.Gender);
            }

            if (request.SpecialtyId.HasValue)
            {
                AddCriteria(v => v.Doctor.SpecialtyId == request.SpecialtyId);
            }

            if (request.From.HasValue)
            {
                AddCriteria(v => v.StartDate >= request.From.Value);
            }

            if (request.To.HasValue)
            {
                AddCriteria(v => v.EndDate <= request.To.Value);
            }

            if (request.Status.HasValue)
            {
                AddCriteria(v => v.Status == request.Status);
            }

            if (!string.IsNullOrEmpty(request.SortField))
            {
                bool isDescending = request.SortOrder == -1;

                switch (request.SortField.ToLower())
                {
                    case "fullname":
                        if (isDescending)
                            AddOrderByDescending(v => v.Doctor.User.Person.FirstName);
                        else
                            AddOrderBy(v => v.Doctor.User.Person.FirstName);
                        break;

                    case "email":
                        if (isDescending)
                            AddOrderByDescending(v => v.Doctor.User.Person.Email);
                        else
                            AddOrderBy(v => v.Doctor.User.Person.Email);
                        break;

                    case "phonenumber":
                        if (isDescending)
                            AddOrderByDescending(v => v.Doctor.User.Person.PhoneNumber);
                        else
                            AddOrderBy(v => v.Doctor.User.Person.PhoneNumber);
                        break;

                    case "specialty":
                        if (isDescending)
                            AddOrderByDescending(v => v.Doctor.Specialty.Name);
                        else
                            AddOrderBy(v => v.Doctor.Specialty.Name);
                        break;

                    case "experience":
                        if (isDescending)
                            AddOrderByDescending(v => v.Doctor.ExperienceYears);
                        else
                            AddOrderBy(v => v.Doctor.ExperienceYears);
                        break;

                    case "status":
                        if (isDescending)
                            AddOrderByDescending(v => v.Status);
                        else
                            AddOrderBy(v => v.Status);
                        break;

                    default:
                        AddOrderByDescending(v => v.Id);
                        break;
                }
            }
            else
            {
                AddOrderByDescending(v => v.Id);
            }
        }
    }
}
using ClinicFlow.Application.Common.Specifications;
using ClinicFlow.Application.Features.Doctors.DTOs.Requests;
using ClinicFlow.Domain.Entities;

namespace ClinicFlow.Application.Features.Doctors.DTOs.Specifications
{
    public class DoctorsWithSearchSpecification : BaseSpecification<Doctor>
    {
        public DoctorsWithSearchSpecification(DoctorSearchDtoRequest request, int clinicId)
        {
            AddCriteria(x => x.ClinicId == clinicId);

            if (!string.IsNullOrWhiteSpace(request.FullNameSearch))
            {
                var search = request.FullNameSearch.Trim();
                AddCriteria(x =>
                    x.User.Person.FirstName.Contains(search) ||
                    x.User.Person.LastName.Contains(search) ||
                    (x.User.Person.FirstName + " " + x.User.Person.LastName).Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.EmailSearch))
            {
                AddCriteria(x => x.User.Person.Email != null && x.User.Person.Email.Contains(request.EmailSearch));
            }

            if (!string.IsNullOrWhiteSpace(request.PhoneNumberSearch))
            {
                AddCriteria(x => x.User.Person.PhoneNumber != null && x.User.Person.PhoneNumber.Contains(request.PhoneNumberSearch));
            }

            if (request.Gender.HasValue)
            {
                AddCriteria(x => x.Gender == request.Gender);
            }

            if (request.SpecialtyId.HasValue)
            {
                AddCriteria(x => x.SpecialtyId == request.SpecialtyId);
            }

            if (!string.IsNullOrEmpty(request.SortField))
            {
                bool isDescending = request.SortOrder == -1;

                switch (request.SortField.ToLower())
                {
                    case "fullname":
                        if (isDescending)
                            AddOrderByDescending(x => x.User.Person.FirstName);
                        else
                            AddOrderBy(x => x.User.Person.FirstName);
                        break;

                    case "email":
                        if (isDescending)
                            AddOrderByDescending(x => x.User.Person.Email);
                        else
                            AddOrderBy(x => x.User.Person.Email);
                        break;

                    case "phonenumber":
                        if (isDescending)
                            AddOrderByDescending(x => x.User.Person.PhoneNumber);
                        else
                            AddOrderBy(x => x.User.Person.PhoneNumber);
                        break;

                    case "specialty":
                        if (isDescending)
                            AddOrderByDescending(x => x.Specialty.Name);
                        else
                            AddOrderBy(x => x.Specialty.Name);
                        break;

                    case "experience":
                        if (isDescending)
                            AddOrderByDescending(x => x.ExperienceYears);
                        else
                            AddOrderBy(x => x.ExperienceYears);
                        break;

                    case "status":
                        if (isDescending)
                            AddOrderByDescending(x => x.User.IsActive);
                        else
                            AddOrderBy(x => x.User.IsActive);
                        break;

                    default:
                        AddOrderByDescending(x => x.Id);
                        break;
                }
            }
            else
            {
                AddOrderByDescending(x => x.Id);
            }
        }
    }
}
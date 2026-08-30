using ClinicFlow.Application.Common.Specifications;
using ClinicFlow.Application.Features.Appointments.DTOs.Requests;
using ClinicFlow.Domain.Entities;
using System.Text.RegularExpressions;

namespace ClinicFlow.Application.Features.Appointments.DTOs.Specifications
{
    public class AppointmentWithSearchSpecification: BaseSpecification<Appointment>
    {

        public AppointmentWithSearchSpecification(AppointmentSearchDtoRequest request, int clinicId)
        {
            AddCriteria(x => x.ClinicId == clinicId);

            if(!string.IsNullOrWhiteSpace(request.FullNameOrPhoneNumberSearch))
            {
                var search = request.FullNameOrPhoneNumberSearch.Trim();

                if(Regex.IsMatch(request.FullNameOrPhoneNumberSearch, @"^01[0125][0-9]{8}$"))
                {
                    AddCriteria(x => x.Patient.Person.PhoneNumber != null && x.Patient.Person.PhoneNumber.Contains(search));
                }
                else
                {
                    AddCriteria(x =>
                   x.Patient.Person.FirstName.Contains(search) ||
                   x.Patient.Person.LastName.Contains(search) ||
                   (x.Patient.Person.FirstName + " " + x.Patient.Person.LastName).Contains(search));

                }
              
            }

            if (request.StatusSearch.HasValue)
            {
                AddCriteria(x => x.Status == request.StatusSearch);
            }

            if (request.DoctorIdSearch.HasValue)
            {
                AddCriteria(x => x.DoctorId == request.DoctorIdSearch);
            }

            if (request.DateSearch.HasValue)
            {
                AddCriteria(x => x.AppointmentDate == request.DateSearch);
            }

            if (!string.IsNullOrEmpty(request.SortField))
            {
                bool isDescending = request.SortOrder == -1;

                switch (request.SortField.ToLower())
                {
                    case "time":
                        if (isDescending)
                            AddOrderByDescending(x => x.StartTime);
                        else
                            AddOrderBy(x => x.StartTime);
                        break;
                    case "patientName":
                        if (isDescending)
                            AddOrderByDescending(x => x.Patient.Person.FirstName);
                        else
                            AddOrderBy(x => x.Patient.Person.FirstName);
                        break;

                    case "doctorName":
                        if (isDescending)
                            AddOrderByDescending(x => x.Doctor.User.Person.FirstName);
                        else
                            AddOrderBy(x => x.Doctor.User.Person.FirstName);
                        break;

                    case "status":
                        if (isDescending)
                            AddOrderByDescending(x => x.Status);
                        else
                            AddOrderBy(x => x.Status);
                        break;

                    case "consultationFee":
                        if (isDescending)
                            AddOrderByDescending(x => x.Doctor.ConsultationFee);
                        else
                            AddOrderBy(x => x.Doctor.ConsultationFee);
                        break;

                    case "paymentstatus":
                        if (isDescending)
                            AddOrderByDescending(x => x.Invoice.Status);
                        else
                            AddOrderBy(x => x.Invoice.Status);
                        break;

                    default:
                        AddOrderByDescending(x => x.Id);
                        break;
                }
            }
            else
            {
                AddOrderByDescending(x => x.AppointmentDate);
            }
        }

    }
}

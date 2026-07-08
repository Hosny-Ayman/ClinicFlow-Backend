using AutoMapper;
using ClinicFlow.Application.Common.Errors;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Features.Authentication.DTOs.Requests;
using ClinicFlow.Application.Features.Authentication.DTOs.Responses;
using ClinicFlow.Domain.InterFaces;
using Microsoft.Extensions.Logging;

namespace ClinicFlow.Application.Features.Authentication
{
    public class AuthenticationService
    {
        public readonly IUserRepository _userRepository;
        public readonly IMapper _mapper;
        public readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(IUserRepository userRepository, IMapper mapper, ILogger<AuthenticationService> Logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = Logger;
        }

        public async Task<OperationResult<LoginDtoResponse>> LoginAsync(LoginDtoRequest request)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);


            if (user == null || BCrypt.Net.BCrypt.Verify(request.Password,user.PasswordHash))
            {
                _logger.LogWarning("Login Faild With Email: {(Email)}", request.Email);
                return OperationResult<LoginDtoResponse>.Failure(new List<Error> { (GeneralErrors.BadRequst("Email or Password Wrong"))});
            }

            var response = _mapper.Map<LoginDtoResponse>(user);

            return OperationResult<LoginDtoResponse>.Success(response);

        }


    }
}

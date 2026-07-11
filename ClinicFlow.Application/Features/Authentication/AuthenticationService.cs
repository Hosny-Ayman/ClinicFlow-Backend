using AutoMapper;
using ClinicFlow.Application.Common.Authentication;
using ClinicFlow.Application.Common.Errors;
using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Features.Authentication.DTOs;
using ClinicFlow.Application.Features.Authentication.DTOs.Requests;
using ClinicFlow.Application.Features.Authentication.DTOs.Responses;
using ClinicFlow.Domain.Entities;
using ClinicFlow.Domain.InterFaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace ClinicFlow.Application.Features.Authentication
{
    public class AuthenticationService
    {
        public readonly IUserRepository _userRepository;
        public readonly IMapper _mapper;
        public readonly ILogger<AuthenticationService> _logger;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtProvider _jwtProvider;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly IRefreshTokenHasher _refreshTokenHasher;
        private readonly JwtSettings _jwtSettings;
        private readonly IUnitOfWork _unitOfWork;

        public AuthenticationService(IUserRepository userRepository, IMapper mapper, ILogger<AuthenticationService> Logger
            , IRefreshTokenRepository refreshTokenRepository,IJwtProvider jwtProvider,
            IRefreshTokenGenerator refreshTokenGenerator, IRefreshTokenHasher refreshTokenHasher
            , IOptions<JwtSettings> jwtOptions, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = Logger;
            _refreshTokenRepository = refreshTokenRepository;
            _jwtProvider = jwtProvider;
            _refreshTokenHasher = refreshTokenHasher;
            _refreshTokenGenerator = refreshTokenGenerator;
            _jwtSettings = jwtOptions.Value;
            _unitOfWork = unitOfWork;
        }

        public async Task<OperationResult<AuthenticationResultDto>> LoginAsync(LoginDtoRequest request)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);


            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password,user.PasswordHash))
            {
                _logger.LogWarning("Login Failed With Email: {Email}", request.Email);
                return OperationResult<AuthenticationResultDto>.Unauthorized(GeneralErrors.Unauthorized("Email or Password is incorrect"));
            }


            var accessToken = _jwtProvider.GenerateToken(user);

            var refreshToken = _refreshTokenGenerator.Generate();

            var hash = _refreshTokenHasher.Hash(refreshToken);

            var refreshTokenEntity = new RefreshToken
            {
                TokenHash = hash,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDurationInDays),
                UserId = user.Id

            };

           await _refreshTokenRepository.AddAsync(refreshTokenEntity);

            await _unitOfWork.SaveChangesAsync();


            var response = _mapper.Map<LoginDtoResponse>(user);

            var result = new AuthenticationResultDto
            {
                RefreshToken = refreshToken,
                AccessToken = accessToken,
                User = response
            };


            return OperationResult<AuthenticationResultDto>.Success(result);

        }

        public async Task<OperationResult<GenerateRefreshAndAccessTokenDto>> RefreshAsync(string refreshToken)
        {
            var hash = _refreshTokenHasher.Hash(refreshToken);

            var refreshTokenEntity = await _refreshTokenRepository.GetByTokenHashAsync(hash,true);

            if (refreshTokenEntity is null)
            {
                return OperationResult<GenerateRefreshAndAccessTokenDto>.Unauthorized(GeneralErrors.Unauthorized("Invalid refresh token"));
            }

            if (refreshTokenEntity.RevokedAt is not null)
            {
                return OperationResult<GenerateRefreshAndAccessTokenDto>.Unauthorized(
                    GeneralErrors.Unauthorized("Refresh token has been revoked"));
            }

            if (refreshTokenEntity.ExpiresAt <= DateTime.UtcNow)
            {
                return OperationResult<GenerateRefreshAndAccessTokenDto>.Unauthorized(
                    GeneralErrors.Unauthorized("Refresh token has expired"));
            }

            var accessToken = _jwtProvider.GenerateToken(refreshTokenEntity.User);

            var newRefreshToken = _refreshTokenGenerator.Generate();

            var newRefreshTokenHash = _refreshTokenHasher.Hash(newRefreshToken);

            refreshTokenEntity.RevokedAt = DateTime.UtcNow;

            refreshTokenEntity.ReplacedByTokenHash = newRefreshTokenHash;

            var newRefreshTokenEntity = new RefreshToken
            {
                TokenHash = newRefreshTokenHash,

                UserId = refreshTokenEntity.UserId,

                CreatedAt = DateTime.UtcNow,

                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDurationInDays)
            };

           await _refreshTokenRepository.AddAsync(newRefreshTokenEntity);

           await _unitOfWork.SaveChangesAsync();

            var result = new GenerateRefreshAndAccessTokenDto
            {
                RefreshToken = newRefreshToken,
                AccessToken = accessToken,
            };

            return OperationResult<GenerateRefreshAndAccessTokenDto>.Success(result);


        }


    }
}

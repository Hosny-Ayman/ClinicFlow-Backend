using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Features.SysteamSettings.DTOs.Requests;
using ClinicFlow.Domain.InterFaces;

namespace ClinicFlow.Application.Features.SysteamSettings
{
    public class SysteamSettingService
    {

        private readonly ISysteamSettingRepository _systeamSettingRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly ISysteamSettingService _systeamSettingService;

        public SysteamSettingService(ISysteamSettingRepository systeamSettingRepository, IFileStorageService fileStorageService, ISysteamSettingService systeamSettingService)
        {
            _systeamSettingRepository = systeamSettingRepository;
            _fileStorageService = fileStorageService;
            _systeamSettingService = systeamSettingService;
        }


        public async Task <OperationResult<ImageDtoRequest>> GetSystemLogoAsync()
        {

            var SystemLogo = await _systeamSettingService.GetOnlySettingValueAsyncBySettingKeyAsync("SystemLogo");

            if(SystemLogo == null)
            {
                return OperationResult<ImageDtoRequest>.NotFound();
            }

            var Logo = _fileStorageService.GetFileUrl(SystemLogo);

            var request = new ImageDtoRequest
            {
                ImageUrl = Logo
            };

            return OperationResult<ImageDtoRequest>.Success(request);

        }

    }
}

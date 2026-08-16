using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Responses;
using ClinicFlow.Application.Features.SysteamSettings.DTOs.Requests;
using ClinicFlow.Domain.Interfaces;

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


        public async Task <OperationResult<ImageDtoRequest>> GetSystemImageAsync(string imageKey)
        {

            var SystemImage = await _systeamSettingService.GetOnlySettingValueAsyncBySettingKeyAsync(imageKey);

            if(SystemImage == null)
            {
                return OperationResult<ImageDtoRequest>.NotFound();
            }

            var Image = _fileStorageService.GetFileUrl(SystemImage);

            var request = new ImageDtoRequest
            {
                ImageUrl = Image
            };

            return OperationResult<ImageDtoRequest>.Success(request);

        }

    }
}

using BlueBellDolls.Grpc;
using BlueBellDolls.Server.Grpc;
using BlueBellDolls.Server.Interfaces;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace BlueBellDolls.Server.Services
{
    internal sealed class BlueBellDollsService : BlueBellDolls.Grpc.BlueBellDollsService.BlueBellDollsServiceBase
    {

        #region Fields

        private readonly ILogger<BlueBellDollsService> _logger;
        private readonly ICatService _catService;

        #endregion

        #region Constructor

        public BlueBellDollsService(
            ILogger<BlueBellDollsService> logger,
            ICatService catService)
        {
            _logger = logger;
            _catService = catService;
        }

        #endregion

        #region BlueBellDollsServiceBase implementation

        public override async Task<GetFemaleCatsResponce> GetActiveCatsByGender(BoolValue isMale, ServerCallContext context)
        {
            _logger.LogInformation("Called BlueBellDollsService.GetCatsByGender()");

            var result = new GetFemaleCatsResponce();
            try
            {
                var cats = await _catService.GetActiveCatsByGenderAsync(isMale.Value, context.CancellationToken);

                result.FemaleCats.AddRange(cats.Select(c => c.Compress()));
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception thrown in BlueBellDollsService.GetCatsByGender()");
                return result;
            }

        }

        public override async Task<GetLittersResponce> GetActiveLitters(Empty request, ServerCallContext context)
        {
            _logger.LogInformation("Called BlueBellDollsService.GetLitters()");

            var result = new GetLittersResponce();
            try
            {
                var litters = await _catService.GetActiveLittersAsync(context.CancellationToken);

                result.Litters.AddRange(litters.Select(l => l.Compress()));
                return result;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception thrown in BlueBellDollsService.GetLitters()");
                return result;
            }
        }

        #endregion

    }
}

using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Grpc;
using BlueBellDolls.Service.Grpc;
using BlueBellDolls.Service.Interfaces;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace BlueBellDolls.Service.Services
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

        public override async Task<BoolValue> AddNewCat(ParentCat request, ServerCallContext context)
        {
            return new BoolValue
            {
                Value = await AddNewEntityAsync(request.Decompress(), context.CancellationToken)
            };
        }

        public override async Task<BoolValue> AddNewLitter(Litter request, ServerCallContext context)
        {
            return new BoolValue
            {
                Value = await AddNewEntityAsync(request.Decompress(), context.CancellationToken)
            };
        }

        public override async Task<GetFemaleCatsResponce> GetCatsByGender(BoolValue isMale, ServerCallContext context)
        {
            _logger.LogInformation("Called BlueBellDollsService.GetCatsByGender()");

            var result = new GetFemaleCatsResponce();
            try
            {
                var cats = await _catService.GetCatsByGenderAsync(isMale.Value, context.CancellationToken);

                result.FemaleCats.AddRange(cats.Select(c => c.Compress()));
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception thrown in BlueBellDollsService.GetCatsByGender()");
                return result;
            }

        }

        public override async Task<GetLittersResponce> GetLitters(Empty request, ServerCallContext context)
        {
            _logger.LogInformation("Called BlueBellDollsService.GetLitters()");

            var result = new GetLittersResponce();
            try
            {
                var litters = await _catService.GetLittersAsync(context.CancellationToken);

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

        #region Methods

        private async Task<bool> AddNewEntityAsync(IEntity entity, CancellationToken token)
        {
            try
            {
                _logger.LogInformation("Called BlueBellDollsService.AddNewEntityAsync()");
                await _catService.AddNewEntityAsync(entity, token);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception thrown in BlueBellDollsService.AddNewEntityAsync()");
                return false;
            }
        }

        #endregion
    }
}

using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Types;

namespace BlueBellDolls.Common.Interfaces
{
    public interface ICommonMessageParametersProvider
    {
        MessageParameters GetNewBookingRequestParameters(BookingRequest bookingRequest);
    }
}
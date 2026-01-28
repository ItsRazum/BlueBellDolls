using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Types;

namespace BlueBellDolls.Common.Providers
{
    public class CommonMessageParametersProvider(
        ICommonKeyboardsProvider commonKeyboardsProvider,
        ICommonMessagesProvider commonMessagesProvider) : ICommonMessageParametersProvider
    {
        private readonly ICommonKeyboardsProvider _keyboardsProvider = commonKeyboardsProvider;
        private readonly ICommonMessagesProvider _messagesProvider = commonMessagesProvider;

        public MessageParameters GetNewBookingRequestParameters(BookingRequest bookingRequest)
        {
            return new MessageParameters(
                _messagesProvider.CreateNewBookingRequestMessage(bookingRequest),
                _keyboardsProvider.CreateProcessBookingKeyboard(bookingRequest.Id));
        }

        public MessageParameters GetNewFeedbackRequestParameters(FeedbackRequest feedbackRequest, FeedbackRequest? previousRequestFromThatUser = null)
        {
            return new MessageParameters(
                _messagesProvider.CreateNewFeedbackRequestMessage(feedbackRequest),
                _keyboardsProvider.CreateCloseFeebackRequestKeyboard(feedbackRequest.Id));
        }
    }
}

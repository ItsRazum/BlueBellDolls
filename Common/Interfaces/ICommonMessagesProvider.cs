using BlueBellDolls.Common.Models;

namespace BlueBellDolls.Common.Interfaces
{
    public interface ICommonMessagesProvider
    {
        string CreateBookingRequestTemplateMessage(BookingRequest bookingRequest, bool hidePhoneNumber = false);
        string CreateNewBookingRequestMessage(BookingRequest bookingRequest);

        string CreateNewFeedbackRequestMessage(FeedbackRequest feedbackRequest, FeedbackRequest? previousRequestFromThatUser = null);
    }
}
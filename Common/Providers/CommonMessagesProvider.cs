using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using System.Text;

namespace BlueBellDolls.Common.Providers
{
    public class CommonMessagesProvider : ICommonMessagesProvider
    {
        public string CreateNewBookingRequestMessage(BookingRequest bookingRequest)
            => CreateBookingRequestTemplateMessage(bookingRequest, true) +
            "\n⚠️Для CRM учёта необходимо взять кураторство, прежде чем звонить клиенту! Пожалуйста, нажмите кнопку ниже";

        public string CreateBookingRequestTemplateMessage(BookingRequest bookingRequest, bool hidePhoneNumber = false)
            => $"📢 Новый запрос на бронирование!\n" +
            $"├ Котёнок: {bookingRequest.Kitten?.Name}\n" +
            $"├ Имя клиента: {bookingRequest.CustomerName}\n" +
            $"└ Телефон клиента: {(hidePhoneNumber ? "<u>Необходимо взять кураторство над заявкой</u>" : $"<code>{bookingRequest.CustomerPhone}</code>")} \n" +
            "══════════════════════════";


        public string CreateNewFeedbackRequestMessage(FeedbackRequest feedbackRequest, FeedbackRequest? previousRequestFromThatUser)
        {
            var result = new StringBuilder();
            result.AppendLine(
                $"📢 Новый запрос на обратную связь!\n" +
                $"├ Имя клиента: {feedbackRequest.Name}\n" +
                $"└ Телефон клиента: <code>{feedbackRequest.Phone}</code> \n");

            if (previousRequestFromThatUser != null)
                result.AppendLine($"ℹ️Данный клиент уже отправлял запрос на обратную связь {previousRequestFromThatUser.CreatedAt:D}!");

            result.AppendLine("══════════════════════════");

            return result.ToString();
        }
    }
}

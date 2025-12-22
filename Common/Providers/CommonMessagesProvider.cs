using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;

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
    }
}

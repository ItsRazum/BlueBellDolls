using Telegram.Bot.Types.ReplyMarkups;

namespace BlueBellDolls.Bot.Builders
{
    public class InlineKeyboardButtonBuilder
    {
        private string _text;
        private string _callbackData;
        private KeyboardButtonStyle? _style;

        public InlineKeyboardButtonBuilder()
        {

        }

        public InlineKeyboardButtonBuilder(string text, string callbackData, KeyboardButtonStyle? style = null)
        {
            _text = text;
            _callbackData = callbackData;
            _style = style;
        }

        public InlineKeyboardButtonBuilder WithText(string text)
        {
            _text = text;
            return this;
        }

        public InlineKeyboardButtonBuilder WithCallbackData(string callbackData)
        {
            if (callbackData is { Length: > 64 })
                throw new InvalidOperationException("the length of the CallbackData cannot be more than 64");

            _callbackData = callbackData;
            return this;
        }

        public InlineKeyboardButtonBuilder WithStyle(KeyboardButtonStyle style)
        {
            _style = style;
            return this;
        }

        public InlineKeyboardButton Build()
        {
            if (_text == null || _callbackData == null)
                throw new InvalidOperationException("Text or callback data are empty");
            else
            {
                var result = new InlineKeyboardButton()
                {
                    Text = _text,
                    CallbackData = _callbackData,
                    Style = _style
                };

                return result;
            }
            
        }
    }
}

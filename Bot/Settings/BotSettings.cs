namespace BlueBellDolls.Bot.Settings
{
    public class BotSettings
    {
        public string Token { get; set; }
        public long[] AuthorizedUsers { get; set; }

        public Dictionary<string, string> CommandsSettings { get; set; }

        public InlineKeyboardsSettings InlineKeyboardsSettings { get; set; }

        public CallbackDataSettings CallbackDataSettings { get; set; }
    }
}

namespace Mix.Providers
{
    public class SecretProvider
    {
        public string DiscordBotToken => GetEnvironmentVariable("MIX_DISCORD_BOT_TOKEN");

        public string DiscordGuildIdentifier => GetEnvironmentVariable("MIX_DISCORD_GUILD_IDENTIFIER");

        private string GetEnvironmentVariable(string name) =>
            Environment.GetEnvironmentVariable(name) ?? throw new Exception($"Environment variable '{name}' not found.");
    }
}

namespace Mix.Modules
{
    [Group("test", "Test commands.")]
    public class TestModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("ping", "Ping the bot.")]
        public async Task PingAsync() =>
            await RespondAsync("pong");
    }
}

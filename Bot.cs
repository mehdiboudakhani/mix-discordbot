namespace Mix
{
    class Bot
    {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly IServiceProvider _serviceProvider;
        private readonly InteractionService _interactionService;

        public Bot()
        {
            _discordSocketClient = new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers | GatewayIntents.GuildVoiceStates | GatewayIntents.MessageContent
            });
            _serviceProvider = new ServiceCollection()
                .AddSingleton(_discordSocketClient)
                .AddSingleton(serviceProvider => new InteractionService(serviceProvider.GetRequiredService<DiscordSocketClient>()))
                .BuildServiceProvider();
            _interactionService = _serviceProvider.GetRequiredService<InteractionService>();
        }

        public async Task RunAsync()
        {
            _discordSocketClient.InteractionCreated += OnInteractionCreatedAsync;
            _discordSocketClient.Ready += OnReadyAsync;
            await _discordSocketClient.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("MIX_DISCORD_BOT_TOKEN"));
            await _discordSocketClient.StartAsync();
            await Task.Delay(-1);
        }

        private async Task OnInteractionCreatedAsync(SocketInteraction socketInteraction) =>
            await _interactionService.ExecuteCommandAsync(new SocketInteractionContext(_discordSocketClient, socketInteraction), _serviceProvider);

        private async Task OnReadyAsync() =>
            await _interactionService.RegisterCommandsToGuildAsync(ulong.Parse(Environment.GetEnvironmentVariable("MIX_DISCORD_GUILD_IDENTIFIER")!));
    }
}

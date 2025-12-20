namespace Mix.Modules
{
    [Group("temporary-voice-channels", "Temporary voice channels commands.")]
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    public class TemporaryVoiceChannelsModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly DiscordSocketClient _discordSocketClient;
        private static readonly HashSet<ulong> _hubs = [];
        private static readonly Dictionary<ulong, ulong> _temporaryVoiceChannels = [];

        public TemporaryVoiceChannelsModule(DiscordSocketClient discordSocketClient)
        {
            _discordSocketClient = discordSocketClient;
            _discordSocketClient.UserVoiceStateUpdated += HandleTemporaryVoiceChannelsAsync;
        }

        [SlashCommand("add-hub", "Add a hub.")]
        public async Task AddHubAsync(IVoiceChannel voiceChannel)
        {
            if (_hubs.Add(voiceChannel.Id))
            {
                var embed = new EmbedBuilder()
                    .WithTitle("Success")
                    .WithDescription($"Hub {voiceChannel.Name} added.")
                    .WithColor(Color.Green)
                    .Build();
                await RespondAsync(embed: embed, ephemeral: true);
            }
            else
            {
                var embed = new EmbedBuilder()
                    .WithTitle("Failed")
                    .WithDescription($"{voiceChannel.Name} is already a hub.")
                    .WithColor(Color.Red)
                    .Build();
                await RespondAsync(embed: embed, ephemeral: true);
            }
        }

        [SlashCommand("remove-hub", "Remove a hub.")]
        public async Task RemoveHubAsync(IVoiceChannel voiceChannel)
        {
            if (_hubs.Remove(voiceChannel.Id))
            {
                var embed = new EmbedBuilder()
                    .WithTitle("Success")
                    .WithDescription($"Hub {voiceChannel.Name} removed.")
                    .WithColor(Color.Green)
                    .Build();
                await RespondAsync(embed: embed, ephemeral: true);
            }
            else
            {
                var embed = new EmbedBuilder()
                    .WithTitle("Failed")
                    .WithDescription($"{voiceChannel.Name} is not a hub.")
                    .WithColor(Color.Red)
                    .Build();
                await RespondAsync(embed: embed, ephemeral: true);
            }
        }

        [SlashCommand("list-hubs", "List hubs.")]
        public async Task ListHubsAsync()
        {
            var embed = new EmbedBuilder()
                .WithTitle("Success")
                .WithDescription(_hubs.Count == 0 ? "No hubs." : string.Join("\n", _hubs.Select(id => $"<#{id}>")))
                .WithColor(Color.Green)
                .Build();
            await RespondAsync(embed: embed, ephemeral: true);
        }

        private async Task HandleTemporaryVoiceChannelsAsync(SocketUser user, SocketVoiceState before, SocketVoiceState after)
        {
            if (user is not SocketGuildUser socketGuildUser) return;
            if (after.VoiceChannel is not null && _hubs.Contains(after.VoiceChannel.Id) && !_temporaryVoiceChannels.ContainsKey(socketGuildUser.Id))
            {
                var categoryIdentifier = after.VoiceChannel.CategoryId;
                var temporaryVoiceChannel = await socketGuildUser.Guild.CreateVoiceChannelAsync($"{socketGuildUser.Username}'s channel", properties =>
                {
                    if (categoryIdentifier is not null)
                        properties.CategoryId = categoryIdentifier;
                });
                _temporaryVoiceChannels[socketGuildUser.Id] = temporaryVoiceChannel.Id;
                await socketGuildUser.ModifyAsync(properties => properties.Channel = temporaryVoiceChannel);
            }
            if (before.VoiceChannel is not null && _temporaryVoiceChannels.ContainsValue(before.VoiceChannel.Id) && before.VoiceChannel.ConnectedUsers.Count == 0)
            {
                await before.VoiceChannel.DeleteAsync();
                var temporaryVoiceChannel = _temporaryVoiceChannels.FirstOrDefault(keyValuePair => keyValuePair.Value == before.VoiceChannel.Id);
                if (!temporaryVoiceChannel.Equals(default(KeyValuePair<ulong, ulong>)))
                    _temporaryVoiceChannels.Remove(temporaryVoiceChannel.Key);
            }
        }
    }
}

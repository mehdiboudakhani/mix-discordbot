namespace Mix.Factories
{
    public class EmbedFactory
    {
        public Embed Failed(string message)
        {
            var embed = new EmbedBuilder()
                .WithTitle("Failed")
                .WithDescription(message)
                .WithColor(Color.Red)
                .Build();
            return embed;
        }

        public Embed Success(string message)
        {
            var embed = new EmbedBuilder()
                .WithTitle("Success")
                .WithDescription(message)
                .WithColor(Color.Green)
                .Build();
            return embed;
        }
    }
}

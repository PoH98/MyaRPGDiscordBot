using Newtonsoft.Json;

namespace MyaDiscordBot.Models
{
    public class Channel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public int Type { get; set; }
    }

    public class Guild
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("splash")]
        public object Splash { get; set; }

        [JsonProperty("banner")]
        public object Banner { get; set; }

        [JsonProperty("description")]
        public object Description { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("features")]
        public List<object> Features { get; set; }

        [JsonProperty("verification_level")]
        public int VerificationLevel { get; set; }

        [JsonProperty("vanity_url_code")]
        public object VanityUrlCode { get; set; }

        [JsonProperty("premium_subscription_count")]
        public int PremiumSubscriptionCount { get; set; }

        [JsonProperty("nsfw")]
        public bool Nsfw { get; set; }

        [JsonProperty("nsfw_level")]
        public int NsfwLevel { get; set; }
    }

    public class Inviter
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("avatar_decoration")]
        public object AvatarDecoration { get; set; }

        [JsonProperty("discriminator")]
        public string Discriminator { get; set; }

        [JsonProperty("public_flags")]
        public int PublicFlags { get; set; }
    }

    public class DiscordInvite
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("expires_at")]
        public object ExpiresAt { get; set; }

        [JsonProperty("guild")]
        public Guild Guild { get; set; }

        [JsonProperty("channel")]
        public Channel Channel { get; set; }

        [JsonProperty("inviter")]
        public Inviter Inviter { get; set; }
    }
}

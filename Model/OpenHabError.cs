using System.Text.Json.Serialization;

namespace OpenHab.UniFiProxy.Model
{
    public class OpenHabError
    {

        [JsonPropertyName("error")]
        public ErrorClass Error { get; set; }

        public partial class ErrorClass
        {
            [JsonPropertyName("message")]
            public string Message { get; set; }

            [JsonPropertyName("http-code")]
            public long HttpCode { get; set; }

            [JsonPropertyName("exception")]
            public ExceptionClass Exception { get; set; }
        }

        public partial class ExceptionClass
        {
            [JsonPropertyName("class")]
            public string Class { get; set; }

            [JsonPropertyName("message")]
            public string Message { get; set; }

            [JsonPropertyName("localized-message")]
            public string LocalizedMessage { get; set; }
        }

    }
}
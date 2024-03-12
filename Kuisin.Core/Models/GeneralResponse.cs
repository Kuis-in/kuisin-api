using System.Text.Json.Serialization;

namespace Kuisin.Core.Models
{
    public class GeneralResponse(string message)
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = message;
    }
}

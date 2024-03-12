using Kuisin.Core.Models;

namespace Kuisin.Core.Interfaces
{
    public interface IVideoService
    {
        Task<Video?> GetYoutubeVideoMetadataAsync(string videoId);
        Task<string> ExtractAudioFromYoutubeVideoAsync(string videoId, string uniqueId);
    }
}

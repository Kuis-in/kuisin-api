using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Kuisin.Core.Enums;
using Kuisin.Core.Interfaces;
using Kuisin.Core.Models;
using Kuisin.Infrastructure.Constants;
using Microsoft.Extensions.Configuration;
using System.Xml;

namespace Kuisin.Infrastructure.Services
{
    internal class VideoService : IVideoService
    {
        private readonly YouTubeService _youtubeService;
        private readonly YoutubeDLSharp.YoutubeDL _ydl;

        private static readonly string[] _ydlPostProcessorArgs = [ "ExtractAudio:-ac 1" ];

        public VideoService(AppConfig appConfig, IConfiguration configuration)
        {
            _youtubeService = new(new BaseClientService.Initializer
            {
                ApiKey = configuration[Configuration.YoutubeApiKey],
                ApplicationName = configuration[Configuration.ApplicationName]
            });
            _ydl = new()
            {
                YoutubeDLPath = Path.Combine(appConfig.ApplicationRootPath, "Binaries\\yt-dlp.exe"),
                FFmpegPath = Path.Combine(appConfig.ApplicationRootPath, "Binaries\\ffmpeg.exe")
            };
        }

        private static string GetYoutubeVideoUrl(string videoId) => string.Format("https://www.youtube.com/watch?v={0}", videoId);

        public async Task<Video?> GetYoutubeVideoMetadataAsync(string videoId)
        {
            var request = _youtubeService.Videos.List("snippet,contentDetails");
            request.Id = videoId;

            var result = await request.ExecuteAsync();
            if (result.Items.Count == 0) return null;

            var ytv = result.Items.First();
            return new Video
            {
                Id = videoId,
                Title = ytv.Snippet.Title,
                Description = ytv.Snippet.Description,
                ThumbnailUrl = ytv.Snippet.Thumbnails.Maxres?.Url ?? ytv.Snippet.Thumbnails.High?.Url ?? ytv.Snippet.Thumbnails.Default__.Url,
                VideoUrl = GetYoutubeVideoUrl(videoId),
                Source = VideoSource.YouTube,
                DurationSeconds = Convert.ToUInt32(XmlConvert.ToTimeSpan(ytv.ContentDetails.Duration).TotalSeconds)
            };
        }

        public async Task<string> ExtractAudioFromYoutubeVideoAsync(string videoId, string uniqueId)
        {
            var videoUrl = GetYoutubeVideoUrl(videoId);
            var result = await _ydl.RunAudioDownload(videoUrl, YoutubeDLSharp.Options.AudioConversionFormat.Mp3, overrideOptions: new YoutubeDLSharp.Options.OptionSet
            {
                PostprocessorArgs = _ydlPostProcessorArgs,
                Output = $"%(id)s_{uniqueId}.%(ext)s"
            });
            if (result.ErrorOutput.Length > 0)
            {
                throw new Exception(string.Join(" ", result.ErrorOutput));
            }
            return result.Data;
        }
    }
}

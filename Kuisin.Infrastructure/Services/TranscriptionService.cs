using Kuisin.Core.Interfaces;
using OpenAI_API;

namespace Kuisin.Infrastructure.Services
{
    internal class TranscriptionService : ITranscriptionService
    {
        private readonly IOpenAIAPI _openAI;

        public TranscriptionService(IOpenAIAPI openAI)
        {
            _openAI = openAI;
        }

        public async Task<string> GetAudioTranscriptionAsync(string audioFilePath)
        {
            return await _openAI.Transcriptions.GetTextAsync(audioFilePath);
        }
    }
}

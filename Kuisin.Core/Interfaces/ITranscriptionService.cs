namespace Kuisin.Core.Interfaces
{
    public interface ITranscriptionService
    {
        Task<string> GetAudioTranscriptionAsync(string audioFilePath);
    }
}

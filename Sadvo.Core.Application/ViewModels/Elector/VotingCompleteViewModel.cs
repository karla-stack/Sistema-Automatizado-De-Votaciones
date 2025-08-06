

namespace Sadvo.Core.Application.ViewModels.Elector
{
    public class VotingCompleteViewModel
    {
        public string CompletionMessage { get; set; } = string.Empty;
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public DateTime CompletionTime { get; set; } = DateTime.Now;
    }
}

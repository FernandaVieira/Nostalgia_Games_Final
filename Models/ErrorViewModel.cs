namespace Nostalgia_Games. s
{
    public class ErrorView 
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}

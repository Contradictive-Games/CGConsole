namespace CG.Console
{
    public enum ResponseType
    {
        Success,
        Invalid,
        Info,
        Error
    }
    
    public struct CommandResponse
    {
        public readonly ResponseType ResponseType;
        public readonly string Message;

        public CommandResponse(ResponseType responseType, string message = "")
        {
            ResponseType = responseType;
            Message = message;
        }
    }
}
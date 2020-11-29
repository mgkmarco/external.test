namespace External.Test.Contracts.Commands
{
    public class UpdateMarketResponse
    {
        public bool Success { get; }

        public UpdateMarketResponse(bool success)
        {
            Success = success;   
        }
    }
}
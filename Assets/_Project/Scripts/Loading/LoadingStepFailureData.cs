
namespace Loading
{
    public class LoadingStepFailureData
    {
        public string ErrorDescription { get; }
        public string RetryDescription { get; }
        public int MaxRetries { get; }
        public float RetryDelaySeconds { get; }

        public LoadingStepFailureData(string errorDescription = "Loading Step Failure!", string retryDescription = "Trying Loading Again...", int maxRetries = 2, float retryDelaySeconds = 1f)
        {
            ErrorDescription = errorDescription;
            RetryDescription = retryDescription;
            MaxRetries = maxRetries;
            RetryDelaySeconds = retryDelaySeconds;
        }
    }
}

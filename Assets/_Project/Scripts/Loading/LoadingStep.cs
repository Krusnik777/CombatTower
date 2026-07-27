using System;
using Cysharp.Threading.Tasks;

namespace Loading
{
    public class LoadingStep
    {
        public string Description { get; }
        public LoadingStepFailureData FailureData { get; }
        public Func<UniTask> ActionAsync { get; }

        public LoadingStep(string description, Func<UniTask> actionAsync, LoadingStepFailureData failureData = null)
        {
            Description = description;
            ActionAsync = actionAsync;
            FailureData = failureData;
        }
    }
}

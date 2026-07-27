using Cysharp.Threading.Tasks;
using Loading;

namespace UnityR3ProjectTemplate.Game.Services
{
    public class FakeAwaitableService
    {
        public LoadingStep CreateLoadingStep(string description, int waitingTime, System.Action action)
        {
            return new LoadingStep(description, async () =>
            {
                await UniTask.Delay(waitingTime);

                action?.Invoke();
            });
        }

        public LoadingStep CreateSometimesFailingLoadingStep(string description, int waitingTime, System.Action actionOnSuccess, System.Action actionOnFailure, float failureChance = 0.5f)
        {
            var failureData = new LoadingStepFailureData($"[IMITATION] Loading Error", $"[IMITATION] Trying Again");

            return new LoadingStep(description, async () =>
            {
                await UniTask.Delay(waitingTime);

                if (UnityEngine.Random.value > 1 - failureChance)
                {
                    actionOnFailure?.Invoke();

                    throw new System.Exception("Error loading imitation");
                }
                else
                {
                    actionOnSuccess?.Invoke();
                }

            }, failureData);
        }
    
    }
}

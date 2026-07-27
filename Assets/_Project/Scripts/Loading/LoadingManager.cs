using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Loading
{
    public class LoadingManager
    {
        private LoadingScreen _loadingScreen;

        public LoadingManager(LoadingScreen loadingScreen)
        {
            _loadingScreen = loadingScreen;
            _loadingScreen.Hide();
        }

        public string GetActiveSceneName() => SceneManager.GetActiveScene().name;

        public void LoadScene(string sceneName, List<LoadingStep> steps, LoadingStep stepAfterSceneLoaded = null)
        {
            LoadSceneAsync(sceneName, steps, stepAfterSceneLoaded).Forget();
        }

        public async UniTask LoadSceneAsync(string sceneName, List<LoadingStep> steps, LoadingStep stepAfterSceneLoaded = null)
        {
            var loadSceneStep = new LoadingStep("Loading scene...", async () => await LoadSceneTask(sceneName));
            steps.Add(loadSceneStep);

            if (stepAfterSceneLoaded != null) steps.Add(stepAfterSceneLoaded);

            var stepFraction = 1f / steps.Count;
            var totalProgress = 0f;

            _loadingScreen.SetProgress(totalProgress);
            _loadingScreen.Show();

            for (int i = 0; i < steps.Count; i++)
            {
                var step = steps[i];

                if (step.FailureData == null)
                {
                    _loadingScreen.SetStatus(steps[i].Description);

                    await step.ActionAsync();

                    totalProgress += stepFraction;
                    _loadingScreen.SetProgress(totalProgress);
                }
                else
                {
                    bool success = false;
                    int attempt = 0;

                    while (!success && attempt <= step.FailureData.MaxRetries)
                    {
                        attempt++;

                        string statusText = attempt == 1 ? step.Description : $"{step.FailureData.RetryDescription} (try #{attempt})";
                        _loadingScreen.SetStatus(statusText);

                        try
                        {
                            await step.ActionAsync();

                            success = true;
                        }
                        catch (System.Exception e)
                        {
                            if (attempt <= step.FailureData.MaxRetries)
                            {
                                _loadingScreen.SetStatus($"{step.FailureData.ErrorDescription}: {e.Message}");

                                await UniTask.Delay(System.TimeSpan.FromSeconds(step.FailureData.RetryDelaySeconds));
                            }
                            else
                            {
                                _loadingScreen.SetStatus("[IMITATION] Critical Loading Error!");

                                await UniTask.Delay(500);

                                _loadingScreen.SetStatus("Though this is [IMITATION], so starting next step.");

                                await UniTask.Delay(500);

                                success = true;
                            }
                        }
                    }
                }
            }

            _loadingScreen.Hide();
        }

        public LoadingStep CreateSceneLoadingStep(string sceneName, string stepDescription, int delayTime = 500)
        {
            return new LoadingStep(stepDescription, async () => await LoadSceneTask(sceneName, delayTime));
        }

        public LoadingStep CreateWaitingLoadingStep(string description, int waitingTime, System.Action action)
        {
            return new LoadingStep(description, async () =>
            {
                await UniTask.Delay(waitingTime);

                action?.Invoke();
            });
        }

        private async UniTask LoadSceneTask(string sceneName, int delayTime = 500)
        {
            var asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            asyncOperation.allowSceneActivation = false;

            while (asyncOperation.progress < 0.9f)
            {
                await UniTask.Yield();
            }

            await UniTask.Delay(delayTime);

            asyncOperation.allowSceneActivation = true;
        }
    }
}

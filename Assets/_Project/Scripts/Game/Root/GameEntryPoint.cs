using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Loading;
using R3;
using DI;
using UnityR3ProjectTemplate.Game.EntryPoints;
using UnityR3ProjectTemplate.Game.Services;

namespace UnityR3ProjectTemplate.Game.Root
{
    public class GameEntryPoint
    {
        private static GameEntryPoint _instance;

        private readonly UIRootView _uiRoot;
        private readonly LoadingManager _loadingManager;
        
        private readonly DIContainer _rootContainer = new();
        private DIContainer _cachedSceneContainer;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void AutostartGame()
        {
            _instance = new GameEntryPoint();
            _instance.RunGame();
        }

        private GameEntryPoint()
        {
            var prefabUIRoot = Resources.Load<UIRootView>("UIRoot");
            _uiRoot = Object.Instantiate(prefabUIRoot);
            Object.DontDestroyOnLoad(_uiRoot.gameObject);
            _rootContainer.RegisterInstance(_uiRoot);

            _loadingManager = new(_uiRoot.LoadingScreen);

            SetupAudioService();
            SetupInputServices();
            SetupProviders();
        }

        private void RunGame()
        {
            #if UNITY_EDITOR

            var sceneName = _loadingManager.GetActiveSceneName();

            if (sceneName == Scenes.GAMEPLAY)
            {
                LoadAndStartGameplay(new GameplayEnterParameters(0), true);

                return;
            }

            if (sceneName != Scenes.BOOTSTRAP)
            {
                return;
            }
            
            #endif

            LoadAndStartGameplay(new GameplayEnterParameters(0), true);
        }

        private void LoadAndStartGameplay(GameplayEnterParameters enterParams, bool isFromBootstrap = false)
        {
            _cachedSceneContainer?.Dispose();

            List<LoadingStep> steps = new()
            {
                _loadingManager.CreateSceneLoadingStep(Scenes.BOOTSTRAP, isFromBootstrap ? "Initializing Global Services..." : "Scene Cleanup..."),
            };

            var finalLoadingStep = _loadingManager.CreateWaitingLoadingStep("Final Scene Setup...", 250, () => HandleGameplayEntryPoint(enterParams));
            _loadingManager.LoadScene(Scenes.GAMEPLAY, steps, finalLoadingStep);
        }

        private void HandleGameplayEntryPoint(GameplayEnterParameters enterParams)
        {
            var sceneEntryPoint = Object.FindFirstObjectByType<EntryPoint<GameplayEnterParameters, GameplayExitParameters>>();
            var sceneContainer = _cachedSceneContainer = new DIContainer(_rootContainer);
            sceneEntryPoint.Run(sceneContainer, enterParams).Subscribe(exitParameters =>
            {
                if (exitParameters.ExitTag == GameplayTags.RESTART)
                {
                    LoadAndStartGameplay(new GameplayEnterParameters(exitParameters.Runs));

                    return;
                }

                if (exitParameters.ExitTag == GameplayTags.NEXT)
                {
                    LoadAndStartGameplay(new GameplayEnterParameters(exitParameters.Runs));

                    return;
                }

                if (exitParameters.ExitTag == GameplayTags.EXIT)
                {
                    #if UNITY_EDITOR
                    LoadAndStartGameplay(new GameplayEnterParameters(0));
                    #else
                    Application.Quit();
                    #endif

                    return;
                }

                throw new System.NotImplementedException("[Gameplay Exit Parameters] Current exit parameters currently not supported");
            });
        }

        #region Services Setup Methods

        private void SetupAudioService()
        {
            var audioService = new AudioService();
            _rootContainer.RegisterInstance(audioService);
        }

        private void SetupInputServices()
        {
            var inputDeviceDetectService = new InputDeviceDetectService();
            _rootContainer.RegisterInstance(inputDeviceDetectService);

            var gameInputService = new GameInputService();
            _rootContainer.RegisterInstance(gameInputService);
        }

        private void SetupProviders()
        {
            /*var playerAvatarConfigProvider = new PlayerConfigsProvider();
            _rootContainer.RegisterInstance(playerAvatarConfigProvider);*/
        }

        #endregion
    }
}

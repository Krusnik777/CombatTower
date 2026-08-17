using DI;
using CombatTower.Game.Root;
using CombatTower.Game.Services;
using UI;
using R3;
using UnityEngine;
using CombatTower.Game.Gameplay.Entities.Player;
using CombatTower.Game.Gameplay;

namespace CombatTower.Game.EntryPoints
{
    public class GameplayEntryPoint : EntryPoint<GameplayEnterParameters, GameplayExitParameters>
    {
        [SerializeField] private UISceneRootView m_sceneUIRootPrefab;
        [SerializeField] private PlayerView m_playerView;
        [SerializeField] private CameraRotation m_cameraRotation;
        [SerializeField] private LockOnCamera m_lockOnCamera;

        private Subject<GameplayExitParameters> _onEnd;

        public override Observable<GameplayExitParameters> Run(DIContainer sceneContainer, GameplayEnterParameters enterParameters)
        {
            _onEnd = new();

            RegisterLocalInstances(sceneContainer, enterParameters);
            SetupUI(sceneContainer);

            return _onEnd;
        }

        private void OnDestroy()
        {
            DisposeOfListeners();
        }

        private void Exit(GameplayExitParameters exitParameters)
        {
            DisposeOfListeners();

            _onEnd.OnNext(exitParameters);
        }

        private void DisposeOfListeners()
        {
            
        }

        private void RegisterLocalInstances(DIContainer sceneContainer, GameplayEnterParameters enterParameters)
        {
            var restartInvoker = new EventInvoker(() => Exit(new(GameplayTags.RESTART, enterParameters.Runs)));
            var nextInvoker = new EventInvoker(() => Exit(new(GameplayTags.NEXT, enterParameters.Runs + 1)));
            var exitInvoker = new EventInvoker(() => Exit(new(GameplayTags.EXIT, enterParameters.Runs)));

            sceneContainer.RegisterInstance(GameplayTags.RESTART, restartInvoker as IEventInvoker);
            sceneContainer.RegisterInstance(GameplayTags.NEXT, nextInvoker as IEventInvoker);
            sceneContainer.RegisterInstance(GameplayTags.EXIT, exitInvoker as IEventInvoker);

            var inputService = sceneContainer.Resolve<GameInputService>();
            m_cameraRotation.Bind(inputService);
            sceneContainer.RegisterInstance(m_cameraRotation);

            var playerConfigsProvider = sceneContainer.Resolve<PlayerConfigsProvider>();

            var lockOnHandler = new LockOnHandler(playerConfigsProvider.ParametersConfig, m_cameraRotation.transform, m_lockOnCamera, m_playerView.Movement, m_playerView.transform);
            lockOnHandler.SubcribeToLockOnInput(inputService.OnLockOnPressed, inputService.OnLockOnTargetSwitchPressed);
            sceneContainer.RegisterInstance(lockOnHandler); 

            var player = new Player(playerConfigsProvider.ParametersConfig, m_playerView, sceneContainer);
            sceneContainer.RegisterInstance(player);
        }

        private void SetupUI(DIContainer sceneContainer)
        {
            var uiRoot = sceneContainer.Resolve<UIRootView>();
            var uiSceneRoot = Instantiate(m_sceneUIRootPrefab);
            uiRoot.AttachSceneUI(uiSceneRoot.gameObject);

            var windowsFactory = new GameplayWindowsFactory(uiSceneRoot.ScreensTransform, uiSceneRoot.PopupsTransform);
            sceneContainer.RegisterInstance(new UIWindowsProvider(windowsFactory));
            //sceneContainer.RegisterFactory(_ => new UIWindowsProvider(windowsFactory)).AsSingle();
        }
    }
}

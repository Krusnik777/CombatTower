using DI;
using CombatTower.Game.Root;
using CombatTower.Game.Services;
using UI;
using R3;
using UnityEngine;
using CombatTower.Game.Gameplay.Entities.Player;
using CombatTower.Game.Gameplay;
using Unity.Cinemachine;

namespace CombatTower.Game.EntryPoints
{
    public class GameplayEntryPoint : EntryPoint<GameplayEnterParameters, GameplayExitParameters>
    {
        [SerializeField] private UISceneRootView m_sceneUIRootPrefab;
        [SerializeField] private PlayerView m_playerView;
        [SerializeField] private CameraRotation m_cameraRotation;
        [SerializeField] private CinemachineCamera m_lockOnCamera;
        [SerializeField] private Transform m_lookTarget;

        private Subject<GameplayExitParameters> _onEnd;

        private bool _isLookLocked;
        private System.IDisposable _testDisposable;

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
            _testDisposable?.Dispose();
        }

        private void RegisterLocalInstances(DIContainer sceneContainer, GameplayEnterParameters enterParameters)
        {
            var restartInvoker = new EventInvoker(() => Exit(new(GameplayTags.RESTART, enterParameters.Runs)));
            var nextInvoker = new EventInvoker(() => Exit(new(GameplayTags.NEXT, enterParameters.Runs + 1)));
            var exitInvoker = new EventInvoker(() => Exit(new(GameplayTags.EXIT, enterParameters.Runs)));

            sceneContainer.RegisterInstance(GameplayTags.RESTART, restartInvoker as IEventInvoker);
            sceneContainer.RegisterInstance(GameplayTags.NEXT, nextInvoker as IEventInvoker);
            sceneContainer.RegisterInstance(GameplayTags.EXIT, exitInvoker as IEventInvoker);

            var player = new Player(m_playerView, sceneContainer);
            sceneContainer.RegisterInstance(player);

            var inputService = sceneContainer.Resolve<GameInputService>();
            m_cameraRotation.Bind(inputService);
            sceneContainer.RegisterInstance(m_cameraRotation);

            _testDisposable = inputService.OnLockOnPressed.Subscribe(_ =>
            {
                _isLookLocked = !_isLookLocked;

                m_playerView.Movement.SetLookTransform(_isLookLocked ? m_lookTarget : null);
                m_cameraRotation.gameObject.SetActive(!_isLookLocked);
                m_lockOnCamera.gameObject.SetActive(_isLookLocked);
            });
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

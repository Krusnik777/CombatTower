using DI;
using CombatTower.Game.Root;
using CombatTower.Game.Services;
using UI;
using R3;
using UnityEngine;
using CombatTower.Game.Gameplay.Entities.Player;

namespace CombatTower.Game.EntryPoints
{
    public class GameplayEntryPoint : EntryPoint<GameplayEnterParameters,GameplayExitParameters>
    {
        [SerializeField] private UISceneRootView m_sceneUIRootPrefab;
        [SerializeField] private PlayerView m_playerView;

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

            var player = new Player(m_playerView, sceneContainer);
            sceneContainer.RegisterInstance(player);

            /*var inputService = sceneContainer.Resolve<GameInputService>();
            m_playerMovement.Bind(inputService);
            m_playerAnimator.Bind(m_playerMovement);

            _testDisposable = inputService.OnAbilityXPressed.Subscribe(_ =>
            {
                if (_playingAttack) return;

                _testDisposable2?.Dispose();

                m_playerMovement.SetActive(false);
                m_playerMovement.IsControlledByRootMotion = true;
                m_playerAnimator.PlayAttack();
                _playingAttack = true;

                _testDisposable2 = Observable.Timer(System.TimeSpan.FromSeconds(2f)).Subscribe(_ =>
                {
                    _testDisposable2?.Dispose();

                    m_playerMovement.IsControlledByRootMotion = false;
                    m_playerMovement.SetActive(true);
                    _playingAttack = false;
                });
            });*/
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

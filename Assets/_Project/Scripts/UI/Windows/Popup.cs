using R3;

namespace UI.Windows
{
    public abstract class Popup : Window
    {
        public virtual bool IsMultipleInstancesAllowed => false;

        private CompositeDisposable _closeListenerDisposable;

        public Popup(IWindowView view) : base(view)
        {
            _closeListenerDisposable = new();
        }

        public override void Dispose()
        {
            base.Dispose();

            _closeListenerDisposable?.Dispose();
            _view?.Dispose();
        }

        public void SubscribeToClose(System.Action<Popup> onClosed)
        {
            _closeListenerDisposable.Add(OnClose.Subscribe(_ => onClosed?.Invoke(this)));
        }

        public void ClearSubscriptions() => _closeListenerDisposable?.Clear();

        //public abstract void Initialize(IPopupInitData initData = null);
    }
}

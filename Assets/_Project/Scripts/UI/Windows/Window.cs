using System;
using R3;

namespace UI.Windows
{
    public abstract class Window : IDisposable
    {
        public Observable<Window> OnClose => _onClose;

        protected readonly IWindowView _view;

        protected bool _isShowing { get; private set; }

        private readonly Subject<Window> _onClose = new();

        public Window(IWindowView view)
        {
            _view = view;
        }

        public virtual void Show()
        {
            _view.Show();

            _isShowing = true;
        }

        public virtual void Hide()
        {
            _view.Hide();

            _isShowing = false;

            _onClose?.OnNext(this);
        }

        public virtual void Dispose() { }
    }
}

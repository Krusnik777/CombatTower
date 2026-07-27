using UnityEngine;

namespace UI.Windows
{
    public abstract class WindowView : MonoBehaviour, IWindowView
    {
        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Dispose()
        {
            Destroy(gameObject);
        }
    }
}

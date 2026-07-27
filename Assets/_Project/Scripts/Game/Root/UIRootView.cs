using Loading;
using UnityEngine;

namespace UnityR3ProjectTemplate.Game.Root
{
    public class UIRootView : MonoBehaviour
    {
        [field: SerializeField] public LoadingScreen LoadingScreen { get; private set; }
        [SerializeField] private Transform _UISceneContainer;

        public void AttachSceneUI(GameObject sceneUI)
        {
            ClearSceneUI();

            sceneUI.transform.SetParent(_UISceneContainer, false);
        }

        private void ClearSceneUI()
        {
            var childCount = _UISceneContainer.childCount;

            for (int i = 0; i < childCount; i++)
            {
                Destroy(_UISceneContainer.GetChild(i).gameObject);
            }
        }

    }
}

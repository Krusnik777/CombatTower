using UnityEngine;

namespace UI
{
    public class UISceneRootView : MonoBehaviour
    {
        [field: SerializeField] public Transform ScreensTransform { get; private set; }
        [field: SerializeField] public Transform PopupsTransform { get; private set; }
    }
}

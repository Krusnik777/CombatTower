using UnityEngine;

namespace CombatTower.Game.Settings
{
    [CreateAssetMenu(fileName = "ApplicationSettings", menuName = "Scriptable Objects/Game Settings/Application Settings")]
    public class ApplicationSettings : ScriptableObject
    {
        [field: Header("Sounds")]
        [field: SerializeField][field: Range(-80, 20)] public int SFXVolume { get; private set; } = 0;
        [field: SerializeField][field: Range(-80, 20)] public int BGMVolume { get; private set; } = 0;
    }
}

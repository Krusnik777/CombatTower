using DI;
using R3;
using UnityEngine;

namespace CombatTower.Game.EntryPoints
{
    public abstract class EntryPoint<TEnterParams,TExitParams> : MonoBehaviour where TEnterParams : SceneEnterParameters where TExitParams : SceneExitParameters
    {
        public abstract Observable<TExitParams> Run(DIContainer sceneContainer, TEnterParams enterParameters);
    }
}

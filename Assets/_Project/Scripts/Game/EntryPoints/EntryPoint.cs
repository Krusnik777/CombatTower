using DI;
using R3;
using UnityEngine;

namespace UnityR3ProjectTemplate.Game.EntryPoints
{
    public abstract class EntryPoint<TEnterParams,TExitParams> : MonoBehaviour where TEnterParams : SceneEnterParameters where TExitParams : SceneExitParameters
    {
        public abstract Observable<TExitParams> Run(DIContainer sceneContainer, TEnterParams enterParameters);
    }
}

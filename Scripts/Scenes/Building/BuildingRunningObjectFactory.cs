namespace TheOneStudio.HyperCasual.DrawCarBase.Scripts.Runtime.Scenes.Building
{
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public abstract class BuildingRunningObjectFactory
    {
        public abstract UniTask<GameObject> Create();
    }
}
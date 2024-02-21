namespace TheOneStudio.UITemplate.UITemplate.Elements
{
    using UnityEngine;

    public interface IElementView
    {
        public GameObject GameObject => (this as Component)?.gameObject;
    }

    public abstract class BaseElementView : MonoBehaviour, IElementView
    {
    }
}
namespace UITemplate.Scripts.Scenes.Utils
{
    using System;
    using DG.Tweening;
    using DG.Tweening.Core;
    using DG.Tweening.Plugins.Options;
    using UniRx;
    using UnityEngine;

    public static class UITemplateAnimationExtensions
    {
        public static IDisposable Spin(this Transform obj, float speed, RotateAxis axis = RotateAxis.Z)
        {
            return Observable.EveryUpdate().Subscribe(_ =>
            {
                switch (axis)
                {
                    case RotateAxis.X:
                        RotateX();
                        break;
                    case RotateAxis.Y:
                        RotateY();
                        break;
                    case RotateAxis.Z:
                        RotateZ();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
                }
            });

            void RotateX()
            {
                var eulerAngles = obj.eulerAngles;
                var xAngle      = eulerAngles.x + speed * Time.deltaTime;
                eulerAngles     = new Vector3(xAngle, eulerAngles.y, eulerAngles.z);
                obj.eulerAngles = eulerAngles;
            }

            void RotateY()
            {
                var eulerAngles = obj.eulerAngles;
                var yAngle      = eulerAngles.y + speed * Time.deltaTime;
                eulerAngles     = new Vector3(eulerAngles.x, yAngle, eulerAngles.z);
                obj.eulerAngles = eulerAngles;
            }

            void RotateZ()
            {
                var eulerAngles = obj.eulerAngles;
                var zAngle      = eulerAngles.z + speed * Time.deltaTime;
                eulerAngles     = new Vector3(eulerAngles.x, eulerAngles.y, zAngle);
                obj.eulerAngles = eulerAngles;
            }
        }
    }

    public enum RotateAxis
    {
        X,
        Y,
        Z
    }
}
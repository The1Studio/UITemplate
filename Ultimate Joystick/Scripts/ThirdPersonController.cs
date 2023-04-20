namespace Ultimate_Joystick.Scripts
{
    using UnityEngine;

    public class ThirdPersonController : MonoBehaviour
    {
        public float speed = 1;

        private void Update()
        {
            if (!Input.GetMouseButton(0)) return;
            var h = UltimateJoystick.GetHorizontalAxis("draw");

            var v             = UltimateJoystick.GetVerticalAxis("draw");
            var transform1    = this.transform;
            var localPosition = transform1.localPosition;
            localPosition            += new Vector3(h, 0, v) * Time.deltaTime * this.speed;
            transform1.localPosition =  localPosition;
        }
    }
}
#region Package

using UnityEngine;

#endregion

namespace Runtime.World.Overworld.Decorations
{
    public class Windmill : MonoBehaviour
    {
        private Transform wings;
        private float speed = 20;
        private Vector3 forward;

        private void Start()
        {
            Transform tempTrans;
            wings = (tempTrans = transform).GetChild(1);
            forward = tempTrans.up;
            wings.Rotate(forward, Random.Range(1f, 100f));
            speed += Random.Range(-5f, 5f);
        }

        private void Update()
        {
            wings.Rotate(forward, (speed * Time.deltaTime));
        }
    }
}
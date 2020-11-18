using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class Controller : MonoBehaviour
    {
        #region Values
        [Header("Object Reference:")]
        [SerializeField] private Animator anim = null;

        [Header("Movement:")]
        [SerializeField] private Transform moveOrigin = null;
        [SerializeField] private float speed = 0;
        [SerializeField] private Vector2 moveDir = Vector2.zero;
        #endregion

        private void Start()
        {
            if (anim == null)
                anim = GetComponent<Animator>();
        }

        private void Update()
        {
            GetInputFromSystem();
            SetAnimator();
            Move();
        }

        #region In
        private void GetInputFromSystem()
        {
            moveDir.x = Input.GetAxis("Horizontal");
            if (moveDir.x < 0)
                moveDir.x = -1;
            else if (moveDir.x > 0)
                moveDir.x = 1;
            else
                moveDir.x = 0;

            moveDir.y = Input.GetAxis("Vertical");
            if (moveDir.y < 0)
                moveDir.y = -1;
            else if (moveDir.y > 0)
                moveDir.y = 1;
            else
                moveDir.y = 0;
        }
        #endregion

        #region Internal
        private void SetAnimator()
        {
            anim.SetInteger("Horizontal", (int)moveDir.x);
            anim.SetInteger("Vertical", (int)moveDir.y);
        }

        private void Move()
        {
            moveOrigin.transform.position += new Vector3(moveDir.x, moveDir.y, 0) * speed * Time.deltaTime;
        }
        #endregion
    }
}
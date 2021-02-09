using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUsingCamera : MonoBehaviour
{
    //camTransform Should Be Set To The VR Cam And Is Where We Get Our Look Direction.
    [SerializeField] private Transform camTransform = null;
    //speed Is How Fast The Object Moves.
    [SerializeField] private float speed = 1;
    //angel Determines When The Object Can Move. 
    [SerializeField] private float angel = 35;
    //moveTransform Is Made To Hold The Forward Direction.
    private Transform moveTransform = null;

    private void Start()
    {
        //Making A New Empty GameObject To Use Its Transform.
        moveTransform = Instantiate(new GameObject()).transform;
        //Giving It A Name To Make It Easier To Find.
        moveTransform.gameObject.name = "Move Transform";
        //Setting The Parent To The Object So moveTransform Will Follow.
        moveTransform.parent = transform;
        //Reseting Its Position
        moveTransform.localPosition = Vector3.zero;
    }

    void Update()
    {
        //Check Angel Between Directly Down And Where VR Cam Are Looking.
        float calcAngel = Vector3.Angle(camTransform.forward, -Vector3.up);

        //Move If The Calculated Angel Is Less Then Or Equal To "angel" Then Move.
        if (calcAngel <= angel)
        {
            //Setting moveTransform To Face The Same Direction As The Cam But Only Around The Y-Axis.
            moveTransform.rotation = Quaternion.Euler(0, camTransform.localRotation.eulerAngles.y, 0);
            //Moving The Object. Time.deltaTime Makes The Movement Smooth
            transform.position += moveTransform.forward * speed * Time.deltaTime;
        }
    }
}

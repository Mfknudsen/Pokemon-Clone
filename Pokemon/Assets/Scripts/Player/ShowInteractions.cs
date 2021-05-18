using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowInteractions : MonoBehaviour
{
    #region Values
    [SerializeField] private GameObject closestInteractable = null;
    [SerializeField] private List<GameObject> interactableInRange = new List<GameObject>();
    #endregion

    #region Internal
    private void Evaluate()
    {
        float dist = 0;
        Vector3 playerPos = Player.MasterPlayer.instance.transform.position;

        if (closestInteractable != null)
            dist = Vector3.Distance(playerPos, closestInteractable.transform.position);

        foreach (GameObject obj in interactableInRange)
        {
            if (closestInteractable != obj)
            {
                float tempDist = Vector3.Distance(playerPos, obj.transform.position);

                if (tempDist < dist)
                {
                    dist = tempDist;
                    closestInteractable = obj;
                }
            }
        }
    }
    #endregion

    #region Collision
    private void OnCollisionEnter(Collision collision)
    {
        InteractableInterface holder = collision.gameObject.GetComponent<InteractableInterface>();

        if (holder != null)
        {
            if (!interactableInRange.Contains(gameObject))
            {
                interactableInRange.Add(collision.gameObject);
                Evaluate();
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        InteractableInterface holder = collision.gameObject.GetComponent<InteractableInterface>();

        if (holder != null)
        {
            if (interactableInRange.Contains(gameObject))
            {
                interactableInRange.Remove(gameObject);
                Evaluate();
            }
        }
    }
    #endregion
}
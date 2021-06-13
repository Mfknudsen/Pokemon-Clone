#region SDK

using Mfknudsen.Battle.UI;
using Mfknudsen.Monster;
using UnityEngine; //Custom

#endregion

namespace Mfknudsen.Battle.Systems
{
    public class Spot : MonoBehaviour
    {
        #region Values
        [SerializeField] private bool active = true, needNew = true;
        [SerializeField] private Pokemon activePokemon = null;
        [SerializeField] private int spotNumber = -1;
        [SerializeField] private Spot left = null, right = null, front = null;
        [SerializeField] private Transform currentTransform = null;
        [SerializeField] private Trainer.Team teamAllowed = null;

        [Header("UI:")]
        [SerializeField] Vector2 offset = Vector2.zero;
        [SerializeField] private PokemonDisplay display = null;
        [SerializeField] private GameObject displayPrefab = null;
        #endregion

        private void Update()
        {
            //DEBUG
            if (left != null)
                Debug.DrawRay(currentTransform.position, currentTransform.position - left.GetTransform().position);
            if (right != null)
                Debug.DrawRay(currentTransform.position, currentTransform.position - right.GetTransform().position);
            if (front != null)
                Debug.DrawRay(currentTransform.position, currentTransform.position - front.GetTransform().position);
            //
        }

        #region Getters
        public Spot GetLeft()
        {
            return left;
        }
        public Spot GetRight()
        {
            return right;
        }
        public Spot GetFront()
        {
            return front;
        }

        public bool GetNeedNew()
        {
            return needNew;
        }

        public Pokemon GetActivePokemon()
        {
            return activePokemon;
        }

        public int GetSpotNumber()
        {
            return spotNumber;
        }

        public bool GetActive()
        {
            return active;
        }

        public Transform GetTransform()
        {
            return currentTransform;
        }

        public Trainer.Team GetAllowedTeam()
        {
            return teamAllowed;
        }
        #endregion

        #region Setters
        public void SetLeft(Spot set)
        {
            left = set;
        }
        public void SetRight(Spot set)
        {
            right = set;
        }
        public void SetFront(Spot set)
        {
            front = set;
        }

        public void SetNeedNew(bool set)
        {
            needNew = set;
        }

        public void SetActivePokemon(Pokemon set)
        {
            activePokemon = set;

            if (set != null)
                display.SetNewPokemon(set);
        }

        public void SetSpotNumber(int set)
        {
            spotNumber = set;
        }

        public void SetActive(bool set)
        {
            active = set;
        }

        public void SetTransform()
        {
            currentTransform = transform;
        }

        public void SetAllowedTeam(Trainer.Team t)
        {
            if (t != null)
                teamAllowed = t;
        }
        #endregion

        #region In
        public void Setup(Transform origin, int i)
        {
            GameObject obj = Instantiate(displayPrefab);
            Vector2 v2 = origin.position;
            obj.transform.position = v2 + offset * i;
            obj.transform.parent = origin;
            obj.transform.localScale = Vector3.one;

            display = obj.GetComponent<PokemonDisplay>();
        }
        #endregion
    }
}
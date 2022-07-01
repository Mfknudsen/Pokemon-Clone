#region Packages

using System.Collections.Generic;
using Mfknudsen.Communication;
using Mfknudsen.World.Overworld.Interactions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

#endregion

namespace Mfknudsen.AI
{
    public abstract class NpcBase : MonoBehaviour, IInteractable
    {
        #region Values

        [FoldoutGroup("Base")] [SerializeField]
        protected Chat idleChat;

        [FoldoutGroup("Base/Visual")] protected GameObject visualsObject;

        [FoldoutGroup("Base/Navmesh")] protected NavMeshAgent agent;

        private Dictionary<string, object> memoryBank = new();

        #endregion

        #region Getters

        public NavMeshAgent GetAgent()
        {
            return agent;
        }

        public TObject GetFromMemory<TObject>(string key) where TObject : Object
        {
            if (!memoryBank.ContainsKey(key)) return null;

            return memoryBank[key] as TObject;
        }
        
        #endregion

        #region Setters

        public void SetMemory(string key, object value)
        {
            if (memoryBank.ContainsKey(key))
            {
                memoryBank[key] = value;
                return;
            }
            
            memoryBank.Add(key, value);
        }

        #endregion
        
        #region In

        public abstract void Trigger();

        public void HideVisual()
        {
            visualsObject.SetActive(false);
        }

        public void ShowVisual()
        {
            visualsObject.SetActive(true);
        }

        #endregion
    }
}
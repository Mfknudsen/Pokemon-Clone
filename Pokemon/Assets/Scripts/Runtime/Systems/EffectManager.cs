using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.Systems.PersistantRunner;
using Runtime.VFX;
using Runtime.VFX.World;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.Systems
{
    [CreateAssetMenu, Serializable]
    public sealed class EffectManager : ScriptableObject, IFrameUpdate
    {
        [SerializeField] private List<EffectLimit> effectsLimits = new();

        private readonly Dictionary<string, int> maxPerEffect = new();
        private readonly Dictionary<string, List<GameObject>> currentEffects = new();
        private readonly List<EffectBase> staticEffects = new();

        public void Start()
        {
            foreach (EffectLimit effectsLimit in this.effectsLimits)
            {
                this.maxPerEffect.Add(effectsLimit.effect.name, effectsLimit.max);
                this.currentEffects.Add(effectsLimit.effect.name, new List<GameObject>());
            }
        }

        public void Update()
        {
            foreach (WorldEffect worldEffect in this.staticEffects.OfType<WorldEffect>())
                worldEffect.CheckRules();
        }

        public void LateUpdate()
        {
        }

        private GameObject CheckMaxAndReturn(GameObject toCheck, int manualMax = -1)
        {
            string objectName = toCheck.name.Replace("(Clone)", "");

            if (manualMax == -1)
            {
                if (!this.maxPerEffect.ContainsKey(objectName))
                    return null;

                if (this.maxPerEffect[objectName] == 0)
                    return null;

                if (this.currentEffects[objectName].Count < this.maxPerEffect[objectName]) return null;
            }
            else
            {
                if (!this.currentEffects.ContainsKey(objectName))
                    return null;

                if (manualMax == 0)
                    return null;

                if (this.currentEffects[objectName].Count < manualMax)
                    return null;
            }

            GameObject toReturn = this.currentEffects[objectName][0];
            this.currentEffects[objectName].RemoveAt(0);
            this.currentEffects[objectName].Add(toReturn);

            toReturn.SetActive(false);
            toReturn.SetActive(true);

            return toReturn;
        }

        private GameObject CheckMaxAndReturn(string toCheck, int manualMax = -1)
        {
            string objectName = toCheck.Replace("(Clone)", "");

            if (manualMax == -1)
            {
                if (!this.maxPerEffect.ContainsKey(objectName))
                    return null;

                if (this.maxPerEffect[objectName] == 0)
                    return null;

                if (this.currentEffects[objectName].Count < this.maxPerEffect[objectName]) return null;
            }
            else
            {
                if (!this.currentEffects.ContainsKey(objectName))
                    return null;

                if (manualMax == 0)
                    return null;

                if (this.currentEffects[objectName].Count < manualMax)
                    return null;
            }

            GameObject toReturn = this.currentEffects[objectName][0];
            this.currentEffects[objectName].RemoveAt(0);
            this.currentEffects[objectName].Add(toReturn);

            toReturn.SetActive(false);
            toReturn.SetActive(true);

            return toReturn;
        }


        public void RegisterStaticEffect(EffectBase effectBase)
        {
            this.staticEffects.Add(effectBase);

            if (effectBase is WorldEffect worldEffect)
                worldEffect.CheckRules();
        }

        public void UnregisterStaticEffect(EffectBase effectBase) => this.staticEffects.Remove(effectBase);
    }

    [Serializable]
    internal struct EffectLimit
    {
        [SerializeField, AssetSelector(Paths = "Assets/Prefabs/VFX"), AssetsOnly]
        public GameObject effect;

        [SerializeField, Min(0), Tooltip("Maximum allowed instances of the effect prefab. 0 = No limit")]
        public int max;
    }
}
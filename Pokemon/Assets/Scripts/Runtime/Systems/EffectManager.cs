using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Runtime.Systems.PersistantRunner;
using Runtime.Systems.Pooling;
using Runtime.VFX;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.Systems
{
    [CreateAssetMenu, Serializable]
    public sealed class EffectManager : ScriptableObject, IFrameUpdate
    {
        private static EffectManager instance;

        [FoldoutGroup("Common")] [BoxGroup("Common/Blood")] [SerializeField, Min(0)]
        private int maxBloodEffectInstances;

        [BoxGroup("Common/Blood")] [SerializeField, AssetsOnly, AssetSelector(Paths = "Assets/Prefabs/VFX")]
        private List<GameObject> commonBloodHitEffects;

        [BoxGroup("Common/Dents")] [SerializeField, Min(0)]
        private int maxDentEffectInstances;

        [BoxGroup("Common/Dents")] [SerializeField, AssetsOnly, AssetSelector(Paths = "Assets/Prefabs/VFX")]
        private List<GameObject> commonDentHitEffects;

        [SerializeField] private List<EffectLimit> effectsLimits = new();

        private static readonly Dictionary<string, int> MaxPerEffect = new();
        private static readonly Dictionary<string, List<GameObject>> CurrentEffects = new();
        private static readonly List<EffectBase> StaticEffects = new();

        private void OnValidate()
        {
            this.commonBloodHitEffects = this.commonBloodHitEffects
                .Where(e => e != null)
                .ToList();
            this.commonDentHitEffects = this.commonDentHitEffects
                .Where(e => e != null)
                .ToList();
        }

        private void OnDisable() => instance = null;

        public void Start()
        {
        }

        public void Update()
        {
            if (instance == null)
            {
                instance = this;

                MaxPerEffect.Clear();
                CurrentEffects.Clear();

                foreach (EffectLimit effect in this.effectsLimits.Where(effect => effect.effect != null))
                {
                    MaxPerEffect.Add(effect.effect.name, effect.max);
                    CurrentEffects.Add(effect.effect.name, new List<GameObject>());
                }
            }

            foreach (EffectBase staticEffect in StaticEffects)
                CheckStaticEffect(staticEffect);
        }

        public void LateUpdate()
        {
        }

        public static bool instanceReady => instance != null;

        private static GameObject CheckMaxAndReturn(GameObject toCheck, int manualMax = -1)
        {
            string objectName = toCheck.name.Replace("(Clone)", "");

            if (manualMax == -1)
            {
                if (!MaxPerEffect.ContainsKey(objectName))
                    return null;

                if (MaxPerEffect[objectName] == 0)
                    return null;

                if (CurrentEffects[objectName].Count < MaxPerEffect[objectName]) return null;
            }
            else
            {
                if (!CurrentEffects.ContainsKey(objectName))
                    return null;

                if (manualMax == 0)
                    return null;

                if (CurrentEffects[objectName].Count < manualMax)
                    return null;
            }

            GameObject toReturn = CurrentEffects[objectName][0];
            CurrentEffects[objectName].RemoveAt(0);
            CurrentEffects[objectName].Add(toReturn);

            toReturn.SetActive(false);
            toReturn.SetActive(true);

            return toReturn;
        }

        private static GameObject CheckMaxAndReturn(string toCheck, int manualMax = -1)
        {
            string objectName = toCheck.Replace("(Clone)", "");

            if (manualMax == -1)
            {
                if (!MaxPerEffect.ContainsKey(objectName))
                    return null;

                if (MaxPerEffect[objectName] == 0)
                    return null;

                if (CurrentEffects[objectName].Count < MaxPerEffect[objectName]) return null;
            }
            else
            {
                if (!CurrentEffects.ContainsKey(objectName))
                    return null;

                if (manualMax == 0)
                    return null;

                if (CurrentEffects[objectName].Count < manualMax)
                    return null;
            }

            GameObject toReturn = CurrentEffects[objectName][0];
            CurrentEffects[objectName].RemoveAt(0);
            CurrentEffects[objectName].Add(toReturn);

            toReturn.SetActive(false);
            toReturn.SetActive(true);

            return toReturn;
        }

        private static void AddList(GameObject toAdd)
        {
            string objectName = toAdd.name.Replace("(Clone)", "");

            if (!CurrentEffects.ContainsKey(objectName))
                CurrentEffects.Add(objectName, new List<GameObject>());

            CurrentEffects[objectName].Add(toAdd);
        }

        private static void AddList(string objectName, GameObject toAdd)
        {
            if (!CurrentEffects.ContainsKey(objectName))
                CurrentEffects.Add(objectName, new List<GameObject>());

            CurrentEffects[objectName].Add(toAdd);
        }

        private static void CheckStaticEffect(EffectBase effectBase)
        {
            if (effectBase.CheckAllRules())
                effectBase.Play();
            else
                effectBase.Stop();
        }


        public static void RegisterStaticEffect(EffectBase effectBase)
        {
            StaticEffects.Add(effectBase);
            effectBase.EnableRules();

            CheckStaticEffect(effectBase);
        }

        public static void UnregisterStaticEffect(EffectBase effectBase)
        {
            effectBase.DisableRules();
            StaticEffects.Remove(effectBase);
        }

        #region Spawn Effects

        public static GameObject SpawnEffectAtLocation(GameObject prefabReference, Vector3 pos)
        {
            if (prefabReference == null)
                return null;

            if (CheckMaxAndReturn(prefabReference) is { } positive)
            {
                positive.transform.position = pos;

                return positive;
            }

            GameObject result = PoolManager.CreateAtPosition(prefabReference, pos, Quaternion.identity);
            AddList(result);
            return result;
        }

        public static GameObject SpawnEffectAtLocation(GameObject prefabReference, Vector3 pos, Quaternion rot)
        {
            if (prefabReference == null)
                return null;

            if (CheckMaxAndReturn(prefabReference) is { } positive)
            {
                positive.transform.position = pos;
                positive.transform.rotation = rot;

                return positive;
            }

            GameObject result = PoolManager.CreateAtPosition(prefabReference, pos, rot);
            AddList(result);
            return result;
        }

        public static GameObject SpawnEffectAndParentObject(GameObject prefabReference, Transform newParent)
        {
            if (prefabReference == null)
                return null;

            if (CheckMaxAndReturn(prefabReference) is { } positive)
            {
                positive.transform.parent = newParent;
                positive.transform.localPosition = Vector3.zero;
                positive.transform.localRotation = Quaternion.identity;

                return positive;
            }

            GameObject result = PoolManager.CreateAsChild(prefabReference, newParent);
            AddList(result);
            return result;
        }

        public static GameObject SpawnEffectAndSetUp(GameObject prefabReference, Vector3 pos, Vector3 up)
        {
            if (prefabReference == null)
                return null;

            Quaternion rotation = Quaternion.LookRotation(up);
            if (CheckMaxAndReturn(prefabReference) is { } positive)
            {
                positive.transform.position = pos;
                positive.transform.rotation = rotation;

                return positive;
            }

            GameObject result = PoolManager.CreateAtPosition(prefabReference, pos, rotation);
            AddList(result);
            return result;
        }

        public static GameObject SpawnEffectAtTransform(GameObject prefabReference, Transform transform)
        {
            if (prefabReference == null)
                return null;

            if (CheckMaxAndReturn(prefabReference) is { } positive)
            {
                positive.transform.position = transform.position;
                positive.transform.rotation = transform.rotation;

                return positive;
            }

            GameObject result = PoolManager.CreateAtTransform(prefabReference, transform);
            AddList(result);
            return result;
        }

        #endregion

        #region Affect Effects

        public static void DelayTrigger(ParticleSystem system, float delayTime)
        {
        }

        public static Tweener[] FadeOverTime(ParticleSystem system, float endValue, float time, bool disableOnDone,
            AnimationCurve curve, Action onComplete = null)
        {
            return system.GetComponent<ParticleSystemRenderer>()
                .materials.Select(mat => mat.DOFade(endValue, time)
                    .SetEase(curve)
                    .SetLink(system.gameObject, LinkBehaviour.KillOnDisable)
                    .OnComplete(() =>
                    {
                        onComplete?.Invoke();

                        if (disableOnDone)
                            system.gameObject.SetActive(false);
                    }))
                .Cast<Tweener>().ToArray();
        }

        public static void PauseEffect(ParticleSystem system)
        {
            if (system == null)
                return;

            system.Pause(true);
            GameObject gameObject = system.gameObject;
            string objectName = gameObject.name.Replace("(Clone)", "");
            CurrentEffects[objectName].Remove(gameObject);
            for (int i = 0; i < CurrentEffects[objectName].Count; i++)
            {
                if (!CurrentEffects[objectName][i].GetComponent<ParticleSystem>().isPaused)
                    continue;

                CurrentEffects[objectName].Insert(i, gameObject);
            }
        }

        public static void ResumeEffect(ParticleSystem system)
        {
            if (system == null)
                return;

            system.Play(true);
            GameObject gameObject = system.gameObject;
            string objectName = gameObject.name.Replace("(Clone)", "");
            CurrentEffects[objectName].Remove(gameObject);
            for (int i = 0; i < CurrentEffects[objectName].Count; i++)
            {
                if (CurrentEffects[objectName][i].GetComponent<ParticleSystem>().isPaused)
                    continue;

                CurrentEffects[objectName].Insert(i, gameObject);
            }
        }

        #endregion

        #region Get Effect

        public static EffectBase GetFirstStaticEffect(string effectName)
        {
            foreach (EffectBase staticEffect in StaticEffects)
            {
                if (staticEffect.EffectName().Equals(effectName))
                    return staticEffect;
            }

            return null;
        }

        public static EffectBase[] GetAllStaticEffects(string effectName)
        {
            List<EffectBase> result = new();
            foreach (EffectBase staticEffect in StaticEffects)
            {
                if (staticEffect.EffectName().Equals(effectName))
                    result.Add(staticEffect);
            }

            return result.ToArray();
        }

        #endregion
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
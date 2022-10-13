using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.ScriptableVariables.Objects;
using Runtime.Systems.PersistantRunner;
using Runtime.VFX;
using Runtime.VFX.Reuseable;
using Runtime.VFX.Scene;
using Runtime.VFX.SingleUSe;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.Systems
{
    [CreateAssetMenu, Serializable]
    public sealed class EffectManager : ScriptableObject, IFrameUpdate
    {
        [SerializeField, Required] private TransformVariable playerTransform;
        [SerializeField] private List<EffectLimit> effectsLimits = new();

        private readonly Dictionary<Type, SceneEffectHolder> sceneEffects = new();
        private readonly Dictionary<Type, SingleUseEffectHolder> singleUseEffects = new();
        private readonly Dictionary<Type, ReuseableEffect> reuseableEffects = new();

        #region IFrameUpdate

        public void Start()
        {
            foreach (EffectLimit effectsLimit in this.effectsLimits)
            {
                switch (effectsLimit.effect)
                {
                    case SceneEffect sceneEffect:
                        this.sceneEffects.Add(sceneEffect.GetType(),
                            new SceneEffectHolder(effectsLimit.max, sceneEffect));
                        break;
                    case SingleUseEffect singleUseEffect:
                        this.singleUseEffects.Add(singleUseEffect.GetType(),
                            new SingleUseEffectHolder(effectsLimit.max, singleUseEffect));
                        break;
                }
            }
        }

        public void Update()
        {
            foreach (SceneEffectHolder sceneEffectHolder in this.sceneEffects.Values)
                sceneEffectHolder.UpdateEffects();
        }

        public void LateUpdate()
        {
            foreach (SceneEffectHolder sceneEffectHolder in this.sceneEffects.Values)
                sceneEffectHolder.UpdateActives();

            foreach (SingleUseEffectHolder singleUseEffectHolder in this.singleUseEffects.Values)
                singleUseEffectHolder.UpdateActives();
        }

        #endregion

        #region In

        public void RegisterEffect(EffectBase effectBase)
        {
            switch (effectBase)
            {
                case SceneEffect sceneEffect:
                {
                    Type type = sceneEffect.GetType();
                    if (!this.sceneEffects.ContainsKey(type))
                    {
                        EffectLimit limit = this.effectsLimits.FirstOrDefault(e => e.effect.GetType() == type);
                        this.sceneEffects.Add(type, new SceneEffectHolder(limit?.max ?? 0, sceneEffect));
                    }

                    this.sceneEffects[type].Add(sceneEffect);
                    sceneEffect.CheckRules();
                    break;
                }
                case SingleUseEffect singleUseEffect:
                {
                    Type type = singleUseEffect.GetType();
                    if (!this.singleUseEffects.ContainsKey(type))
                    {
                        EffectLimit limit = this.effectsLimits.FirstOrDefault(e => e.effect.GetType() == type);
                        this.singleUseEffects.Add(type, new SingleUseEffectHolder(limit?.max ?? 0, singleUseEffect));
                    }

                    this.singleUseEffects[type].Add(singleUseEffect);
                    break;
                }
                case ReuseableEffect reuseableEffect:
                {
                    this.reuseableEffects.Add(reuseableEffect.GetType(), reuseableEffect);
                    break;
                }
            }
        }

        public void UnregisterEffect(EffectBase effectBase)
        {
            switch (effectBase)
            {
                case SceneEffect sceneEffect:
                    this.sceneEffects[sceneEffect.GetType()].Remove(sceneEffect);
                    break;
                case SingleUseEffect singleUseEffect:
                    this.singleUseEffects[singleUseEffect.GetType()].Remove(singleUseEffect);
                    break;
                case ReuseableEffect reuseableEffect:
                    this.reuseableEffects.Remove(reuseableEffect.GetType());
                    break;
            }
        }

        #region Reuseable

        public void PlayOne(ReuseableEffect reuseableEffect, Vector3 position)
        {
            if (reuseableEffect is null) return;

            Type type = reuseableEffect.GetType();
            if (!this.reuseableEffects.ContainsKey(type))
                this.InstantiateNew(reuseableEffect);

            this.reuseableEffects[type]
                .PlayOne(this.playerTransform.Position, position, Quaternion.identity);
        }

        public void PlayOne(ReuseableEffect reuseableEffect, Vector3 position, Quaternion rotation)
        {
            if (reuseableEffect is null) return;

            Type type = reuseableEffect.GetType();
            if (!this.reuseableEffects.ContainsKey(type))
                this.InstantiateNew(reuseableEffect);

            this.reuseableEffects[type]
                .PlayOne(this.playerTransform.Position, position, rotation);
        }

        public void PlayOne(ReuseableEffect reuseableEffect, Vector3 position, Vector3 forwardDirection)
        {
            if (reuseableEffect is null) return;

            Type type = reuseableEffect.GetType();
            if (!this.reuseableEffects.ContainsKey(type))
                this.InstantiateNew(reuseableEffect);

            this.reuseableEffects[type]
                .PlayOne(this.playerTransform.Position, position, Quaternion.LookRotation(forwardDirection));
        }

        #endregion

        #region Single Use

        public void PlayAtLocation(SingleUseEffect singleUseEffect, Vector3 position)
        {
            if (singleUseEffect is null) return;

            Type type = singleUseEffect.GetType();
            if (!this.singleUseEffects.ContainsKey(type))
                this.InstantiateNew(singleUseEffect);

            SingleUseEffect selected = this.singleUseEffects[type].TryGetEffect();

            selected ??= this.InstantiateNew(singleUseEffect);

            selected.transform.position = position;
            selected.Play(this.playerTransform.Position);
            this.singleUseEffects[type].Switch(selected);
        }

        public void PlayAtLocationAndRotation(SingleUseEffect singleUseEffect, Vector3 position, Quaternion rotation)
        {
            if (singleUseEffect is null) return;

            Type type = singleUseEffect.GetType();
            if (!this.singleUseEffects.ContainsKey(type))
                this.InstantiateNew(singleUseEffect);

            SingleUseEffect selected = this.singleUseEffects[type].TryGetEffect();

            selected ??= this.InstantiateNew(singleUseEffect);
            Transform selectedTransform = selected.transform;
            selectedTransform.position = position;
            selectedTransform.rotation = rotation;
            selected.Play(this.playerTransform.Position);
            this.singleUseEffects[type].Switch(selected);
        }

        public void PlayAtLocationAndDirection(SingleUseEffect singleUseEffect, Vector3 position, Vector3 direction)
        {
            if (singleUseEffect is null) return;

            Type type = singleUseEffect.GetType();
            if (!this.singleUseEffects.ContainsKey(type))
                this.InstantiateNew(singleUseEffect);

            SingleUseEffect selected = this.singleUseEffects[type].TryGetEffect();

            selected ??= this.InstantiateNew(singleUseEffect);
            Transform selectedTransform = selected.transform;
            selectedTransform.position = position;
            selectedTransform.rotation = Quaternion.LookRotation(direction);
            selected.Play(this.playerTransform.Position);
            this.singleUseEffects[type].Switch(selected);
        }

        public void PlayAtTransform(SingleUseEffect singleUseEffect, Transform parent)
        {
            if (singleUseEffect is null) return;

            Type type = singleUseEffect.GetType();
            if (!this.singleUseEffects.ContainsKey(type))
                this.InstantiateNew(singleUseEffect);

            SingleUseEffect selected = this.singleUseEffects[type].TryGetEffect();

            selected ??= this.InstantiateNew(singleUseEffect);
            Transform selectedTransform = selected.transform;
            selectedTransform.parent = parent;
            selectedTransform.position = parent.position;
            selectedTransform.rotation = parent.rotation;
            selected.Play(this.playerTransform.Position);
            this.singleUseEffects[type].Switch(selected);
        }

        #endregion

        #endregion


        #region Internal

        private T InstantiateNew<T>(T effect) where T : EffectBase
        {
            T effectBase = Instantiate(effect);

            effectBase.gameObject.SetActive(true);

            return effectBase;
        }

        #endregion
    }

    [Serializable]
    internal class EffectLimit
    {
        [SerializeField, AssetSelector(Paths = "Assets/Prefabs/VFX"), AssetsOnly]
        public EffectBase effect;

        [SerializeField, Min(0), Tooltip("Maximum allowed instances of the effect prefab. 0 = No limit")]
        public int max;
    }
}
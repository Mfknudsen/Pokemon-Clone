#region Packages

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Runtime.Systems;
using Runtime.VFX.Profiles;
using Runtime.VFX.Rules;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.VFX
{
    public sealed class EffectBase : MonoBehaviour
    {
        private string effectCallEffectName;
        private bool active, callRunning;
        private Coroutine managerCall;

        [SerializeField, Required] private EffectManager effectManager;
        
        [SerializeField, Required] private EffectProfile effectProfile;

        [SerializeField, Required]
        private ParticleSystem[] systems;

        [SerializeReference, FoldoutGroup("Rules"), TableList]
        private List<EffectRule> currentRules = new();

        #if UNITY_EDITOR
        [ShowInInspector, HorizontalGroup("Rules/Setup"), ValueDropdown("AllRuleTypes")]
        private Type createType = typeof(ScriptableBoolRule);

        [HorizontalGroup("Rules/Setup"), Button("Create New Rule")]
        private void CreateRule() =>
            this.currentRules.Add(Activator.CreateInstance(this.createType) as EffectRule);

        // ReSharper disable once UnusedMember.Local
        private IEnumerable<Type> AllRuleTypes() =>
            (from Type type in System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
             where type.IsSubclassOf(typeof(EffectRule)) && type != typeof(TriggerRule) 
             select type)
            .ToArray();
        #endif

        private void Start() => this.effectProfile.ObjectStart(this.systems);

        private void OnEnable()
        {
            
            if (this.callRunning)
                this.managerCall = this.StartCoroutine(this.StartCall(this.ContinueCall));
        }

        private void OnDisable()
        {
            if (this.managerCall != null)
                this.StopCoroutine(this.managerCall);

        }

        private void OnTriggerEnter(Collider other)
        {
            foreach (TriggerRule triggerRule in this.currentRules
                .OfType<TriggerRule>())
                triggerRule.Add(other);
        }

        private void OnTriggerExit(Collider other)
        {
            foreach (TriggerRule triggerRule in this.currentRules
                .OfType<TriggerRule>())
                triggerRule.Remove(other);
        }

        public string EffectName() => this.effectCallEffectName;

        public void Play()
        {
            if (this.active) return;

            if (this.managerCall != null)
                this.StopCoroutine(this.managerCall);

            this.managerCall = this.StartCoroutine(
                this.StartCall(() => this.effectProfile.Startup(this.systems)));

            this.active = true;
        }

        public void Stop()
        {
            if (!this.active) return;

            if (this.managerCall != null)
                this.StopCoroutine(this.managerCall);

            this.managerCall = this.StartCoroutine(
                this.StartCall(() => this.effectProfile.Shutdown(this.systems)));

            this.active = false;
        }

        public void EnableRules()
        {
            foreach (EffectRule effectRule in this.currentRules)
                effectRule.Enable();
        }

        public void DisableRules()
        {
            foreach (EffectRule effectRule in this.currentRules)
                effectRule.Disable();
        }

        public bool CheckAllRules() => this.currentRules.All(rule => rule.CheckRule(this.gameObject));

        private void ContinueCall()
        {
            if (this.active)
                this.effectProfile.Startup(this.systems);
            else
                this.effectProfile.Shutdown(this.systems);
        }

        private IEnumerator StartCall(Action onTrue)
        {
            this.callRunning = true;
            yield return new WaitUntil(() => !this.effectProfile.IsRunning);

            onTrue?.Invoke();
            this.callRunning = false;
        }
    }
}

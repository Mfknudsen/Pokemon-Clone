#region Packages

using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

#endregion

namespace Runtime.Systems
{
    public class ManagerUpdater : MonoBehaviour
    {
        private static ManagerUpdater _instance;
        [ShowInInspector, ReadOnly] private readonly List<Manager> toUpdate = new();

        #region Build In States

        private void Start()
        {
            if (_instance is not null)
                Destroy(gameObject);

            _instance = this;

            foreach (string guid in AssetDatabase.FindAssets("t:" + nameof(Manager)))
                this.toUpdate.Add(AssetDatabase.LoadAssetAtPath<Manager>(AssetDatabase.GUIDToAssetPath(guid)));

            this.toUpdate.ForEach(m => StartCoroutine(m.StartManager()));
        }

        #endregion

        private void Update() => this.toUpdate.ForEach(m =>
        {
            if (m.GetReady())
                m.UpdateManager();
        });

        private void FixedUpdate() => this.toUpdate.ForEach(m =>
        {
            if (m.GetReady())
                m.FixedUpdateManager();
        });
    }
}
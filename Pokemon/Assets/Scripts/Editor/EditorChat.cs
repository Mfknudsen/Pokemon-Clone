#region Packages

using Runtime.Communication;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

#endregion

namespace Editor
{
    [CustomEditor(typeof(Chat), true)]
    public sealed class EditorChat : OdinEditor
    {
        #region Values

        private Chat script;

        private GUIStyle textStyle;

        #endregion

        protected override void OnEnable() =>
            this.script = this.target as Chat;

        public override void OnInspectorGUI()
        {
            this.ShowList();

            base.OnInspectorGUI();
        }

        private void ShowList()
        {
            if (GUILayout.Button("Create New"))
                this.script.CreateNew();

            for (int i = 0; i < this.script.GetListCount; i++)
            {
                EditorGUILayout.BeginHorizontal("Box");

                //Hide after and time till next
                EditorGUILayout.BeginVertical(GUILayout.Width(30));

                this.script.SetTimeToNextByIndex(i,
                    EditorGUILayout.FloatField(this.script.GetTimeToNextByIndex(i)));

                EditorGUILayout.EndVertical();

                this.script.SetTextByIndex(i,
                    EditorGUILayout.TextField(this.script.GetTextByIndex(i), GUILayout.Height(40)));

                if (GUILayout.Button("X", GUILayout.Width(20)))
                    this.script.DeleteByIndex(i);

                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
﻿#region Packages

using Runtime.Communication;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

#endregion

namespace Runtime.UI.Communication
{
    public class TextField : MonoBehaviour
    {
        #region Values

        public static TextField instance;

        [SerializeField, Required] private ChatManager chatManager;

        [SerializeField, Required] private ResponseField responseField;

        [SerializeField, Required] private TextMeshProUGUI text;

        #endregion

        #region Build In States

        private void Start()
        {
            this.Hide();
            this.text.text = "";
        }

        #endregion

        #region Getters

        public TextMeshProUGUI GetText =>
            this.text;

        public ResponseField GetResponseField => this.responseField;

        #endregion

        #region In

        public void MakeCurrent()
        {
            if (instance != null)
            {
                this.text.text = instance.GetText.text;
                instance.Hide();
            }

            if (this.chatManager.GetShow())
                this.Show();

            instance = this;
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);

            this.text.text = "";
        }

        public void Show() =>
            this.gameObject.SetActive(true);

        #endregion
    }
}
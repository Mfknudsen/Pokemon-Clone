#region Packages

using System.Linq;
using Mfknudsen.Player.UI_Book;
using Mfknudsen.UI.Book.Interfaces;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Mfknudsen.UI.Book.Button
{
    [AddComponentMenu("Mfknudsen/BookUI/Book Button")]
    public class BookButton : UnityEngine.UI.Button, ICustomGUIElement
    {
        #region Values

        [SerializeField, HideInInspector] private BookTurn bookTurn;

        #endregion

        protected override void Awake()
        {
            base.Awake();

            Navigation nav = navigation;
            nav.mode = Navigation.Mode.None;
            navigation = nav;
        }

        #region Getters

        public BookTurn GetBookTurn()
        {
            return bookTurn;
        }

        #endregion

        #region Setters

        public void SetBookTurn(BookTurn set)
        {
            bookTurn = set;
        }

        #endregion

        #region In

        public void SetPressedState(string buttonState)
        {
            SelectionState[] states =
            {
                SelectionState.Normal,
                SelectionState.Disabled,
                SelectionState.Highlighted,
                SelectionState.Pressed,
                SelectionState.Selected
            };

            SelectionState state =
                states.FirstOrDefault(selectionState => selectionState.ToString().Equals(buttonState));

            base.DoStateTransition(state, false);

            if (state == SelectionState.Pressed)
                Invoke(nameof(TriggerOnClick), 0.1f);
        }

        #endregion

        #region Internal

        private void TriggerOnClick()
        {
            onClick.Invoke();
        }

        #endregion
    }
}
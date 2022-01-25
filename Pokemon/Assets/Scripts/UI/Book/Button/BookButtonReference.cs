#region Packages

using Mfknudsen.Player.UI_Book;
using Mfknudsen.UI.Book.Interfaces;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Mfknudsen.UI.Book.Button
{
    [AddComponentMenu("Mfknudsen/BookUI/Book Button Reference")]
    public class BookButtonReference : UnityEngine.UI.Button, ICustomGUIElementReference
    {
        #region Values

        private BookButton bookButton;
        private readonly Color color = new Color(0, 0, 0, 0);

        #endregion

        protected override void Awake()
        {
            Navigation nav = navigation;
            nav.mode = Navigation.Mode.None;
            navigation = nav;

            ColorBlock colorBlock = colors;

            colorBlock.normalColor = color;
            colorBlock.highlightedColor = color;
            colorBlock.pressedColor = color;
            colorBlock.disabledColor = color;
            colorBlock.selectedColor = color;

            colors = colorBlock;

            base.Awake();
        }

        #region In

        public void Setup(ICustomGUIElement element)
        {
            if(element is BookButton button)
                bookButton = button;
        }

        #endregion

        #region Internal

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);

            if (bookButton == null) return;

            if (state == SelectionState.Pressed)
                UIBook.instance.Effect(bookButton.GetBookTurn());

            bookButton.SetPressedState(state.ToString());
        }

        #endregion
    }
}
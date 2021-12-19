#region Packages

using Mfknudsen.Player.UI_Book;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Mfknudsen.UI.Book
{
    [AddComponentMenu("Mfknudsen/BookUI/Book Button Reference")]
    public class BookButtonReference : Button
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

        public void Setup(BookButton set)
        {
            bookButton = set;
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
#region Packages

using Mfknudsen.Player.UI_Book;
using Mfknudsen.UI.Book;
using UnityEditor;

#endregion

namespace Mfknudsen.Editor.UI
{
    [CustomEditor(typeof(BookButton))]
    public class EditorBookButton : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            BookButton bookButton = target as BookButton;

            if (bookButton != null)
            {
                BookTurn turn = (BookTurn) EditorGUILayout.EnumPopup("Book Turn Enums", bookButton.GetBookTurn());
                if (turn != bookButton.GetBookTurn())
                {
                    bookButton.SetBookTurn(turn);
                    EditorUtility.SetDirty(bookButton);
                }

                EditorGUILayout.Space(10);
            }

            base.OnInspectorGUI();
        }
    }
}
using Mfknudsen.UI.Book.Interfaces;
using UnityEngine;

namespace Mfknudsen.UI.Book.TextInputField
{
    public class BookTextInputFieldReference : MonoBehaviour,ICustomGUIElementReference
    {
        private BookTextInputField bookField;
        
        public void Setup(ICustomGUIElement element)
        {
            if (element is BookTextInputField field)
                bookField = field;
        }
    }
}

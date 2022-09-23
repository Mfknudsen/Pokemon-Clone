using Runtime.UI.Book.Interfaces;
using UnityEngine;

namespace Runtime.UI.Book.TextInputField
{
    public class BookTextInputFieldReference : MonoBehaviour,ICustomGUIElementReference
    {
        private BookTextInputField bookField;
        
        public void Setup(ICustomGUIElement element)
        {
            if (element is BookTextInputField field) this.bookField = field;
        }
    }
}

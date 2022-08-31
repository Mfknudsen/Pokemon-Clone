using Runtime.UI.Book.Interfaces;
using Runtime.UI.Book.TextInputField;
using UnityEngine;

namespace Runtime.UI.Book.Slider
{
    public class BookSliderReference : MonoBehaviour, ICustomGUIElementReference
    {
        private BookTextInputField textInputField;
        
        public void Setup(ICustomGUIElement element)
        {
            throw new System.NotImplementedException();
        }
    }
}

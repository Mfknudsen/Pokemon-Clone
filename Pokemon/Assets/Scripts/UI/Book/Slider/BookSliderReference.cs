using Mfknudsen.UI.Book.Interfaces;
using Mfknudsen.UI.Book.TextInputField;
using UnityEngine;

namespace Mfknudsen.UI.Book.Slider
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

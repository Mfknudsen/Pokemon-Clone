#region Packages

using Runtime.Pokémon;
using Runtime.Systems;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

#endregion

namespace Runtime._Debug
{
    public class PokemonPlaceholder : MonoBehaviour
    {
        [SerializeField, Required] private CameraManager cameraManager;
        [SerializeField] private string pokmemonName = "";
        [SerializeField, Required] private TextMeshPro textMesh;

        private void SetText(string t)
        {
            this.pokmemonName = t;
            this.textMesh.text = t + "\nPlaceholder";
        }

        public void CheckPlaceholder(Pokemon pokemon)
        {
            this.SetText(pokemon.GetName());

            Vector3 targetVector = this.transform.GetChild(0).transform.position +
                                   (this.transform.GetChild(0).transform.position - this.cameraManager.GetCurrentCamera().transform.position);
            targetVector = new Vector3(
                targetVector.x, this.transform.GetChild(0).transform.position.y,
                targetVector.z);
            this.transform.GetChild(0).transform.LookAt(targetVector);
        }
    }
}
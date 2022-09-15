#region Packages

using Runtime.Player.Camera;
using Runtime.Pokémon;
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
        [SerializeField] private TextMeshPro textMesh;

        private void SetText(string t)
        {
            if (textMesh == null) return;

            pokmemonName = t;
            textMesh.text = t + "\nPlaceholder";
        }

        public void CheckPlaceholder(Pokemon pokemon, GameObject spawnedObj)
        {
            PokemonPlaceholder placeholder = spawnedObj.GetComponent<PokemonPlaceholder>();

            if (placeholder == null)
                return;

            placeholder.SetText(pokemon.GetName());

            Vector3 targetVector = spawnedObj.transform.GetChild(0).transform.position +
                                   (spawnedObj.transform.GetChild(0).transform.position -
                                    cameraManager.GetCurrentCamera().transform.position);
            targetVector = new Vector3(
                targetVector.x,
                spawnedObj.transform.GetChild(0).transform.position.y,
                targetVector.z);
            spawnedObj.transform.GetChild(0).transform.LookAt(targetVector);
        }
    }
}
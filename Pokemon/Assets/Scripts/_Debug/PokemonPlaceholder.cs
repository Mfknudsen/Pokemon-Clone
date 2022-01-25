#region Packages

using Mfknudsen.Player.Camera;
using Mfknudsen.Pokémon;
using UnityEngine;

#endregion

namespace Mfknudsen._Debug
{
    public class PokemonPlaceholder : MonoBehaviour
    {
        [SerializeField] private string pokmemonName = "";
        [SerializeField] private TextMesh textMesh = null;

        public void SetText(string t)
        {
            if (textMesh != null)
            {
                pokmemonName = t;
                textMesh.text = t + "\nPlaceholder";
            }
        }

        public static void CheckPlaceholder(Pokemon pokemon, GameObject spawnedObj)
        {
            PokemonPlaceholder placeholder = spawnedObj.GetComponent<PokemonPlaceholder>();

            if (placeholder == null)
                return;

            placeholder.SetText(pokemon.GetName());

            Vector3 targetVector = spawnedObj.transform.GetChild(0).transform.position +
                                   (spawnedObj.transform.GetChild(0).transform.position -
                                    CameraManager.instance.GetCurrentCamera().transform.position);
            targetVector = new Vector3(
                targetVector.x, 
                spawnedObj.transform.GetChild(0).transform.position.y,
                targetVector.z);
            spawnedObj.transform.GetChild(0).transform.LookAt(targetVector);
        }
    }
}
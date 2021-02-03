using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonPlaceholder : MonoBehaviour
{
    [SerializeField] private string pokmemon = "";
    [SerializeField] private TextMesh textMesh = null;

    public void SetText(string t)
    {
        if (textMesh != null)
        {
            pokmemon = t;
            textMesh.text = t + "\nPlaceholder";
        }
    }

    public static void CheckPlaceholder(Pokemon pokemon, GameObject spawnedObj)
    {
        PokemonPlaceholder placeholder = spawnedObj.GetComponent<PokemonPlaceholder>();

        if (placeholder == null)
            return;

        placeholder.SetText(pokemon.GetName());

        Vector3 targetVector = spawnedObj.transform.GetChild(0).transform.position + (spawnedObj.transform.GetChild(0).transform.position - Camera.main.transform.position);
        targetVector = new Vector3(targetVector.x, spawnedObj.transform.GetChild(0).transform.position.y, targetVector.z);
        spawnedObj.transform.GetChild(0).transform.LookAt(targetVector);
    }
}

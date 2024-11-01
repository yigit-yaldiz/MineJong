using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniqueGames.Saving;
using UnityEngine;

[RequireComponent(typeof(SaveableEntity))]
public class FurnacesDataHolder : MonoBehaviour, ISaveable
{
    HashSet<int> _activatedFurnaces = new();

    public void AddActivatedFurnace(Furnace furnace)
    {
        _activatedFurnaces.Add(furnace.transform.GetSiblingIndex());
    }

    [System.Serializable]
    struct SaveData
    {
        public int[] ActivatedFurnaces;
    }

    public object CaptureState()
    {
        SaveData saveData = new();
        saveData.ActivatedFurnaces = _activatedFurnaces.ToArray();

        return saveData;
    }

    public void RestoreState(object state)
    {
        SaveData saveData = (SaveData)state;
        _activatedFurnaces = new(saveData.ActivatedFurnaces);

        foreach (int furnaceIndex in _activatedFurnaces)
        {
            transform.GetChild(furnaceIndex).GetComponent<Furnace>().ActivateFurnace();
        }
    }
}

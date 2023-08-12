using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
[CreateAssetMenu(menuName = "Tools/HdfSetTest")]
public class HdfSet : ScriptableObject
{
    public List<HdfDatabaseSet> hdfFiles;
    public void Reset()
    {
        hdfFiles = new()
        {
            //new() {path = "test" },
            //new() {path ="" },
        };
    }
}

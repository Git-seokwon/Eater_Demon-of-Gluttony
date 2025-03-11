using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveTestUI : MonoBehaviour
{

    public void DeleteSaveJson()
    {
        string path = Path.Combine(Application.dataPath, "Save.json");
        string pathmeta = Path.Combine(Application.dataPath, "Save.json.meta");

        if (File.Exists(path))
            System.IO.File.Delete(path);
        else
            Debug.Log("Already Deleted");

        if(File.Exists(pathmeta))
            System.IO.File.Delete(pathmeta);
        else
            Debug.Log("Already Deleted");
    }
}

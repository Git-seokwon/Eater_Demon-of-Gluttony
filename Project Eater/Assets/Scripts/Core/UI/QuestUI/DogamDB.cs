using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Quest/DogamDB")]
public class DogamDB : ScriptableObject
{
    [SerializeField] private List<DogamMonster> dogamMonsters;

    public IReadOnlyList<DogamMonster> DogamMonsters => dogamMonsters;

    public DogamMonster FindDogamMonstersBy(string codeName) => dogamMonsters.FirstOrDefault(x => x.CodeName == codeName);

#if UNITY_EDITOR
    [ContextMenu("FindDogamMonster")]
    private void FindDogamMonsters()
    {
        FindDogamMonstersBy<DogamMonster>();
    }

    private void FindDogamMonstersBy<T>() where T : DogamMonster
    {
        dogamMonsters = new List<DogamMonster>();

        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var quest = AssetDatabase.LoadAssetAtPath<T>(assetPath);

            if (quest.GetType() == typeof(T))
                dogamMonsters.Add(quest);

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
#endif 

}

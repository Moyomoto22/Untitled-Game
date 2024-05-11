using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassManager : MonoBehaviour
{
    public static ClassManager Instance { get; private set; }
    public List<Class> AllClasses;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ID ‚É‚æ‚Á‚ÄƒNƒ‰ƒX‚ðŒŸõ‚·‚é
    public Class GetClassByID(string id)
    {
        foreach (Class c in AllClasses)
        {
            if (c.ID == id)
            {
                return c;
            }
        }
        Debug.LogWarning("Skill with ID: " + id + " not found.");
        return null;
    }

    public Class GetClassByIndex(int index)
    {
        if(AllClasses.Count >= index)
        {
            return AllClasses[index];
        }
        return null;
    }
}
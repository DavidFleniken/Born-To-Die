using UnityEngine;
using System.Collections.Generic;

public class RefIDs : MonoBehaviour
{
    /* 
     * This script is not meant to be put on any game object, only contain static methods
     * 
     * Will contain a dictionary that stores string Ids as keys and game object ref as value
     */

    static Dictionary<string, GameObject> idDict = new Dictionary<string, GameObject>();

    // returns true if succeeded, false if id already taken
    public static bool addRef(string id, GameObject val)
    {
        // dont be case sensitive
        id = id.ToLower();
        if (idDict.ContainsKey(id))
        {
            Debug.LogError("ID \"" + id + "\" is already taken by: " + idDict[id]);
            return false;
        }

        idDict.Add(id, val);

        return true;
    }

    public static bool replaceRef(string id, GameObject val)
    {
        id = id.ToLower();
        if (!idDict.ContainsKey(id))
        {
            Debug.LogError("ID \"" + id + "\" was never defined.");
            return false;
        }
        idDict[id] = val;
        return true;
    }

    // Returns object on success, null if id doesnt exist
    public static GameObject getRef(string id)
    {
        id = id.ToLower();
        if (!idDict.ContainsKey(id))
        {
            Debug.LogError("ID \"" + id + "\" was never defined.");
            return null;
        }

        return idDict[id];
    }

}

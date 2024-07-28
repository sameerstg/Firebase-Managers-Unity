using System.Collections.Generic;
using UnityEngine;

public class FirestoreSample : MonoBehaviour
{
    public string collectionName = "MatchLogs";
    public string documentId = "Sample Document";
    [ContextMenu("Get Data")]
    public async void GetData()
    {
        var res = await FirestoreManager._instance.GetData(() => { }, collectionName);
        foreach (var item in res)
        {
            Debug.LogError(item.Key);
            Debug.LogError(item.Value);
        }
    }

    [ContextMenu("save")]
    public void Save()
    {
        _ = FirestoreManager._instance.SaveData(collectionName, documentId, new Dictionary<string, object>() { { "list", "asfasfas" } }, null);
    }
}

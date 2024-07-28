using UnityEngine;

public class RealtimeDatabaseSample : MonoBehaviour
{
    
    [ContextMenu("Save Data")]
    public void SaveData()
    {
        _ = RealtimeDatabaseManager._instance.SaveData(RealtimeDatabaseManager._instance.reference.Child("Games").Child("stg"), 241);
    }
    [ContextMenu("Get Data")]
    public void GetData()
    {
        _ = RealtimeDatabaseManager._instance.GetData(RealtimeDatabaseManager._instance.reference.Child("UserData").Child("afasfas"));
    }
    [ContextMenu("Delete Data")]
    public void DeleteData()
    {
        RealtimeDatabaseManager._instance.DeleteData(RealtimeDatabaseManager._instance.reference.Child("Games").Child("stg"));
    }
}

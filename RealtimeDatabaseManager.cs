using Firebase.Database;
using Firebase.Extensions;
using System.Threading.Tasks;
using UnityEngine;

public class RealtimeDatabaseManager : MonoBehaviour
{
    public static RealtimeDatabaseManager _instance;
    internal DatabaseReference reference;
    public bool log = true;
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(transform.gameObject);
            return;
        }
        _instance = this;
    }
    public void Initialize()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    public async Task<object> GetData(DatabaseReference reference)
    {
        object obj = null;
        await reference.GetValueAsync().ContinueWithOnMainThread(
                    task =>
                    {
                        if (task.IsFaulted)
                        {
                            Debug.LogError("error while getting data");
                        }
                        else if (task.IsCompletedSuccessfully)
                        {
                            DataSnapshot snapshot = task.Result;
                            //Debug.LogError(snapshot.Value);
                            if (snapshot.Value == null)
                            {
                                if (log)
                                    Debug.LogError("Data got but null");
                            }
                            else
                            {
                                obj = snapshot.Value;
                                if (log)
                                    Debug.LogError("Data got succesfully");

                            }
                        }
                        return obj;
                    }

                    );
        return obj;
    }
    public async Task<bool> SaveData(DatabaseReference reference, object data)
    {
        bool success = false;
        await reference.SetValueAsync(data).ContinueWithOnMainThread(x =>

        {
            success = x.IsCompletedSuccessfully;

            if (log && success)
                Debug.LogError("Data got succesfully");
            else
                Debug.LogError(x.Exception);
            return success;
        }

        );
        return success;
    }
    public async Task<bool> SaveData(DatabaseReference reference, string json)
    {
        bool success = false;
        await reference.SetRawJsonValueAsync(json).ContinueWithOnMainThread(x =>

        {
            success = x.IsCompletedSuccessfully;
            return success;
        }

        );
        return success;
    }
    public void DeleteData(DatabaseReference reference)
    {
        reference.RemoveValueAsync();
    }
    [ContextMenu("Save Data")]
    public void SaveData()
    {
        _ = SaveData(reference.Child("Games").Child("stg"), 241);
    }
    [ContextMenu("Get Data")]
    public void GetData()
    {
        _ = GetData(reference.Child("UserData").Child("afasfas"));
    }
    [ContextMenu("Delete Data")]
    public void DeleteData()
    {
        DeleteData(reference.Child("Games").Child("stg"));
    }




}

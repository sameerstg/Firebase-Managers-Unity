using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;
using System.Threading.Tasks;

public class FirestoreManager : MonoBehaviour
{
    public static FirestoreManager _instance;
    FirebaseFirestore db;
    public Dictionary<string, object> data;
    public Action onDataGet, onDataSave;
    public string collectionName = "MatchLogs";
    public string documentId = "Sample Document";
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this);
        _instance = this;
        data = new();
    }
    private void Start()
    {
        InitializeFirestore();
    }
    public async void InitializeFirestore()
    {
        //Firebase.AppOptions options = new Firebase.AppOptions();
        //options.ApiKey = "";
        //options.AppId = "";
        //options.ProjectId = "";
        //var app = Firebase.FirebaseApp.Create(options);

        //loginManager.message.text = "called";
        await Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread((task =>
        {

            Firebase.DependencyStatus dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {



                db = FirebaseFirestore.DefaultInstance;
                _ = GetData(() => { onDataGet?.Invoke(); }, collectionName, documentId);

            }
            else
            {

                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        }));

    }
    public async Task<Dictionary<string, object>> GetData(Action continuation, string collectionName, string documentId)
    {
        Dictionary<string, object> data = new();
        DocumentReference docRef = db.Collection(collectionName).Document(documentId);

        await docRef.GetSnapshotAsync().ContinueWithOnMainThread((task) =>
        {
            var snapshot = task.Result;
            Assert.IsNotNull(snapshot);
            if (snapshot.Exists)
            {
                data = snapshot.ToDictionary();
                foreach (var item in data)
                {
                    Debug.LogError(item.Key);
                    Debug.LogError(item.Value);
                }
                continuation();
            }
            else
            {
                continuation();
            }


        });
        return data;
    }
    /// <summary>
    /// Get Data Of All documents
    /// </summary>
    /// <param name="continuation"></param>
    /// <param name="collectionName"></param>
    /// <returns></returns>
    public async Task<Dictionary<string, Dictionary<string, object>>> GetData(Action continuation, string collectionName)
    {
        Dictionary<string, Dictionary<string, object>> data = new();
        Query docRef = db.Collection(collectionName);
        await docRef.GetSnapshotAsync().ContinueWithOnMainThread((task) =>
        {
            if (task != null)
            {
                QuerySnapshot snapshot = task.Result;

                foreach (var item in snapshot.Documents)
                {
                    var dic = item.ToDictionary();
                    data.Add(item.Id, dic);
                }
                continuation();
            }
            return data;

        });
        return data;
    }
    /// <summary>
    /// Returns particular key value
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="documentId"></param>
    /// <param name="key"></param>
    public async Task<object> GetData(Action continuation, string collectionName, string documentId, string key)
    {
        var data = await GetData(continuation, collectionName, documentId);
        if (!data.ContainsKey(key)) return null;
        return data[key];
    }
    [ContextMenu("Get Data")]
    public async void GetData()
    {
        _ = GetData(() => { }, collectionName);
    }

    [ContextMenu("save")]
    public void Save()
    {
        _ = SaveData(collectionName, documentId, "s1s", "214124", null);
    }
    public async Task<bool> SaveData(string collectionName, string documentId, string key, string value, Action continuation)
    {
        var data = await GetData(() =>
        {
        }, collectionName, documentId
        );

        Assert.IsNotNull(data);
        DocumentReference docRef = db.Collection(collectionName).Document(documentId);
        Debug.LogError(docRef);
        if (data.ContainsKey(key.ToString())) { data[key.ToString()] = value; }
        else data.Add(key.ToString(), value);
        await docRef.SetAsync(data).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {

                Debug.LogError("Added data .");
                continuation?.Invoke();
            }
            return task.IsCompletedSuccessfully;
        }
);
        return false;
    }


}

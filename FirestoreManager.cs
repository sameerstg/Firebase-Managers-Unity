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
    internal FirebaseFirestore db;
    public Action onDataGet, onDataSave;
   
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this);
    }
    public void Initialize()
    {
        db = FirebaseFirestore.DefaultInstance;
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
                continuation?.Invoke();
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
    public object ListenData(Action<object> continuation, string collectionName, string documentId, string key)
    {
        object data = null;
        db.Collection(collectionName).Document(documentId).Listen(x =>
        {
            var dict = x.ToDictionary();
            if (dict.ContainsKey(key))
            {
                data = dict[key];
                continuation(data);
            }
        });
        return data;
    }
    public async Task<string> GetDocumentId(Action continuation, string collectionName, string key, string value)
    {
        string docId = string.Empty;
        var querry = db.Collection(collectionName).Where(filter: Filter.EqualTo(key, value));
        var res = await querry.GetSnapshotAsync().ContinueWithOnMainThread(task =>
                {
                    QuerySnapshot capitalQuerySnapshot = task.Result;
                    if (capitalQuerySnapshot.Documents.ToList().Count > 0) docId = capitalQuerySnapshot.Documents.ToList()[0].Id;
                    return docId;
                    //foreach (DocumentSnapshot documentSnapshot in capitalQuerySnapshot.Documents)
                    //{
                    //    Debug.Log(String.Format("Document data for {0} document:", documentSnapshot.Id));
                    //    Dictionary<string, object> city = documentSnapshot.ToDictionary();
                    //    foreach (KeyValuePair<string, object> pair in city)
                    //    {
                    //        Debug.Log(String.Format("{0}: {1}", pair.Key, pair.Value));
                    //    }

                    //
                }
                );
        return docId;
    }
  
    public async Task<bool> SaveData(string collectionName, Dictionary<string, object> data, Action continuation)
    {

        Assert.IsNotNull(data);
        data.Add("Time Stamp", DateTime.Now);
        DocumentReference docRef = db.Collection(collectionName).Document(Mathf.Abs(data.GetHashCode()).ToString());
        await docRef.SetAsync(data).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                continuation?.Invoke();
            }
            return task.IsCompletedSuccessfully;
        }
);
        return false;
    }
    public async Task<bool> SaveData(string collectionName, string documentId, Dictionary<string, object> data, Action continuation)
    {

        Assert.IsNotNull(data);
        DocumentReference docRef = db.Collection(collectionName).Document(documentId);
        await docRef.SetAsync(data).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                continuation?.Invoke();
            }
            return task.IsCompletedSuccessfully;
        }
);
        return false;
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
                continuation?.Invoke();
            }
            return task.IsCompletedSuccessfully;
        }
);
        return false;
    }



}
using Firebase.Extensions;
using System;
using UnityEngine;
using UnityEngine.Events;

public class FirebaseInitializer : MonoBehaviour
{
    public static FirebaseInitializer _instance;
    public UnityEvent onFirebaseInitialized;
    public bool log = true;
    public bool isInitialized;
    public void Awake()
    {
        _instance = this;
        InitializeFirestore();
    }
    public async void InitializeFirestore()
    {
        // Firebase.AppOptions options = new Firebase.AppOptions();
        // options.ApiKey = "";
        
        // options.AppId = "";
        // options.ProjectId = "";
        // options.DatabaseUrl = new Uri("");
        // var app = Firebase.FirebaseApp.Create(options);
        await Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread((task =>
        {

            Firebase.DependencyStatus dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                if (log)
                    Debug.LogError("Firebase Initialized");
                onFirebaseInitialized?.Invoke();
                isInitialized = true;
            }
            else
            {
                if (log)
                    Debug.LogError(String.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        }));

    }
}

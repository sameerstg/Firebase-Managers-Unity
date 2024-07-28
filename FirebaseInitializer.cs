using Firebase;
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
    private FirebaseApp app;
    public string userId;
    public void Awake()
    {
        if(_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        InitializeFirestore();
    }
    public async void InitializeFirestore()
    {
        //AppOptions options = new AppOptions();
        //options.AppId = "";
        //options.ProjectId = "";
        //options.ApiKey = "";
        //var app = Firebase.FirebaseApp.Create(options);
        FirebaseApp.Create();
        await FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread((task =>
        {

            Firebase.DependencyStatus dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                if (log)
                    Debug.LogError("Firebase Initialized");
                onFirebaseInitialized?.Invoke();
                isInitialized = true;
                app = FirebaseApp.DefaultInstance;
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

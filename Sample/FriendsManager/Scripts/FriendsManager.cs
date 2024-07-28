using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FriendsManager : MonoBehaviour
{
    public static FriendsManager _instance;
    public FriendsData data;
    public FriendsPanel friendsPanel;

    private void Awake()
    {
        _instance = this;
    }
    private void OnEnable()
    {
        GetData();
    }
    private void Start()
    {
        FirestoreManager._instance.ListenData(OnDataChange, "FriendsData", AuthenticationManager._instance.user.UserId, "FriendsData");
    }

    private void OnDataChange(object data)
    {
        if (data != null)
        {
            this.data = JsonUtility.FromJson<FriendsData>((string)data);
            UpdateUI();
        }
    }

    public async Task<FriendsData> GetData(string userId)
    {

        var res = await FirestoreManager._instance.GetData(null, "FriendsData", userId, "FriendsData");
        if (res != null)
        {
            var data = JsonUtility.FromJson<FriendsData>((string)res);
            return data;
        }
        return null;

    }
    public async void GetData()
    {
        data = await GetData(AuthenticationManager._instance.user.UserId);
        UpdateUI();
    }
    public void UpdateUI()
    {
        Debug.LogError("data update");
        friendsPanel.UpdateUI(data);
    }
    public async void AcceptFriendRequest(string name)
    {
        name = name.Trim();
        if (AuthenticationManager._instance.user.DisplayName == name) return;
        if (data.friends.Contains(name)) return;
        var myId = await FirestoreManager._instance.GetDocumentId(null, "Userdata", "username", AuthenticationManager._instance.user.DisplayName);
        var otherId = await FirestoreManager._instance.GetDocumentId(null, "Userdata", "username", name);
        var otherData = await GetData(otherId);
        if (otherData == null)
        {
            Debug.LogError("other user not found");
            return;
        }
        otherData.friends.Add(AuthenticationManager._instance.user.DisplayName);
        data.friends.Add(name);
        data.request.Remove(name);
        await FirestoreManager._instance.SaveData("FriendsData", myId, "FriendsData", JsonUtility.ToJson(data), null);
        await FirestoreManager._instance.SaveData("FriendsData", otherId, "FriendsData", JsonUtility.ToJson(otherData), null);
        UpdateUI();

    }
    public async void SendFriendRequest(string name)
    {
        name = name.Trim();
        if (AuthenticationManager._instance.user.DisplayName == name) return;
        if (data.friends.Contains(name)) return;
        var docId = await FirestoreManager._instance.GetDocumentId(null, "Userdata", "username", name);
        if (docId == string.Empty)
        {
            Debug.LogError("user not found");
            return;
        }
        var otherData = await GetData(docId);
        if (otherData == null)
        {
            Debug.LogError("other user not found");
            return;
        }
        if (otherData.request.Contains(AuthenticationManager._instance.user.DisplayName)) return;
        otherData.request.Add(AuthenticationManager._instance.user.DisplayName);
        await FirestoreManager._instance.SaveData("FriendsData", docId, "FriendsData", JsonUtility.ToJson(otherData), null);

    }

    internal async void DeclineFriendRequest(string name)
    {
        name = name.Trim();
        if (!data.request.Contains(name)) return;
        data.request.Remove(name);
        await FirestoreManager._instance.SaveData("FriendsData", AuthenticationManager._instance.user.UserId, "FriendsData", JsonUtility.ToJson(data), UpdateUI);
        UpdateUI();
    }
}
public class UserData
{
    public string userName;

    public UserData(string username)
    {
        this.userName = username;
    }
}
[System.Serializable]
public class FriendsData
{
    public List<string> friends;
    public List<string> request;

    public FriendsData()
    {
        this.friends = new();
        this.request = new();
    }
}



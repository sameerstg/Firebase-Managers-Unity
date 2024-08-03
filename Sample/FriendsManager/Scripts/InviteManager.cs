using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class InviteManager : MonoBehaviour, IChatClientListener
{
    public static InviteManager _instance;
    public Transform requestParent;
    public FriendRequestSlot friendRequestSlot;
    public ChatClient chatClient;
    public string channel = "RegionalChannel";
    public List<FriendRequestSlot> slots;
    private void Awake()
    {
        _instance = this;
    }
    private void Start()
    {
        Initialize();
    }
    public void Initialize()
    {
        chatClient = new(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new AuthenticationValues(FirebaseAuthManager._instance.user.DisplayName));
    }
    private void Update()
    {
        if (chatClient != null)
            chatClient.Service();
    }
    #region LocalFunctionality
    public void SendRequest(string username)
    {
        if (string.IsNullOrEmpty(username)) return;
        if (!chatClient.CanChat) return;
        var invite = new Invite(FirebaseAuthManager._instance.user.DisplayName);
        chatClient.SendPrivateMessage(username, "invite");
    }
    public void GetRequest(string sender)
    {
        if (slots.Exists(x => x.userNameText.text == sender)) return;
        var card = Instantiate(friendRequestSlot, requestParent);
        card.userNameText.text = sender + " invited you to play...";
        card.gameObject.SetActive(true);
        slots.Add(card);
        card.acceptButton.onClick.AddListener(() =>
        {
            slots.Remove(card);
            LeanTween.cancel(card.gameObject);
            Destroy(card.gameObject);
            AcceptInvite(sender);

        });
        card.declineButton.onClick.AddListener(() =>
        {
            LeanTween.cancel(card.gameObject);
            slots.Remove(card);
            Destroy(card.gameObject);
        });
        LeanTween.delayedCall(7, () =>
        {
            slots.Remove(card);
            try
            {
                Destroy(card.gameObject);
            }
            catch (System.Exception)
            {

            }
        });
    }
    public void AcceptInvite(string sender)
    {
        var newInvite = new Invite(sender, FirebaseAuthManager._instance.user.DisplayName);
        chatClient.SendPrivateMessage(sender, JsonUtility.ToJson(newInvite));
        StartGame(newInvite);
    }
    public void StartGame(Invite invite)
    {
        if (invite.sender == string.Empty || invite.receiver == string.Empty) return;
        GameManager._instance.roomName = invite.sender + invite.receiver;
        GameManager._instance.customRoom = true;
        SceneLoader.LoadScene(3);
    }


    #endregion
    public void DebugReturn(DebugLevel level, string message)
    {
    }

    public void OnDisconnected()
    {
        throw new System.NotImplementedException();
    }

    public void OnConnected()
    {
        chatClient.Subscribe(new string[] { channel });
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.LogError(state);
    }


    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        throw new System.NotImplementedException();
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        Debug.LogError(sender);
        Debug.LogError(message);
        if (Equals(sender, FirebaseAuthManager._instance.user.DisplayName)) return;
        try
        {
            if (Equals(message, "invite"))
            {
                GetRequest(sender);
                return;
            }
        }
        catch (System.Exception)
        {

        }
        try
        {
            Invite invite = JsonUtility.FromJson<Invite>((string)message);
            Assert.IsNotNull(invite);
            Assert.AreNotEqual(invite.sender, string.Empty);
            Assert.AreNotEqual(invite.receiver, string.Empty);
            StartGame(invite);
        }
        catch (System.Exception)
        {

        }
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
    }

    public void OnUnsubscribed(string[] channels)
    {
        throw new System.NotImplementedException();
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        throw new System.NotImplementedException();
    }

    public void OnUserSubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }
}
[System.Serializable]
public class Invite
{
    public string sender, receiver;
    public Invite(string sender)
    {
        this.sender = sender;
        this.receiver = string.Empty;
    }

    public Invite(string sender, string receiver)
    {
        this.sender = sender;
        this.receiver = receiver;
    }

}

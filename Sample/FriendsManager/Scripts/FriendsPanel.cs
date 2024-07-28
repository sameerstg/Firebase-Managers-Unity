using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FriendsPanel : MonoBehaviour
{
    public FriendCard friendCardPrefab;
    public GameObject noFriendPrompt;
    public Transform parent;
    public List<FriendCard> friendCardList;

    [Header("Request")]
    public Transform requestParent;
    public FriendRequestSlot friendRequestSlot;
    public List<FriendRequestSlot> friendRequestSlots;
    public GameObject receivedFriendRequestSign;
    [Header("send request reference")]
    public TMP_InputField reqUsernameInputField;
    public Button sendReqButton;
    private void Awake()
    {
        sendReqButton.onClick.AddListener(() =>
        {
            var user = reqUsernameInputField.text;
            user = user.Trim();
            if (string.IsNullOrEmpty(user)) return;
            FriendsManager._instance.SendFriendRequest(user);
            reqUsernameInputField.text = string.Empty;
        });
    }
    public void UpdateFriendsList(FriendsData friends)
    {
        foreach (var item in friendCardList.ToList())
        {

            Destroy(item.gameObject);
        }
        friendCardList.Clear();
        try
        {

            if (friends == null || friends.friends.Count == 0)
            {
                noFriendPrompt.SetActive(true);
                return;
            }
            noFriendPrompt.SetActive(false);
        }
        catch (System.Exception)
        {

        }
        foreach (var item in friends.friends)
        {
            var card = Instantiate(friendCardPrefab, parent);
            card.userName.text = item;
            string other = item;
            card.inviteButton.onClick.AddListener(() =>
            {
                InviteManager._instance.SendRequest(other);
            });
            friendCardList.Add(card);
            card.gameObject.SetActive(true);
        }


    }
    public void UpdateFriendsRequestList(FriendsData friends)
    {

        foreach (var item in friendRequestSlots.ToList())
        {
            Destroy(item.gameObject);
        }
        friendRequestSlots.Clear();
        if (friends == null || friends.request.Count == 0)
        {
            receivedFriendRequestSign.SetActive(false);
            return;
        }
        receivedFriendRequestSign.SetActive(true);
        foreach (var item in friends.request)
        {
            var card = Instantiate(friendRequestSlot, requestParent);
            card.userNameText.text = item + " sent friend request...";
            friendRequestSlots.Add(card);
            card.gameObject.SetActive(true);
            card.acceptButton.onClick.AddListener(() =>
            {
                FriendsManager._instance.AcceptFriendRequest(item);
                friendRequestSlots.Remove(card);
                Destroy(card.gameObject);
            });
            card.declineButton.onClick.AddListener(() =>
            {
                FriendsManager._instance.DeclineFriendRequest(item);
                friendRequestSlots.Remove(card);
                Destroy(card.gameObject);
            });
        }
    }
    public void UpdateUI(FriendsData friends)
    {

        UpdateFriendsList(friends);
        UpdateFriendsRequestList(friends);
    }

}

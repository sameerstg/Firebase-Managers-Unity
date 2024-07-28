using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendRequestSlot : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI userNameText;
    public Button acceptButton;
    public Button declineButton;
    //private void Awake()
    //{
    //    declineButton.onClick.AddListener(() =>
    //    {
            
    //        FriendsManager._instance.DeclineFriendRequest(userNameText.text);
    //        Destroy(gameObject);
    //    });
    //    acceptButton.onClick.AddListener(() =>
    //    {
    //        FriendsManager._instance.AcceptFriendRequest(userNameText.text); 
    //        Destroy(gameObject);
    //    });
    //}
}

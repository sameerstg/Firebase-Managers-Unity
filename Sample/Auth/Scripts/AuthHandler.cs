using System.Linq;
using TMPro;
using UnityEngine;

public class AuthHandler : MonoBehaviour
{
    public static AuthHandler _instance;
    private void Awake()
    {
        _instance = this;
    }
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;



    public TMP_InputField nameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField confirmPasswordRegisterField;
    public TMP_Text loginError;
    public void Login()
    {
        AuthenticationManager._instance.Login(emailLoginField.text, passwordLoginField.text);
    }
    public void Register()
    {
        AuthenticationManager._instance.Register(nameRegisterField.text,emailRegisterField.text,passwordRegisterField.text,confirmPasswordRegisterField.text);
    }
    public void RegisterAsGuest()
    {
        var randomUserName = "Guest-" + Random.Range(100000, 999999);
        AuthenticationManager._instance.Register(randomUserName, randomUserName+"@gmail.com","asasfasgfasgagsasg", "asasfasgfasgagsasg");

    }
}

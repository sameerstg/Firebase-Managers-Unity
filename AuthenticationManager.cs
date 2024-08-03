using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class AuthenticationManager : MonoBehaviour
{

    public static AuthenticationManager _instance;
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;

    public UnityEvent onAuthentication;



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
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;

            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);

                SceneManager.LoadScene(0);
            }

            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
                onAuthentication?.Invoke();
                SceneManager.LoadScene(2);
            }
        }
    }
    public void Login(string email, string password)
    {
        StartCoroutine(LoginAsync(email, password));
    }
    [ContextMenu("Logout")]
    public void LogOut()
    {
        if (user != null)
        {
            auth.SignOut();
        }
    }
    private IEnumerator LoginAsync(string email, string password)
    {
        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            Debug.LogError(loginTask.Exception);

            FirebaseException firebaseException = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError authError = (AuthError)firebaseException.ErrorCode;


            string failedMessage = "Login Failed! Because ";

            switch (authError)
            {
                case AuthError.UserDisabled:
                    failedMessage += "Your account has been disabled by an administrator.";
                    //UiManager.instance.accountDisabled();
                    break;
                case AuthError.InvalidEmail:
                    failedMessage += "Email is invalid";
                    break;
                case AuthError.WrongPassword:
                    failedMessage += "Wrong Password";
                    break;
                case AuthError.MissingEmail:
                    failedMessage += "Email is missing";
                    break;
                case AuthError.MissingPassword:
                    failedMessage += "Password is missing";
                    break;
                default:
                    failedMessage = "Login Failed";
                    break;
            }

            Debug.Log(failedMessage);
            AuthHandler._instance.loginError.text = "<color=red>" + failedMessage + "</color>";
        }
        else
        {
            user = loginTask.Result.User;

            Debug.LogFormat("{0} You Are Successfully Logged In", user.DisplayName);
            AuthHandler._instance.loginError.text = "<color=white>" + user.DisplayName + "You Are Successfully Logged In" + "</color>";
        }
    }

    public void Register(string username, string email, string password, string confirmPassword)
    {
        StartCoroutine(RegisterAsync(username, email, password, confirmPassword));
    }

    private IEnumerator RegisterAsync(string name, string email, string password, string confirmPassword)
    {
        if (name == "")
        {
            Debug.LogError("User Name is empty");
        }
        else if (email == "")
        {
            Debug.LogError("email field is empty");
        }
        else if (password != confirmPassword)
        {
            Debug.LogError("Password does not match");
        }
        else
        {
            var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);

            yield return new WaitUntil(() => registerTask.IsCompleted);

            if (registerTask.Exception != null)
            {
                Debug.LogError(registerTask.Exception);

                FirebaseException firebaseException = registerTask.Exception.GetBaseException() as FirebaseException;
                AuthError authError = (AuthError)firebaseException.ErrorCode;

                string failedMessage = "Registration Failed! Becuase ";
                switch (authError)
                {
                    case AuthError.InvalidEmail:
                    User:
                        failedMessage += "Email is invalid";
                        break;
                    case AuthError.WrongPassword:
                        failedMessage += "Wrong Password";
                        break;
                    case AuthError.MissingEmail:
                        failedMessage += "Email is missing";
                        break;
                    case AuthError.MissingPassword:
                        failedMessage += "Password is missing";
                        break;
                    default:
                        failedMessage = "Registration Failed";
                        break;
                }

                Debug.Log(failedMessage);
                AuthHandler._instance.registerError.text = "<color=red>" + failedMessage + "</color>";
            }
            else
            {

                user = registerTask.Result.User;

                UserProfile userProfile = new UserProfile { DisplayName = name };

                var updateProfileTask = user.UpdateUserProfileAsync(userProfile);

                yield return new WaitUntil(() => updateProfileTask.IsCompleted);

                if (updateProfileTask.Exception != null)
                {

                    user.DeleteAsync();

                    Debug.LogError(updateProfileTask.Exception);

                    FirebaseException firebaseException = updateProfileTask.Exception.GetBaseException() as FirebaseException;
                    AuthError authError = (AuthError)firebaseException.ErrorCode;


                    string failedMessage = "Profile update Failed! Becuase ";

                    switch (authError)
                    {
                        case AuthError.InvalidEmail:
                            failedMessage += "Email is invalid";
                            break;
                        case AuthError.WrongPassword:
                            failedMessage += "Wrong Password";
                            break;
                        case AuthError.MissingEmail:
                            failedMessage += "Email is missing";
                            break;
                        case AuthError.MissingPassword:
                            failedMessage += "Password is missing";
                            break;
                        default:
                            failedMessage = "Profile update Failed";
                            break;
                    }

                    Debug.Log(failedMessage);
                    AuthHandler._instance.loginError.text = "<color=red>" + failedMessage + "</color>";
                }
                else
                {
                    Debug.Log("Registration Sucessful Welcome " + user.DisplayName);
                    AuthHandler._instance.loginError.text = "<color=red>" + "Registration Sucessful Welcome " + user.DisplayName + "</color>";
                    //_ = FirestoreManager._instance.SaveData("Users", user.UserId, "username", user.DisplayName, () =>
                    yield return FirestoreManager._instance.SaveData("Userdata", user.UserId, new() { { "username", user.DisplayName } }, () =>
 {
 });
                    yield return FirestoreManager._instance.SaveData("FriendsData", user.UserId, new() { { "FriendsData", JsonUtility.ToJson(new FriendsData()) } }, () =>
 {
     //SceneManager.LoadScene(2);
 });
                }
            }
        }
    }
   }
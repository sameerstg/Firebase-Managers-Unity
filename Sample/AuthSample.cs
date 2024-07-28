using UnityEngine;

public class AuthSample : MonoBehaviour
{
    public void Sign1()
    {
        AuthenticationManager._instance.Login("s1@g.com", "abcd123");
    }
    public void Sign2()
    {
        AuthenticationManager._instance.Login("s2@g.com", "abcd123");
    }
    public void Register1()
    {
        AuthenticationManager._instance.Register("s1", "s1@g.com", "abcd123", "abcd123");
    }
    public void Register2()
    {
        AuthenticationManager._instance.Register("s2", "s2@g.com", "abcd123", "abcd123");
    }

}

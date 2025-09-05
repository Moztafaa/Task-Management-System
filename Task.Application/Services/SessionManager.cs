using Task.Domain.Entities;

namespace Task.Application.Services;

public class SessionManager
{
    private static SessionManager? _instance;
    private static readonly object _lock = new object();

    public User? CurrentUser { get; private set; }
    public bool IsLoggedIn => CurrentUser != null;
    public Guid UserId => GetCurrentUserId();

    private SessionManager() { }

    public static SessionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= new SessionManager();
                }
            }
            return _instance;
        }
    }

    public void Login(User user)
    {
        CurrentUser = user;
    }

    public void Logout()
    {
        CurrentUser = null;
    }

    public Guid GetCurrentUserId()
    {
        if (!IsLoggedIn)
            throw new InvalidOperationException("No user is currently logged in");

        return CurrentUser!.Id;
    }

    public string GetCurrentUsername()
    {
        if (!IsLoggedIn)
            throw new InvalidOperationException("No user is currently logged in");

        return CurrentUser!.Username ?? "Unknown";
    }
}

namespace Api.Hubs;

public class PresenceTracker
{
    private readonly Dictionary<Guid, HashSet<string>> _onlineUsers = new();
    private readonly object _lock = new();

    public bool UserConnected(Guid userId, string connectionId)
    {
        lock (_lock)
        {
            if (!_onlineUsers.ContainsKey(userId))
            {
                _onlineUsers[userId] = new HashSet<string>();
            }
            return _onlineUsers[userId].Add(connectionId);
        }
    }

    public bool UserDisconnected(Guid userId, string connectionId)
    {
        lock (_lock)
        {
            if (!_onlineUsers.ContainsKey(userId))
                return false;

            _onlineUsers[userId].Remove(connectionId);

            if (_onlineUsers[userId].Count == 0)
            {
                _onlineUsers.Remove(userId);
                return true;
            }

            return false;
        }
    }

    public Guid[] GetOnlineUsers()
    {
        lock (_lock)
        {
            return _onlineUsers.Keys.ToArray();
        }
    }

    public bool IsUserOnline(Guid userId)
    {
        lock (_lock)
        {
            return _onlineUsers.ContainsKey(userId) && _onlineUsers[userId].Count > 0;
        }
    }
}

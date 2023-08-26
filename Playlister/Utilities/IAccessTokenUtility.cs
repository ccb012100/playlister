namespace Playlister.Utilities
{
    public interface IAccessTokenUtility
    {
        string GetAccessTokenFromCurrentHttpContext();
    }
}

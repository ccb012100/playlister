namespace Playlister.Utilities;

public interface IAccessTokenUtility
{
    public string GetAccessTokenFromRequestAuthHeader();
    public string GetTokenFromUserCookie();
}

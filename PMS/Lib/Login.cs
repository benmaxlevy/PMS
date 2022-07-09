using PMS.Models;
using PMS.Contexts;

namespace PMS.Lib;

internal static class Login
{
    internal static void LoginUser(ref User user, string username, string hashedPassword, UserContext userContext)
    {
        if (userContext.User != null)
            user = userContext.User.ToList().FirstOrDefault(u => u.Username == username && u.HashedPassword == hashedPassword,
                defaultValue: null);
        else
        {
            throw new NullReferenceException("UserContext is null");
        }
    }
}
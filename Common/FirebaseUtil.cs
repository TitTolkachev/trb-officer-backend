using FirebaseAdmin.Auth;

namespace trb_officer_backend.Common;

public static class FirebaseUtil
{
    public static async Task Validate(string token)
    {
        var decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        var disabled = FirebaseAuth.DefaultInstance.GetUserAsync(decoded.Uid).Result.Disabled;

        if (disabled)
        {
            throw new Exception($"User {decoded.Uid} is disabled");
        }

        if (decoded.Claims.TryGetValue("officer", out var isOfficer))
        {
            if (!(bool)isOfficer)
                throw new Exception("Forbidden");
        }
        else throw new Exception("Forbidden");
    }

    public static async Task ValidateWithId(string token, string id)
    {
        var decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        var disabled = FirebaseAuth.DefaultInstance.GetUserAsync(decoded.Uid).Result.Disabled;

        if (disabled)
        {
            throw new Exception($"User {decoded.Uid} is disabled");
        }

        if (decoded.Claims.TryGetValue("officer", out var isOfficer))
        {
            if (!(bool)isOfficer)
                throw new Exception("Forbidden");
        }
        else throw new Exception("Forbidden");

        if (decoded.Claims.TryGetValue("id", out var userId))
        {
            if ((string)userId != id)
                throw new Exception("Forbidden");
        }
        else throw new Exception("Forbidden");
    }

    public static async Task<string> GetUserId(string token)
    {
        var decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        if (decoded.Claims.TryGetValue("id", out var userId))
        {
            return (string)userId;
        }

        throw new Exception("Unauthorized");
    }
}
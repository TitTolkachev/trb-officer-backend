using FirebaseAdmin.Auth;

namespace trb_officer_backend.Common;

public static class FirebaseUtil
{
    public static async Task<bool> IsOfficer(string token)
    {
        // Verify the ID token first.
        var decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        if (decoded.Claims.TryGetValue("role", out var userRole))
        {
            return (string)userRole == "officer";
        }

        throw new Exception("Unknown");;
    }

    public static async Task Validate(string token)
    {
        // Verify the ID token first.
        var decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token, true);
        if (decoded.Claims.TryGetValue("officer", out var isOfficer))
        {
            if (!(bool)isOfficer)
                throw new Exception("Forbidden");
        }
        else throw new Exception("Forbidden");
    }

    public static async Task ValidateWithId(string token, string id)
    {
        // Verify the ID token first.
        var decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token, true);
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
        // Verify the ID token first.
        var decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        if (decoded.Claims.TryGetValue("id", out var userId))
        {
            return (string)userId;
        }

        throw new Exception("Unauthorized");
    }
}
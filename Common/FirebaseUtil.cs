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

        return false;
    }

    public static async Task Validate(string token)
    {
        // Verify the ID token first.
        var decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        if (decoded.Claims.TryGetValue("role", out var userRole))
        {
            if ((string)userRole != "officer")
                throw new Exception("Forbidden");
        }
    }

    public static async Task ValidateWithId(string token, string id)
    {
        // Verify the ID token first.
        var decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        if (decoded.Claims.TryGetValue("role", out var userRole))
        {
            if ((string)userRole != "officer")
                throw new Exception("Forbidden");
        }

        if (decoded.Claims.TryGetValue("id", out var userId))
        {
            if ((string)userId != id)
                throw new Exception("Forbidden");
        }
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
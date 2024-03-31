using FirebaseAdmin.Auth;

namespace trb_officer_backend.Common;

public static class FirebaseUtil
{
    public static async Task<bool> IsOfficer(string idToken)
    {
        // Verify the ID token first.
        var decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
        if (decoded.Claims.TryGetValue("role", out var userRole))
        {
            return (string)userRole == "officer";
        }

        return false;
    }
}
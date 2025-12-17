namespace Wanderling.Application.Results
{
    public static class ErrorCodes
    {
        public const string InvalidPayload = "invalid_payloads";
        public const string DuplicateEmail = "duplicate_email";
        public const string DuplicateUsername = "duplicate_username";
        public const string IdentityCreateFailed = "identity_create_failed";
        public const string RoleAssignFailed = "role_assign_failed";
        public const string InvalidCredentials = "invalid_credentials";
        public const string InvalidUserRole = "invalid_user_role";
    }
}

using System.ComponentModel;

namespace UserAccessSystem.Contract
{
    public enum ErrorCode : int
    {
        None = -1,
        UnexpectedError = 1,
        NotFound = 2,
        Unauthorized = 3,
        UserNotFound = 4,
        GroupNotFound = 5,
        UserAlreadyInGroup = 6,
        UserNotInGroup = 7,
        PermissionNotFound = 8,
    }
}

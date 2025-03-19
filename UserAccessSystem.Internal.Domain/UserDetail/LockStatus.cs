namespace UserAccessSystem.Domain.UserDetail;

public enum LockStatus : byte
{
    None = 0,
    Locked = 1,
    Hold = 2,
    PasswordMismatch = 3,
}

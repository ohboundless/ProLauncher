namespace Boundless.Desktop.Models.Accounts;

public class LoginSession(AccountResponse? account, bool isValid = true)
{
    public bool IsValid = isValid;
    public AccountResponse? Account = account;

    public static LoginSession Invalid => new(null, false);
}
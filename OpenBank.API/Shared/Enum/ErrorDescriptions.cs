using System.Runtime.Serialization;

namespace OpenBank.Api.Shared;

public class WarningDescriptions
{
    public static readonly string CreateAccount = "Error while creating the user";
    public static readonly string GetAccounts = "Error while obtaining the account";
    public static readonly string GetAccountById = "Error while obtaining this account";
    public static readonly string FailedTransfer = "Operation could not be concluded";
    public static readonly string ForbiddenAccess = "";

}
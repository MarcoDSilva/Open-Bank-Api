namespace OpenBank.Api.Shared;

public class AccountDescriptions
{
    public static readonly string Debit = "Debit";
    public static readonly string Credit = "Credit";
    public static readonly string AccountNotFound = "Account_from was not found.";
    public static readonly string BearerNotAllowed = "Bearer";
    public static readonly string LowerBalance = "Account_from balance is lower than the transfer amount.";
    public static readonly string DifferentCurrencies = "Accounts have different currencies.";
}


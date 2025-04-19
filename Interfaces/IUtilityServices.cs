namespace fc_minimalApi.Interfaces
{
    public interface IUtilityServices
    {
        string TimeDiff(DateTime start, DateTime end);
        string EncryptString(string plainText);

        string DecryptString(string cipherText);

    }
}

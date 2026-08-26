namespace Application.Abstractions
{
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }
}

namespace GECManager.Api.Entities;

public class UserSecret
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
}
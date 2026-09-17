using ServiceManagerApp.Entities;

namespace ServiceManagerApp;

public class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Phonenumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}
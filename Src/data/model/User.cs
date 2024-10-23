namespace src.data.model;

public class User
{
    public User(int id, string name, string email, string password, string role, DateTime lastLogin, string status, int eloRating, int wins, int losses, int draws, DateTime lastMatch, string lastGameResult, string preferredColor, string theme, string profilePictureUrl, string bio)
    {
        Id = id;
        Name = name;
        Email = email;
        Password = password;
        Role = role;
        LastLogin = lastLogin;
        Status = status;
        EloRating = eloRating;
        Wins = wins;
        Losses = losses;
        Draws = draws;
        LastMatch = lastMatch;
        LastGameResult = lastGameResult;
        PreferredColor = preferredColor;
        Theme = theme;
        ProfilePictureUrl = profilePictureUrl;
        Bio = bio;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime LastLogin { get; set; }
    public string Status { get; set; }
    
    public int EloRating { get; set; } = 1000;
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Draws { get; set; }
    public DateTime LastMatch { get; set; }
    public string LastGameResult { get; set; }
    
    public string PreferredColor { get; set; }
    public string Theme { get; set; }
    
    public string ProfilePictureUrl { get; set; }
    public string Bio { get; set; }
}
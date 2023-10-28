namespace AdminCnsole_Backend.Models;

public class Registration
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Password{ get; set; }
    public int IsActive { get; set; }
}
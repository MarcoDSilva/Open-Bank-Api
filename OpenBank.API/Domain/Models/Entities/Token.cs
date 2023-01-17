using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenBank.API.Domain.Models.Entities;

public class Token
{
    [Required]
    public string RefreshToken { get; set; }
    [Required]
    public string Jti { get; set; }

    [Required]
    public DateTime ExpirationTime { get; set; }

    public DateTime? UsedDate { get; set; }

    [Required]
    [ForeignKey("Users")]
    public int UserId { get; set; }
}
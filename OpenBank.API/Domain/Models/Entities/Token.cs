using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OpenBank.API.Domain.Models.Common;

namespace OpenBank.API.Domain.Models.Entities;

public class Token : BaseEntity
{
    [Required]
    public string RefreshToken { get; set; }
    [Required]
    public string Jti { get; set; }

    [Required]
    public DateTime ExpirationTime { get; set; }

    public DateTime? UsedDate { get; set; }

    [ForeignKey("Users")]
    public int UserId { get; set; }
}
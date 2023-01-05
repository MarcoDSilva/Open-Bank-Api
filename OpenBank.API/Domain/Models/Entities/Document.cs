using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OpenBank.API.Domain.Models.Common;

namespace OpenBank.API.Domain.Models.Entities;

public class Document : BaseEntity
{
    [Required]
    public string Url { get; set; }

    [Required]
    public string FileName { get; set; }

    [Required]
    public string ContentType { get; set; }

    [Required]
    public int SizeMB { get; set; }

    [Required]
    [ForeignKey("Accounts")]
    public int AccountId { get; set; }
}
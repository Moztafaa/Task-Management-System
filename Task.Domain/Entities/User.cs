using System;
using System.ComponentModel.DataAnnotations;

namespace Task.Domain.Entities;

public class User
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MinLength(5)]
    [MaxLength(50)]
    public string? Username { get; set; }

    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    public string? PasswordHash { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }

    // Relationships
    public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
}

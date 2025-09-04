using System;
using System.ComponentModel.DataAnnotations;

namespace Task.Domain.Entities;

public class User
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MinLength(5)]
    [MaxLength(100)]
    public string? Username { get; set; }

    [MaxLength(100)]
    [EmailAddress]
    public string? Email { get; set; }

    // Relationships
    public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
}

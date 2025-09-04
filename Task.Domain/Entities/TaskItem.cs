using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task.Domain.Entities;

public class TaskItem
{
    [Key]
    public Guid Id { get; set; }

    [MaxLength(200)]
    public string? Title { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? DueDate { get; set; }

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public TaskStatus Status { get; set; } = TaskStatus.Pending;

    // Relationships
    public Guid UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual User? AssignedUser { get; set; }

    public Guid? CategoryId { get; set; }
    [ForeignKey("CategoryId")]
    public virtual Category? Category { get; set; }
}


public enum TaskPriority
{
    Low = 1,
    Medium,
    High
}
public enum TaskStatus
{
    Pending = 1,
    InProgress,
    Completed
}

using System;
using System.ComponentModel.DataAnnotations;

namespace Task.Domain.Entities;

public class Category
{
    [Key]
    public Guid Id { get; set; }

    [MaxLength(100)]
    [MinLength(2)]
    public string? Name { get; set; }

    [MaxLength(250)]
    public string? Description { get; set; }

    public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}

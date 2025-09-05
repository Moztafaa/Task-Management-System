using System;
using Task.Domain.Entities;

namespace Task.Application.DTO;

public class TaskStatusReportDto
{
    public int TotalTasks { get; set; }
    public int PendingTasks { get; set; }
    public int InProgressTasks { get; set; }
    public int CompletedTasks { get; set; }
    public double CompletionPercentage { get; set; }
    public DateTime GeneratedAt { get; set; }
    public string? GeneratedBy { get; set; }
    public List<TaskStatusBreakdownDto> StatusBreakdown { get; set; } = new();
}

public class TaskStatusBreakdownDto
{
    public Domain.Entities.TaskStatus Status { get; set; }
    public int Count { get; set; }
    public double Percentage { get; set; }
    public List<TaskItem> Tasks { get; set; } = new();
}

public class TaskPriorityReportDto
{
    public TaskPriority Priority { get; set; }
    public int TotalCount { get; set; }
    public int PendingCount { get; set; }
    public int InProgressCount { get; set; }
    public int CompletedCount { get; set; }
    public double CompletionRate { get; set; }
}

public class DetailedTaskReportDto
{
    public TaskStatusReportDto StatusSummary { get; set; } = new();
    public List<TaskPriorityReportDto> PriorityBreakdown { get; set; } = new();
    public List<TaskItem> OverdueTasks { get; set; } = new();
    public List<TaskItem> UpcomingTasks { get; set; } = new();
    public TimeSpan AverageCompletionTime { get; set; }
}

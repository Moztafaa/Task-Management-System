using System;
using Task.Application.DTO;

namespace Task.Application.ServiveInterface;

public interface IReportService
{
    // Status Reports
    TaskStatusReportDto GenerateStatusReport();
    TaskStatusReportDto GenerateStatusReportForUser(Guid userId);
    TaskStatusReportDto GenerateStatusReportForDateRange(DateTime startDate, DateTime endDate);

    // Detailed Reports
    DetailedTaskReportDto GenerateDetailedReport();
    DetailedTaskReportDto GenerateDetailedReportForUser(Guid userId);

    // Export Reports
    string ExportReportToText(TaskStatusReportDto report);
    string ExportReportToCsv(TaskStatusReportDto report);
}

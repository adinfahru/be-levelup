namespace LevelUp.API.DTOs.Dashboards;

public record DashboardResponse(
    int TotalIdle,
    int TotalEnroll,
    int TotalModules,
    int TotalEmployee
);

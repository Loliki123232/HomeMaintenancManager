namespace HomeMaintenanceManager.Core.Enums;

public enum TaskCategory
{
    Electrical,
    Plumbing,
    Heating,
    Finishing,
    Appliances
}

public enum Priority
{
    Critical,
    High,
    Medium,
    Low
}



public enum Frequency
{
    Once,
    Monthly,
    Quarterly,
    Yearly
}

public enum Executor
{
    Myself,
    Professional,
    Family
}
using System;
namespace Legend.Shared.Core.Enums
{
    [Serializable]
    public enum CourseStatus : byte
    {
        Created = 0,
        Scheduled = 10,
        Published = 20,
        Withdrawn = 30,
        OutOfDate = 40
    }
}
using System;

namespace Playr.DataModels
{
    public class DbEvent : DbModel
    {
        public DateTimeOffset Date { get; set; }
        public string UserId { get; set; }
        public string EventType { get; set; }
        public string Message { get; set; }
        public string ExceptionType { get; set; }
        public string ExceptionMessage { get; set; }
        public string ExceptionStackTrace { get; set; }
    }
}
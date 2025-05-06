namespace ELE.MockApi.Core.Models
{
    public class Log
    {
        private Log() { }

        public Log(string content)
        {
            Content = content;
            Id = Guid.NewGuid();
            DateTime = DateTime.Now;
        }

        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime DateTime { get; set; }
        public LogType LogType { get; set; }
    }
}

using System;

namespace Duo.Models
{
    public class MockComment
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string User { get; set; }
        public string Description { get; set; }
        public int TreeLevel { get; set; }
        public DateTime Date { get; set; }
    }
} 
using System;

namespace IS.Blazor.Models
{
    public class BookModel
    {
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Synopsis { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public int Chapters { get; set; }
        public bool FullReleased { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}

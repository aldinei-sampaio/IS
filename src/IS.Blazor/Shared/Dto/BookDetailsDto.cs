using System.Collections.Generic;
using System.Linq;

namespace IS.Blazor.Dto
{
    public class BookDetailsDto : BookDto
    {
        public string Synopsis { get; set; } = string.Empty;
        public IEnumerable<string> Tags { get; set; } = Enumerable.Empty<string>();
        public int ReleasedChapters { get; set; } = 0;
        public bool FullReleased { get; set; }
    }
}

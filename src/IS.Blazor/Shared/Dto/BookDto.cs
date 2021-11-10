using System.Collections.Generic;
using System.Linq;

namespace IS.Blazor.Dto
{
    public class BookDto
    {
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public IEnumerable<string> Categories { get; set; } = Enumerable.Empty<string>();
    }
}

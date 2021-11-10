using System.Collections.Generic;
using System.Linq;

namespace IS.Blazor.Dto
{
    public class CategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public IEnumerable<BookDto> Books { get; set; } = Enumerable.Empty<BookDto>();
    }
}

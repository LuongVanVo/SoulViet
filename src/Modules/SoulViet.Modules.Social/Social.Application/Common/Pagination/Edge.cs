using System;
using System.Collections.Generic;
using System.Text;

namespace SoulViet.Modules.Social.Social.Application.Common.Pagination
{
    public class Edge<T>
    {
        public string Cursor { get; set; } = string.Empty;
        public T Node { get; set; } = default!;
    }
}

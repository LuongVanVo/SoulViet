using System;
using System.Collections.Generic;
using System.Text;

namespace SoulViet.Modules.Social.Social.Application.Common.Pagination
{
    public class PageInfo
    {
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public string? StartCursor { get; set; }
        public string? EndCursor { get; set; }
    }
}

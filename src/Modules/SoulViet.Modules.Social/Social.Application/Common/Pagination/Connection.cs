using System;
using System.Collections.Generic;
using System.Text;

namespace SoulViet.Modules.Social.Social.Application.Common.Pagination
{
    public class Connection<T>
    {
        public List<Edge<T>> Edges { get; set; } = new();
        public PageInfo PageInfo { get; set; } = new();
        public int TotalCount { get; set; }
    }
}

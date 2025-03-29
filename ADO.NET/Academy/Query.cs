using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy
{
    class Query
    {
        public string Columns { get; }
        public string Tables { get; }
        public string Condition { get; }
        public string Group_by { get; }
        public Query(string colums, string tables, string condition="", string group_by="")
        {
            Columns = colums;
            Tables = tables;
            Condition = condition;
            Group_by = group_by;

        }
    }
}

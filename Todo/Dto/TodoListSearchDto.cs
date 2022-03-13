using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Todo.Dto
{
    public class TodoListSearchDto
    {
        public Guid? TodoId { get; set; }
        public string Name { get; set; }
        public bool? Enable { get; set; }
        public int? minOrders { get; set; }
        public int? maxOrders { get; set; }
        public string InsertEmployeeName { get; set; }
        public string UpdateEmployeeName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Todo.Dto
{
    public class TodoListPostUpDto
    {
        public string Name { get; set; }
        public bool Enable { get; set; }
        public int Orders { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}

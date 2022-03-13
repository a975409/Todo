using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Todo.Dto;
using Todo.Models;

namespace Todo.Abstract
{
    public class TodoListEditDtoAbstract : IValidatableObject
    {
        public string Name { get; set; }
        public bool Enable { get; set; }
        public int Orders { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<UploadFilePostDto> UploadFiles { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //取得資料庫物件
            TodoContext _todoContext = (TodoContext)validationContext.GetService(typeof(TodoContext));

            string todoName = this.Name;

            var result = _todoContext.TodoLists.Where(m => m.Name == todoName);

            if (result != null && result.Count() > 0)
                yield return new ValidationResult("已有相同的代辦事項");

            if (this.StartTime > this.EndTime)
                yield return new ValidationResult("開始時間不可以大於結束時間");
        }
    }
}

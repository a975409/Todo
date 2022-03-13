using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Todo.Dto;
using Todo.Models;

namespace Todo.ValidationAttributes
{
    public class TodoNameAttribute : ValidationAttribute
    {
        //新增錯誤訊息的欄位
        private string _errorMessage;

        //新增錯誤訊息的屬性，讓使用者可以自己指定屬性名稱做設定
        public string errorMessage
        {
            get { return _errorMessage; }
            set
            {
                if (!string.IsNullOrEmpty(value.ToString()))
                {
                    _errorMessage = value;
                }
            }
        }

        public TodoNameAttribute(string errorMessage = "")
        {
            _errorMessage = errorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //取得資料庫物件
            TodoContext _todoContext = (TodoContext)validationContext.GetService(typeof(TodoContext));

            string todoName = (string)value;

            var result = _todoContext.TodoLists.Where(m => m.Name == todoName);

            if (result != null && result.Count() > 0)
                return new ValidationResult(_errorMessage);

            return ValidationResult.Success;
        }
    }
}

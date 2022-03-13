using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.Dto;
using Todo.Models;

namespace Todo.Service
{
    public class TodoListService
    {
        private readonly TodoContext _todoContext;

        public TodoListService(TodoContext todoContext)
        {
            _todoContext = todoContext;
        }

        /// <summary>
        /// 回傳符合搜尋條件的TodoList結果
        /// </summary>
        /// <param name="searchDto">搜尋條件</param>
        /// <returns>搜尋結果</returns>
        public List<TodoList> GetSearchResult(TodoListSearchDto searchDto)
        {
            var source = _todoContext.TodoLists
                .Include(m => m.InsertEmployee)
                .Include(m => m.UpdateEmployee)
                .Include(m => m.UploadFiles)
                .Select(m => m);

            if (searchDto.Enable != null)
                source = source.Where(m => m.Enable == searchDto.Enable);

            if (!string.IsNullOrEmpty(searchDto.Name))
                source = source.Where(m => m.Name == searchDto.Name);

            if (!string.IsNullOrEmpty(searchDto.InsertEmployeeName))
                source = source.Where(m => m.InsertEmployee.Name == searchDto.InsertEmployeeName);

            if (!string.IsNullOrEmpty(searchDto.UpdateEmployeeName))
                source = source.Where(m => m.UpdateEmployee.Name == searchDto.UpdateEmployeeName);

            if (searchDto.minOrders != null)
                source = source.Where(m => m.Orders >= searchDto.minOrders);

            if (searchDto.maxOrders != null)
                source = source.Where(m => m.Orders <= searchDto.maxOrders);

            if (searchDto.TodoId != null)
                source = source.Where(m => m.TodoId == searchDto.TodoId);

            return source.ToList();
        }

        /// <summary>
        /// 更新指定的TodoList
        /// </summary>
        /// <param name="id">TodoId</param>
        /// <param name="update">更新的TodoList欄位</param>
        /// <returns>更新結果</returns>
        public TodoList updateTodoList(Guid id, TodoListPutDto update)
        {
            var result = _todoContext.TodoLists.Find(id);

            if (result == null)
                return null;

            if (update.Enable != null)
                result.Enable = (bool)update.Enable;

            if (update.EndTime != null)
                result.EndTime = (DateTime)update.EndTime;

            if (update.StartTime != null)
                result.StartTime = (DateTime)update.StartTime;

            if (!string.IsNullOrEmpty(update.Name))
                result.Name = update.Name;

            if (update.Orders != null)
                result.Orders = (int)update.Orders;

            result.UpdateTime = DateTime.Now;
            return result;
        }
    }
}

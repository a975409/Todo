using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Todo.Dto;
using Todo.Models;
using Todo.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Todo.Controllers
{
    [Authorize("demo2")]//額外指定授權策略
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TodoContext _todoContext;
        private readonly TodoListService _todoListService;
        private readonly IMapper _iMapper;
        private readonly IWebHostEnvironment _env;

        public TodoController(TodoContext todoContext, IMapper iMapper, TodoListService todoListService, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _todoContext = todoContext;
            _iMapper = iMapper;
            _todoListService = todoListService;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: api/<TodoController>
        [Authorize("demo2",Roles = "select")]
        [HttpGet]
        public IActionResult Get([FromQuery] TodoListSearchDto searchDto)
        {
            var result = _todoListService.GetSearchResult(searchDto);

            if (result == null || !result.Any())
                return NoContent();

            return Ok(_iMapper.Map<List<TodoListResponseDto>>(result));
        }

        // GET api/<TodoController>/5
        [Authorize("demo2", Roles = "automapper")]
        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var source = _todoContext.TodoLists
                .Include(m => m.UploadFiles)
                .Include(m => m.InsertEmployee)
                .Include(m => m.UpdateEmployee)
                .Where(m => m.TodoId == id)
                .SingleOrDefault();

            if (source == null)
                return NoContent();

            var result = _iMapper.Map<TodoListResponseDto>(source);

            return Ok(result);
        }

        // POST api/<TodoController>
        [HttpPost]
        public IActionResult Post([FromBody] TodoListPostDto value)
        {
            var post = _iMapper.Map<TodoList>(value);
            var claims = _httpContextAccessor.HttpContext.User.Claims;
            post.UpdateEmployeeId = Guid.Parse(claims.Where(m => m.Type == "EmployeeId").FirstOrDefault().Value);
            post.InsertEmployeeId = Guid.Parse(claims.Where(m => m.Type == "EmployeeId").FirstOrDefault().Value);

            _todoContext.TodoLists.Add(post);
            try
            {
                _todoContext.SaveChanges();
                return CreatedAtAction(nameof(Get), new { id = post.TodoId }, _iMapper.Map<TodoListResponseDto>(post));
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPost("PostUpFile")]
        public IActionResult PostUpFile(List<IFormFile> files, [FromForm][ModelBinder(BinderType = typeof(AuthorEntityBinder))] TodoListPostUpDto value)
        {
            var post = _iMapper.Map<TodoList>(value);

            if (files.Count > 0)
            {
                List<UploadFile> uploads = new List<UploadFile>();
                files.ForEach((m) =>
                {
                    uploads.Add(new UploadFile
                    {
                        Name = m.FileName,
                        Src = m.FileName
                    });
                });

                post.UploadFiles = uploads;
            }

            _todoContext.TodoLists.Add(post);

            try
            {
                _todoContext.SaveChanges();

                #region 處理檔案儲存
                if (files.Count > 0)
                {
                    string saveDir = Path.Combine(_env.ContentRootPath, "wwwroot", "UploadFiles", post.TodoId.ToString());

                    if (!Directory.Exists(saveDir))
                    {
                        Directory.CreateDirectory(saveDir);
                    }

                    foreach (IFormFile file in files)
                    {
                        string saveFile = Path.Combine(saveDir, file.FileName);

                        using (var stream = System.IO.File.Create(saveFile))
                        {
                            file.CopyTo(stream);
                        }
                    }
                }
                #endregion

                return CreatedAtAction(nameof(Get), new { id = post.TodoId }, _iMapper.Map<TodoListResponseDto>(post));
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }

        }

        // PUT api/<TodoController>/5
        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody] TodoListPutDto update)
        {
            var result = _todoListService.updateTodoList(id, update);

            if (result == null)
                return NotFound("找不到資源");

            _todoContext.TodoLists.Update(result);

            try
            {
                _todoContext.SaveChanges();
                return CreatedAtAction(nameof(Get), new { id = result.TodoId }, _iMapper.Map<TodoListResponseDto>(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        // DELETE api/<TodoController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var result = _todoContext.TodoLists
                .Where(m => m.TodoId == id)
                .Include(m => m.UpdateEmployee)
                .Include(m => m.InsertEmployee)
                .Include(m => m.UploadFiles)
                .SingleOrDefault();

            if (result == null)
                return NotFound("找不到資源");

            _todoContext.TodoLists.Remove(result);

            try
            {
                _todoContext.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        /// <summary>
        /// 刪除多筆資料
        /// </summary>
        /// <param name="ids">Json字串</param>
        /// <returns></returns>
        [HttpDelete("List/{ids}")]
        public IActionResult DeleteList(string ids)
        { 
            List<Guid> TodoIds= System.Text.Json.JsonSerializer.Deserialize<List<Guid>>(ids);

            var result = _todoContext.TodoLists.Where(m => TodoIds.Contains(m.TodoId))
                .Include(m => m.UpdateEmployee)
                .Include(m => m.InsertEmployee)
                .Include(m => m.UploadFiles)
                .Select(m => m);

            if (result == null || result.Count() == 0)
                return NotFound("找不到資源");

            _todoContext.TodoLists.RemoveRange(result);

            try
            {
                _todoContext.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }
    }
}

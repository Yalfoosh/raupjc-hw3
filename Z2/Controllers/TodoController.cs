using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Z2.Models;
using HW3;

namespace Z2.Controllers
{
    [Authorize]
    public class TodoController : Controller
    {
        private readonly ITodoRepository _repository;
        private readonly UserManager<ApplicationUser> _user;

        public TodoController(ITodoRepository repository, UserManager<ApplicationUser> user)
        {
            _repository = repository;
            _user = user;
        }

        public Task<ApplicationUser> FetchUser() => _user.GetUserAsync(HttpContext.User);

        public async Task<IActionResult> Index()
        {
            var thisUser = await FetchUser();

            var thisModel = new IndexViewModel((await _repository.GetActiveAsync(new Guid(thisUser.Id)))
                                               .Select(item => new TodoViewModel(item.Id, item.Text, item.DateDue, false))
                                               .ToList());
            return View(thisModel);
        }

        public IActionResult AddItem() => View();

        [HttpPost]
        public async Task<IActionResult> AddItem(AddTodoViewModel added)
        {
            if (ModelState.IsValid)
            {
                var user = await FetchUser();
                var item = new TodoItem(added.Text, new Guid(user.Id))
                {
                    DateCreated = DateTime.Now,
                    DateDue = added.DateDue
                };

                _repository.Add(item);

                if (!string.IsNullOrEmpty(added.Labels))
                {
                    string[] labels = added.Labels.Split(',');

                    foreach (string s in labels)
                        _repository.AddLabel(s.Trim().ToLower(), item.Id);
                }

                return RedirectToAction("Index");
            }

            return View(added);
        }

        public async Task<IActionResult> Completed()
        {
            var thisUser = await FetchUser();
            var thisModel = new CompletedViewModel((await _repository.GetCompletedAsync(new Guid(thisUser.Id)))
                                                   .Select(item => new TodoViewModel(item.Id, item.Text, item.DateDue, true))
                                                   .ToList());

            return View(thisModel);
        }

        public async Task<IActionResult> ToggleCompleted(Guid id)
        {
            var thisUser = await FetchUser();
            var item = await _repository.GetAsync(id, new Guid(thisUser.Id));

            if (item.IsCompleted)
                await _repository.RemoveAsync(item.Id, new Guid(thisUser.Id));
            else
            {
                item.DateCompleted = DateTime.Now;
                _repository.Update(item, new Guid(thisUser.Id));
            }

            return Redirect("/todo");
        }
    }
}
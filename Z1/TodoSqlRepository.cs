using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace HW3
{
    public class TodoSqlRepository : ITodoRepository
    {
        private readonly TodoDbContext _context;

        public TodoSqlRepository(TodoDbContext context)
        {
            _context = context;
        }

        public TodoItem Get(Guid todoId, Guid userId)
        {
            var item = _context.Items.Find(todoId);

            if (item == null)
                return null;

            if (!item.UserId.Equals(userId))
                throw new TodoAccessDeniedException("Pristup odbijen: samo vlasnik može pristupiti ovom poslu.");

            return _context.Items.FirstOrDefault(t => t.Id.Equals(todoId));
        }

        public async Task<TodoItem> GetAsync(Guid todoId, Guid userId)
        {
            var item = await _context.Items.FindAsync(todoId);

            if (item == null)
                return null;

            if (!item.UserId.Equals(userId))
                throw new TodoAccessDeniedException("Pristup odbijen: samo vlasnik može pristupiti ovom poslu.");

            return await _context.Items.FirstOrDefaultAsync(i => i.Id.Equals(todoId));
        }

        public void Add(TodoItem todoItem)
        {
            if (todoItem != null)
            {
                var item = _context.Items.Find(todoItem.Id);

                if(item != null)
                    throw new DuplicateTodoItemException("duplicate id: {" + todoItem.Id + "}");

                _context.Items?.Add(todoItem);
                _context.SaveChanges();
            }
        }

        public async Task AddAsync(TodoItem todoItem)
        {
            if (todoItem != null)
            {
                if (_context.Items.Contains(todoItem))
                    throw new DuplicateTodoItemException("duplicate id: {" + todoItem.Id + "}");

                _context.Items.Add(todoItem);
                await _context.SaveChangesAsync();
            }
        }

        public void AddLabel(string text, Guid todoId)
        {
            Console.WriteLine(text);

            var item = _context.Items.Find(todoId);
            var label = _context.Labels.Find(text);

            if (item != null)
            {
                if (label == null)
                {
                    label = new TodoItemLabel(text);
                    _context.Labels.Add(label);
                }

                label.LabelTodoItems.Add(item);
                item.Labels.Add(label);
            }

            _context.SaveChanges();
        }

        public async Task AddLabelAsync (string text, Guid todoId)
        {
            var item = await _context.Items.FindAsync(todoId);
            var label = await _context.Labels.FirstOrDefaultAsync(x => x.Value == text);

            if (item != null)
            {
                if (label == null)
                {
                    label = new TodoItemLabel(text);
                    _context.Labels.Add(label);
                }

                label.LabelTodoItems.Add(item);
                item.Labels.Add(label);
            }

            await _context.SaveChangesAsync();
        }

        public bool Remove(Guid todoId, Guid userId)
        {
            var item = _context.Items.Find(todoId);

            if (item == null)
                return false;

            if(!item.UserId.Equals(userId))
                throw new TodoAccessDeniedException("Pristup odbijen: samo vlasnik može ukloniti ovaj posao.");

            _context.Items.Remove(item);
            _context.SaveChanges();

            return true;
        }

        public async Task<bool> RemoveAsync(Guid todoId, Guid userId)
        {
            var item = await _context.Items.FindAsync(todoId);

            if (item == null)
                return false;

            if (!item.UserId.Equals(userId))
                throw new TodoAccessDeniedException("Pristup odbijen: samo vlasnik može ukloniti ovaj posao.");

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

            return true;
        }

        public void Update(TodoItem todoItem, Guid userId)
        {
            if (todoItem != null)
            {
                var item = _context.Items.Find(todoItem.Id);

                if (item == null)
                    Add(todoItem);
                else if (!item.UserId.Equals(userId))
                    throw new TodoAccessDeniedException("Pristup odbijen: samo vlasnik može ažurirati vrijednost ovog posla.");

                _context.Entry(item).CurrentValues.SetValues(todoItem);
                _context.SaveChanges();
            }
        }

        public async Task UpdateAsync(TodoItem todoItem, Guid userId)
        {
            if (todoItem != null)
            {
                var item = await _context.Items.FindAsync(todoItem.Id);

                if (item == null)
                    Add(todoItem);
                else if (!item.UserId.Equals(userId))
                    throw new TodoAccessDeniedException("Pristup odbijen: samo vlasnik može ažurirati vrijednost ovog posla.");

                _context.Entry(item).CurrentValues.SetValues(todoItem);
                await _context.SaveChangesAsync();
            }
        }

        public bool MarkAsCompleted(Guid todoId, Guid userId)
        {
            var item = _context.Items.Find(todoId);

            if (item == null)
                return false;

            if(!item.UserId.Equals(userId))
                throw new TodoAccessDeniedException("Pristup odbijen: samo vlasnik može označiti da je ovaj posao gotov.");

            item.Complete();
            _context.SaveChanges();

            return true;
        }

        public async Task<bool> MarkAsCompletedAsync(Guid todoId, Guid userId)
        {
            var item = await _context.Items.FindAsync(todoId);

            if (item == null)
                return false;

            if (!item.UserId.Equals(userId))
                throw new TodoAccessDeniedException("Pristup odbijen: samo vlasnik može označiti da je ovaj posao gotov.");

            item.Complete();
            await _context.SaveChangesAsync();

            return true;
        }

        public List<TodoItem> GetAll(Guid userId) => _context.Items.Where(item => item.UserId.Equals(userId))
                                                                   .OrderByDescending(item => item.DateCreated)
                                                                   .ToList();

        public Task<List<TodoItem>> GetAllAsync(Guid userId) => _context.Items.Where(item => item.UserId.Equals(userId))
                                                                              .OrderByDescending(item => item.DateCreated)
                                                                              .ToListAsync();

        public List<TodoItem> GetActive(Guid userId) => 
            _context.Items.Where(item => item.UserId.Equals(userId) && !item.DateCompleted.HasValue)
                          .ToList();

        public Task<List<TodoItem>> GetActiveAsync(Guid userId) =>
            _context.Items.Where(item => item.UserId.Equals(userId) && !item.DateCompleted.HasValue)
                          .ToListAsync();

        public List<TodoItem> GetCompleted(Guid userId) =>
            _context.Items.Where(item => item.UserId.Equals(userId) && item.DateCompleted.HasValue)
                          .ToList();

        public Task<List<TodoItem>> GetCompletedAsync(Guid userId) => 
            _context.Items.Where(item => item.UserId.Equals(userId) && item.DateCompleted.HasValue)
                          .ToListAsync();

        public List<TodoItem> GetFiltered(Func<TodoItem, bool> filterFunction, Guid userId) => 
            _context.Items.Where(item => item.UserId.Equals(userId) && filterFunction(item))
                          .ToList();

        public Task<List<TodoItem>> GetFilteredAsync(Func<TodoItem, bool> filterFunction, Guid userId) => 
            _context.Items.Where(item => item.UserId.Equals(userId) && filterFunction(item))
                    .ToListAsync();
    }
}
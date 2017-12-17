using System.Collections.Generic;

namespace Z2.Models
{
    public class IndexViewModel
    {
        public List<TodoViewModel> Models { get; }

        public IndexViewModel(List<TodoViewModel> todoViewModels) => Models = todoViewModels;
    }
}
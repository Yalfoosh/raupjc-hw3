using System.Collections.Generic;

namespace Z2.Models
{
    public class CompletedViewModel
    {
        public List<TodoViewModel> Models { get; }

        public CompletedViewModel(List<TodoViewModel> completedModels) => Models = completedModels;
    }
}
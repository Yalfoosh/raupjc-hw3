using System;
using System.ComponentModel.DataAnnotations;

namespace Z2.Models
{
    public class AddTodoViewModel
    {
        [Required]
        public string Text { get; set; }
        public string Labels { get; set; }

        public DateTime? DateDue { get; set; }
    }
}
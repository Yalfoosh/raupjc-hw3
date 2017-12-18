using System;

namespace Z2.Models
{
    public class TodoViewModel
    {
        public Guid Id { get; set; }
        public String Text { get; set; }
        public DateTime? DateDue { get; set; }
        public bool IsCompleted;

        public TodoViewModel(Guid ID, string text, DateTime? dueDate, bool isCompleted)
        {
            Id = ID;
            Text = text;
            DateDue = dueDate;
            IsCompleted = isCompleted;
        }

        public string Remaining()
        {
            if (DateDue == null)
                return string.Empty;

            if (DateDue < DateTime.Now)
                return "Prešli ste rok!";

            TimeSpan left = (DateTime)DateDue - DateTime.Now;

            int Days = (int)Math.Floor(left.TotalDays);
            int Hours = (int)Math.Floor((double)left.Hours);
            int Minutes = (int)Math.Floor((double)left.Minutes);

            string toRet = "Još " + Days;

            toRet += Days == 1 ? " dan, " : " dana, ";
            toRet += Hours + ((Hours == 1 || Hours == 21) ? " sat, " : Hours < 5 || (Hours > 21 && Hours < 24) ? " sata, " : " sati, ");
            toRet += Minutes + (Minutes > 1 && Minutes < 5 ? " minute." : " minuta");

            return toRet;
        }

        //Will show how much is left if you have less than a week until the deadline.
        public bool ShowReminder() => !DateDue.Equals(null) 
                                      && DateDue > DateTime.Now 
                                      && ((DateTime)DateDue - DateTime.Now).Days < 7;
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HW3
{
    public class TodoItem : IEquatable<TodoItem>
    {
        /// <summary>
        /// The ID of this TodoItem
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// User id that owns this TodoItem
        /// </summary>
        public Guid UserId { get; set; }

        public string Text { get; set; }

        public bool IsCompleted => DateCompleted.HasValue;
        public DateTime? DateCompleted { get; set; }
        public DateTime DateCreated { get; set; }
        /// <summary>
        /// Date due. If null, no date was set by the user
        /// </summary>
        public DateTime? DateDue { get; set; }

        /// <summary>
        /// List of labels associated with TodoItem
        /// </summary>
        public List<TodoItemLabel> Labels { get; set; }



        public TodoItem()
        {
            //Entity framework needs this one, not for use :)
        }

        public TodoItem(string text)
        {
            Id = Guid.NewGuid();

            // DateTime.Now returns local time, it wont always be what you expect (depending where the server is).
            // We want to use universal time (UTC) which we can easily convert to local when needed.
            // (usually done in browser on the client side)

            DateCreated = DateTime.UtcNow;
            Text = text;
        }

        public TodoItem(string text, Guid userId) : this(text)
        {
            /*Id = Guid.NewGuid();
            Text = text;
            DateCreated = DateTime.UtcNow;*/

            UserId = userId;
            Labels = new List<TodoItemLabel>();
        }



        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        public bool Complete() => !IsCompleted && (DateCompleted = DateTime.UtcNow) != null;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;

            return Equals((TodoItem)obj);
        }
        public bool Equals(TodoItem other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Id.Equals(other.Id) && 
                   string.Equals(Text, other.Text) && 
                   DateCompleted.Equals(other.DateCompleted) && 
                   DateCreated.Equals(other.DateCreated);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ Text.GetHashCode();
                hashCode = (hashCode * 397) ^ DateCompleted.GetHashCode();
                hashCode = (hashCode * 397) ^ DateCreated.GetHashCode();

                return hashCode;
            }
        }

        public static bool operator ==(TodoItem left, TodoItem right) => Equals(left, right);
        public static bool operator !=(TodoItem left, TodoItem right) => !(left == right);
    }
}
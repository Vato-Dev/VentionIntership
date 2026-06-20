namespace Api.Models
{
    public sealed class Todo
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public Status  Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; } //For Task won't use DateTimeOffset it's heavier and i do not need timezone handling now

        //I'll encapsulate logic inside class because i think class is capable to know/manage own state
        private Todo(int id ,string description)
        {
            Id = id;
            Description = description;
            Status = Status.Active;
            CreatedAt = DateTime.UtcNow;
            CompletedAt = null;
        }
        
        public static Todo CreateTodo(int id, string description) =>
            new(id, description);

        public void complete()
        {
            CheckIfActiveAndNotCompleted();
            Status = Status.Completed;
            CompletedAt = DateTime.UtcNow;
        }

        public void cancel()
        {
            CheckIfActiveAndNotCompleted();
            Status = Status.Cancelled;
        }

        public void update(string description)
        {
            Description = description;
        }
        
        private void CheckIfActiveAndNotCompleted()
        {
            if(Status != Status.Active)
                throw new InvalidOperationException("Task is not Active");
        }
    }

    public enum Status
    {
        Active,
        Completed,
        Cancelled
    }
}

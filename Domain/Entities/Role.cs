namespace Domain.Entities
{
    public class Role
    {
        public int Id { get; private set; }

        public string Name { get; private set; } = string.Empty;

        private Role() { }

        public Role(string name)
        {
            Name = name;
        }
    }
}

namespace PartsHunter.Models
{
    public class Component
    {
        public string Category { get; set; }
        public string Drawer { get; set; }
        public string Description { get; set; }
        public bool validatedCategory { get; set; }
        public bool validatedDescription { get; set; }
        public bool validatedDrawer { get; set; }
    }
}

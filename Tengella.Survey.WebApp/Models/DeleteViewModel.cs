namespace Tengella.Survey.WebApp.Models
{
    public class DeleteViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public Dictionary<string, string> Properties { get; set; } = [];
        public string DeleteAction { get; set; } = string.Empty;
        public string ReturnAction { get; set; } = "Index";
        public string ReturnController { get; set; } = string.Empty;
        public List<Dictionary<string, string>> MultipleProperties { get; set; } = [];
        public Dictionary<string, string> PropertyIcons { get; set; } = [];
    }
}

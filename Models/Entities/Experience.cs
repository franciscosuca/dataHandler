namespace personalSite.Models.Entities;

public class Experience
{
    public string id { get; set; } = Guid.NewGuid().ToString();
    public string type { get; set; } = string.Empty;
    public string sdate { get; set; } = string.Empty;
    public string edate { get; set; } = string.Empty;
    public string company { get; set; } = string.Empty;
    public string location { get; set; } = string.Empty;
    public string title { get; set; } = string.Empty;
    public string summary { get; set; } = string.Empty;
    public string skills { get; set; } = string.Empty;
}
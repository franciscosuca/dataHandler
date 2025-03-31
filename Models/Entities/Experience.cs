namespace personalSite.Models.Entities;

public class Experience
{
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateOnly Sdate { get; set; }
    public DateOnly Edate { get; set; }
    public string Company { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string SampleSkills { get; set; } = string.Empty;
}
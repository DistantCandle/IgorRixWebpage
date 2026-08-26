namespace IgorRixWebpage.Web.Models.NestedElements;

public class HeaderComponent
{
    public string? Logo { get; set; }
    public List<HeaderNavigationItem> Navigation { get; set; } = new();
    public string? CtaText { get; set; }
    public string? CtaLink { get; set; }
    public bool ShowCTA { get; set; }
    public bool StickyHeader { get; set; }
}

public class HeaderNavigationItem
{
    public string? Name { get; set; }
    public string? Url { get; set; }
    public bool Active { get; set; }
    public List<HeaderNavigationItem> Children { get; set; } = new();
}
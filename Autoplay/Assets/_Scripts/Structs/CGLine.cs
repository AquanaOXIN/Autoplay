// For Setup the CG

public struct CGLine
{
    public string tagName;
    public int? cgSelect;
    public int? status;

    public CGLine(string _tagName, int? _cgSelect, int? status)
    {
        this.tagName = _tagName;
        this.cgSelect = _cgSelect;
        this.status = status;
    }
}

// For Change Scene Background

public struct BGLine
{
    public string tagName;
    public int? spriteSelect;
    public int? status;
    public int? vfxSelect;

    public BGLine(string _tagName, int? _spriteSelect, int? _status, int? _vfxSelect)
    {
        this.tagName = _tagName;
        this.spriteSelect = _spriteSelect;
        this.status = _status;
        this.vfxSelect = _vfxSelect;
    }
}

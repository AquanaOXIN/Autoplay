// For Setup the Stage

public struct STAGELine
{
    public string tagName;
    public int? stageSelect;
    public int? vfxSelect;

    public STAGELine(string _tagName, int? _stageSelect, int? _vfxSelect)
    {
        this.tagName = _tagName;
        this.stageSelect = _stageSelect;
        this.vfxSelect = _vfxSelect;
    }
}

// For Change Scene Background

public struct BGLine
{
    public int spriteIndex;
    public bool isIn;
    public int vfxSelect;

    public BGLine(int _spriteIndex, bool _isIn, int _vfxSelect)
    {
        this.spriteIndex = _spriteIndex;
        this.isIn = _isIn;
        this.vfxSelect = _vfxSelect;
    }
}

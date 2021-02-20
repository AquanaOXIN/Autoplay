// For Change UI Sprite 

public struct UISLine
{
    public int spriteIndex;
    public int posIndex;
    public bool isIn;
    public int vfxSelect;

    public UISLine(int _spriteIndex, int _posIndex, bool _isIn, int _vfxSelect)
    {
        this.spriteIndex = _spriteIndex;
        this.posIndex = _posIndex;
        this.isIn = _isIn;
        this.vfxSelect = _vfxSelect;
    }
}
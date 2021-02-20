// For Change Scene Sprite 
// Separate UI and Scene Sprites just for intuitive

public struct SSLine
{
    public int spriteIndex;
    public int posIndex;
    public bool isIn;
    public int vfxSelect;

    public SSLine(int _spriteIndex, int _posIndex, bool _isIn, int _vfxSelect)
    {
        this.spriteIndex = _spriteIndex;
        this.posIndex = _posIndex;
        this.isIn = _isIn;
        this.vfxSelect = _vfxSelect;
    }
}
// For Change Scene Sprite 
// Separate UI and Scene Sprites just for intuitive

public struct ESLine
{
    public string tagName;
    public int? spriteSelect;
    public int? posSelect;
    public int? status;
    public int? vfxSelect;

    public ESLine(string _tagName, int? _spriteSelect, int? _posSelect, int? _status, int? _vfxSelect)
    {
        this.tagName = _tagName;
        this.spriteSelect = _spriteSelect;
        this.posSelect = _posSelect;
        this.status = _status;
        this.vfxSelect = _vfxSelect;
    }
}
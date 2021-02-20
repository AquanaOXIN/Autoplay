// For Rolling Dice

public struct ROLLine
{
    public int spriteIndex;
    public int posIndex;
    public int sfxIndex; // extraSuccess, success, fail, extraFail,etc => to determine SFX
    public int vfxSelect; // normally a roll of dice won't have visual effects, but just keep it for future feature

    public ROLLine(int _spriteIndex, int _posIndex, int _sfxIndex, int _vfxSelect)
    {
        this.spriteIndex = _spriteIndex;
        this.posIndex = _posIndex;
        this.sfxIndex = _sfxIndex;      
        this.vfxSelect = _vfxSelect;
    }
}
// For Rolling Dice

public struct ROLLine
{
    public int spriteIndex;
    public int posIndex;
    public int sfxIndex; // extraSuccess, success, fail, extraFail,etc => to determine SFX
    public int vfxSelect; // normally a roll of dice won't have visual effects, but just keep it for future feature
}
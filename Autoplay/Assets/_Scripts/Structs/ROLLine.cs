// For Rolling Dice

public struct ROLLine
{
    public string tagName;
    public int? diceSelect;
    public int? posSelect;
    public int? sfxSelect; // extraSuccess, success, fail, extraFail,etc => to determine SFX
    public int? vfxSelect; // normally a roll of dice won't have visual effects, but just keep it for future feature

    public string dialogContent;

    public ROLLine(string _tagName, int? _diceSelect, int? _posSelect, int? _sfxSelect, int? _vfxSelect, string _dialogContent)
    {
        this.tagName = _tagName;
        this.diceSelect = _diceSelect;
        this.posSelect = _posSelect;
        this.sfxSelect = _sfxSelect;      
        this.vfxSelect = _vfxSelect;
        this.dialogContent = _dialogContent;
    }
}
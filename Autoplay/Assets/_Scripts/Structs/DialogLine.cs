// For Character Dialogue

public struct DialogLine
{
    public string speakerName; // Initialize by TAG when processing the file
    public int? emoSelect; // which pose/emotion does the speaking character use
    public int? posSelect; // for position
    public int? status; // 0 - stay | 1 - in | 2 - leave, etc
    public int? vfxSelect;
    
    // Get from script
    public int speechIndex; // which line(AudioClip) to speak
                            // ... can(should) be counted as a Hashtable in Autoplay Manager
    public string dialogContent;

    public DialogLine(string _speakerName, int? _emoSelect, int? _posSelect, int? _status, int? _vfxSelect, int _speechIndex, string _dialogContent)
    {
        this.speakerName = _speakerName;
        this.emoSelect = _emoSelect;
        this.posSelect = _posSelect;
        this.status = _status;
        this.vfxSelect = _vfxSelect;
        this.speechIndex = _speechIndex;
        this.dialogContent = _dialogContent;
    }
}
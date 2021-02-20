// For Character Dialogue

public struct DialogLine
{
    public string speakerName; // Initialize by TAG when processing the file
    public int emoIndex; // which pose/emotion does the speaking character use
    public int posIndex; // for position
    public int status; // 0 - leave | 1 - in | 2 - stay, etc
    public int vfxSelect;
    
    // Get from script
    public int speechIndex; // which line(AudioClip) to speak
                     // ... can(should) be counted as a Hashtable in Autoplay Manager

    public DialogLine(string _speakerName, int _emoIndex, int _posIndex, int _status, int _vfxSelect, int _speechIndex)
    {
        this.speakerName = _speakerName;
        this.emoIndex = _emoIndex;
        this.posIndex = _posIndex;
        this.status = _status;
        this.vfxSelect = _vfxSelect;
        this.speechIndex = _speechIndex;
    }
}
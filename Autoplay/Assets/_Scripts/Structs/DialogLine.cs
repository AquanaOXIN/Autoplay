// For Character Dialogue

public struct DialogLine
{
    public string speakerName; // Initialize by TAG when processing the file
    public int? emoSelect; // which pose/emotion does the speaking character use
    // if it's gonna be anime, then it should be a num stands for a anim state
    public int? posSelect; // for position
    public int? status; // 0 - leave | 1 - in | 2 - move | null - stay
    public int? vfxSelect; // 0 - fadeout | 1 - fadein | 2 - slide
    
    // Get from script
    public int speechIndex; // which line(AudioClip) to speak
                            // ... can(should) be counted as a Hashtable in Autoplay Manager
    public string dialogContent;

    // change other/self emotion part
    public int? charNum;
    public int? emo2Change;
    public int? when2Change;
    public int? where2Change; // if there's already a character, then stick together

    public DialogLine(string _speakerName, int? _emoSelect, int? _posSelect, int? _status, int? _vfxSelect, int _speechIndex,
        string _dialogContent, int? _charNum = null, int? _emo2Change = 0, int? _when2Change = 0, int? _where2Change = 0)
    {
        this.speakerName = _speakerName;
        this.emoSelect = _emoSelect;
        this.posSelect = _posSelect;
        this.status = _status;
        this.vfxSelect = _vfxSelect;
        this.speechIndex = _speechIndex;
        this.dialogContent = _dialogContent;

        this.charNum = _charNum;
        this.emo2Change = _emo2Change;
        this.when2Change = _when2Change;
        this.where2Change = _where2Change;
    }
}
// For Character Dialogue

public struct DialogLine
{
    public string speakerName;
    public int emoIndex; // which pose/emotion does the speaking character use
    public int posIndex; // for position
    public int status; // 0 - leave | 1 - in | 2 - stay, etc
    public int vfxSelect;
    
    // Get from script
    public int audioIndex; // which line(AudioClip) to speak
                     // ... can(should) be counted as a Hashtable in Autoplay Manager
}
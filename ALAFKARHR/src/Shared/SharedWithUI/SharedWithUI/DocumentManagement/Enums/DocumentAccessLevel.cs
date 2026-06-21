namespace SharedWithUI.DocumentManagement.Enums;

public enum DocumentAccessLevel
{
    Read = 1,
    ReadWrite = 2
}

public enum DocumentListScope
{
    All = 0,
    OwnedByMe = 1,
    SharedWithMe = 2
}

public enum DocumentUploadPreset
{
    All = 0,
    Image = 1,
    Text = 2,
    Pdf = 3,
    Office = 4,
    Video = 5,
    Audio = 6,
    Other = 7
}

namespace DoomSlinger;

public interface ISection
{
    bool IsOpen { get; }
    void Toggle();
    void SetOpen(bool open);
}

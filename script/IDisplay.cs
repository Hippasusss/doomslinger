
public interface IDisplay
{
    void ToggleOnOff(bool onOff);
    void UpdateDisplay(Human human);
    bool Enabled {get; set;}
}

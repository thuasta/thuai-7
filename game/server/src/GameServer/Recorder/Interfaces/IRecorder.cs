using GameServer.GameLogic;

namespace GameServer.Recorder;

public interface IRecorder
{
    public void CreateNewRecord(string recordName);
    public void RecordInitialInformation(IGame game);
    public void RecordEvents(EventArgs e);
}

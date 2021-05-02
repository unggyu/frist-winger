public class GamePointAccumulator
{
    private int gamePoint = 0;

    public int GamePoint => gamePoint;

    public void Accumulate(int value)
    {
        gamePoint += value;

        PlayerStatePanel playerStatePanel = PanelManager.GetPanel(typeof(PlayerStatePanel)) as PlayerStatePanel;
        playerStatePanel.SetScore(gamePoint);
    }

    public void Reset()
    {
        gamePoint = 0;
    }
}

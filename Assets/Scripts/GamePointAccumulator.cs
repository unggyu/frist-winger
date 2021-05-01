public class GamePointAccumulator
{
    int gamePoint = 0;

    public int GamePoint
    {
        get => gamePoint;
    }

    public void Accumulate(int value)
    {
        gamePoint += value;
    }

    public void Reset()
    {
        gamePoint = 0;
    }
}

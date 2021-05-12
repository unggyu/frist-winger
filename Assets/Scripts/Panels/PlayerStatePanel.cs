using UnityEngine;
using UnityEngine.UI;

public class PlayerStatePanel : BasePanel
{
    [SerializeField] private Text scoreText = null;
    [SerializeField] private Gage hpGage = null;

    private Player player = null;

    private Player Player
    {
        get
        {
            if (player == null)
            {
                player = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().Player;
            }

            return player;
        }
    }

    public void SetScore(int value)
    {
        Debug.Log("SetScore value = " + value);

        scoreText.text = value.ToString();
    }

    public override void InitializePanel()
    {
        base.InitializePanel();

        hpGage.SetHp(100, 100);
    }

    protected override void UpdatePanel()
    {
        base.UpdatePanel();
        if (Player != null)
        {
            hpGage.SetHp(player.CurrentHp, player.MaxHp);
        }
    }
}

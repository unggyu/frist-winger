using UnityEngine;

public class PlayerHud : MonoBehaviour
{
    private Transform ownerTransform;
    private Transform selfTransform;

    [SerializeField] private Gage hpGage;
    [SerializeField] private Player ownerPlayer;

    public void Initialize(Player player)
    {
        ownerPlayer = player;
        ownerTransform = ownerPlayer.transform;
    }

    private void Start()
    {
        selfTransform = transform;
    }

    private void Update()
    {
        UpdatePosition();
        UpdateHp();
    }

    private void UpdatePosition()
    {
        if (ownerTransform != null)
        {
            selfTransform.position = Camera.main.WorldToScreenPoint(ownerTransform.position);
        }
    }

    private void UpdateHp()
    {
        if (ownerPlayer != null)
        {
            if (!ownerPlayer.gameObject.activeSelf) // player가 active 되어있지 않다면
            {
                // hud도 같이 꺼줌
                gameObject.SetActive(ownerPlayer.gameObject.activeSelf);
            }

            hpGage.SetHp(ownerPlayer.CurrentHp, ownerPlayer.MaxHp);
        }
    }
}

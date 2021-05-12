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
            if (!ownerPlayer.gameObject.activeSelf) // player�� active �Ǿ����� �ʴٸ�
            {
                // hud�� ���� ����
                gameObject.SetActive(ownerPlayer.gameObject.activeSelf);
            }

            hpGage.SetHp(ownerPlayer.CurrentHp, ownerPlayer.MaxHp);
        }
    }
}

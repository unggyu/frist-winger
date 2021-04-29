using UnityEngine;

[System.Serializable]
public class BGScrollData
{
    public Renderer renderForScroll;
    public float speed;
    public float offsetX;
}

public class BGScroller : MonoBehaviour
{
    [SerializeField]
    BGScrollData[] scrollDatas;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScroll();
    }

    void UpdateScroll()
    {
        for (int i = 0; i < scrollDatas.Length; i++)
        {
            SetTextureOffset(scrollDatas[i]);
        }
    }

    void SetTextureOffset(BGScrollData scrollData)
    {
        scrollData.offsetX += scrollData.speed * Time.deltaTime;
        if (scrollData.offsetX > 1)
        {
            scrollData.offsetX %= 1.0f;
        }

        Vector2 _offset = new Vector2(scrollData.offsetX, 0);
        scrollData.renderForScroll.material.SetTextureOffset("_MainTex", _offset);
    }
}

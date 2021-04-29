using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    Vector3 moveVector = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMove();
    }

    void UpdateMove()
    {

    }

    public void ProcessInput(Vector3 moveDirection)
    {

    }
}

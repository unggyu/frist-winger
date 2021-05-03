using UnityEngine;

[System.Serializable]
public class SquadronData
{
    public float squadronGenerateTime;
    public Squadron squadron;
}

public class SquadronManager : MonoBehaviour
{
    private float gameStartedTime;
    private int squadronIndex;
    private bool running = false;

    [SerializeField]
    private SquadronData[] squadronDatas = null;

    public void StartGame()
    {
        gameStartedTime = Time.time;
        squadronIndex = 0;
        running = true;
        Debug.Log("Game Started!");
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            StartGame();
        }

        CheckSquadronGeneratings();
    }

    private void CheckSquadronGeneratings()
    {
        if (!running)
        {
            return;
        }

        if (Time.time - gameStartedTime >= squadronDatas[squadronIndex].squadronGenerateTime)
        {
            GenerateSquadron(squadronDatas[squadronIndex]);
            squadronIndex++;

            if (squadronIndex >= squadronDatas.Length)
            {
                AllSquadronGenerated();
                return;
            }
        }
    }

    private void GenerateSquadron(SquadronData data)
    {
        Debug.Log("GenerateSquadron");
        data.squadron.GenerateAllData();
    }

    private void AllSquadronGenerated()
    {
        Debug.Log("AllSquadronGenerated");

        running = false;
    }
}

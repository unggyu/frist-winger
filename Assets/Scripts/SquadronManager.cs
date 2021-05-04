using UnityEngine;

public class SquadronManager : MonoBehaviour
{
    private float gameStartedTime;
    private int squadronIndex;
    private bool running = false;

    [SerializeField]
    private SquadronTable[] squadronDatas = null;

    [SerializeField]
    private SquadronScheduleTable squadronScheduleTable = null;

    public void StartGame()
    {
        gameStartedTime = Time.time;
        squadronIndex = 0;
        running = true;
        Debug.Log("Game Started!");
    }

    private void Start()
    {
        squadronDatas = GetComponentsInChildren<SquadronTable>();
        for (int i = 0; i < squadronDatas.Length; i++)
        {
            squadronDatas[i].Load();
        }

        squadronScheduleTable.Load();
    }

    // Update is called once per frame
    private void Update()
    {
        CheckSquadronGeneratings();
    }

    private void CheckSquadronGeneratings()
    {
        if (!running)
        {
            return;
        }

        if (Time.time - gameStartedTime >= squadronScheduleTable.GetScheduleData(squadronIndex).GenerateTime)
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

    private void GenerateSquadron(SquadronTable table)
    {
        Debug.Log("GenerateSquadron");
        // data.squadron.GenerateAllData();

        for (int i = 0; i < table.GetCount(); i++)
        {
            SquadronMemberStruct squadronMember = table.GetSquadronMember(i);
            SystemManager
                .Instance
                .GetCurrentSceneMain<InGameSceneMain>()
                .EnemyManager
                .GenerateEnemy(squadronMember);
        }
    }

    private void AllSquadronGenerated()
    {
        Debug.Log("AllSquadronGenerated");

        running = false;
    }
}

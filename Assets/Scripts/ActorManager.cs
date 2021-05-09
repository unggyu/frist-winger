using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Actor�� NetworkVariable�� ������ �� �����Ƿ� ActorManager�� ���� ����
/// </summary>
public class ActorManager
{
    private readonly Dictionary<int, Actor> actors = new Dictionary<int, Actor>();

    public bool Regist(int actorInstanceId, Actor actor)
    {
        if (actorInstanceId == 0)
        {
            Debug.LogError("Regist Error! ActorInstanceId is not set! actorInstanceId = " + actorInstanceId);
            return false;
        }

        if (actors.ContainsKey(actorInstanceId))
        {
            if (actor.GetInstanceID() != actors[actorInstanceId].GetInstanceID())
            {
                Debug.LogError("Regist Error! already exist! actorInstanceId = " + actorInstanceId);
                return false;
            }

            Debug.Log(actorInstanceId + " is already registered!");
            return true;
        }

        actors.Add(actorInstanceId, actor);
        Debug.Log("Actor Regist id = " + actorInstanceId + ", actor = " + actor.name);
        return true;
    }

    public Actor GetActor(int actorInstanceId)
    {
        if (!actors.ContainsKey(actorInstanceId))
        {
            Debug.LogError("GetActor Error! no exist! actorInstanceId = " + actorInstanceId);
            return null;
        }

        return actors[actorInstanceId];
    }
}

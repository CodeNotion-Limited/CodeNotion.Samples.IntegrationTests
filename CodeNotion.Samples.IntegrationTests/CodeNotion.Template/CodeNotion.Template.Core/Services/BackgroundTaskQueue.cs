using System;
using System.Collections.Generic;

namespace CodeNotion.Template.Business.Services;

public interface IBackgroundTaskQueue
{
    void EnqueueMeshGroupRuleUpdate(int meshGroupId);
    (int, DateTime?)? DequeueMeshGroupRuleUpdate();
}

public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Queue<(int, DateTime?)> _meshGroupRuleUpdateQueue = new();

    public void EnqueueMeshGroupRuleUpdate(int meshGroupId)
    {
        _meshGroupRuleUpdateQueue.Enqueue((meshGroupId, null));
    }

    public (int, DateTime?)? DequeueMeshGroupRuleUpdate()
    {
        if (_meshGroupRuleUpdateQueue.Count == 0)
        {
            return null;
        }

        return _meshGroupRuleUpdateQueue.Dequeue();
    }
}
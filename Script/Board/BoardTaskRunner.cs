using System.Collections.Generic;

public class BoardTaskRunner
{
    public bool IsRunning => _tasks.Count > 0;

    private readonly Queue<BoardTask> _tasks;

    public BoardTaskRunner()
    {
        _tasks = new Queue<BoardTask>(5);
    }

    public void Add(BoardTask task)
    {
        _tasks.Enqueue(task);

        if (PlayerManager.Players.Count > 1
            && task is BoardTask_Skill)
        {
            _tasks.Enqueue(new BoardTask_SyncTokens());
        }
    }

    public void Update()
    {
        if (_tasks.Count == 0)
        {
            return;
        }

        var task = _tasks.Peek();

        if (task.Run())
        {
            return;
        }

        _tasks.Dequeue();
    }
}

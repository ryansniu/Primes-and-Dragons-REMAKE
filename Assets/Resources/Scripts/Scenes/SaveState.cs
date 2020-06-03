using System.Collections.Generic;

[System.Serializable]
public class SaveState{
    private readonly string SAVE_DATA = "/sData.binary";
    private int fs = 0;
    private double ts = 0;
    private BoardState bs;
    private List<EnemyState> es;
    private PlayerState ps;
    private bool saveExists = false;
    public bool doesSaveExist() { return saveExists; }

    public void init() {
        SaveState loadedState = GameData.readFile(SAVE_DATA) as SaveState;
        if (loadedState == null) return;
        fs = loadedState.fs;
        ts = loadedState.ts;
        bs = loadedState.bs;
        es = loadedState.es;
        ps = loadedState.ps;
        saveExists = true;
    }

    public void saveGame(int currFloor, double elapsedTime, Board board, List<Enemy> currEnemies, Player player) {
        fs = currFloor;
        ts = elapsedTime;
        bs = board.getState();
        ps = player.getState();
        es = new List<EnemyState>();
        foreach(Enemy e in currEnemies) es.Add(e.getState());
        GameData.writeFile(SAVE_DATA, this);
        saveExists = true;
    }
    public void loadGame(ref int currFloor, ref double elapsedTime, ref Board board, ref List<Enemy> currEnemies, ref Player player) {
        init();
        currFloor = fs;
        elapsedTime = ts;
        if(bs != null) board.setState(bs);
        if (ps != null) player.setState(ps);
        if (es != null) {
            currEnemies = new List<Enemy>();
            foreach (EnemyState e in es) {
                Enemy temp = Enemy.Create(e.prefab, e.number, e.maxHealth, e.damage);
                temp.setState(e);
                temp.setSprite(e.sprite);
                currEnemies.Add(temp);
            }
        }
    }
}

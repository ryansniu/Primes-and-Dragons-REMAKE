using System;

public class TutorialEnemy : Enemy {
    public static TutorialEnemy Create() => Create("Tutorial Enemy", 2, 500, 40, "tv1").GetComponent<TutorialEnemy>();
    
    protected override void addAllSkills() {
        base.addAllSkills();
    }
}

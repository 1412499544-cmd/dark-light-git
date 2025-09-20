using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDataSO",menuName = "Character/Enemy/EnemyDataSO")]
public class EnemyDataSO : ScriptableObject
{
    public Sprite sprite;
    public EnemyType enemyType;
    
    public List<EnemyAction> actions;
}

[System.Serializable]
public struct EnemyAction
{
    public Sprite intentSprite;
    //TODO:public EffectSO effect;
}
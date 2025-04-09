using UnityEngine;

[CreateAssetMenu(fileName = "CrackCard", menuName = "ScriptableObjects/CrackCard")]
public class CrackCard : ScriptableObject
{
    public string cardName;
    public string description;
    public Sprite icon;
    public CrackCardType cardType;

    public SpellEffectType spellEffect; // 항상 보이지만, Spell일 때만 사용
    public TrapEffectType trapEffect;   // 항상 보이지만, Trap일 때만 사용
}

public enum CrackCardType
{
    Spell, // 마법카드
    Trap   // 함정카드
}

public enum SpellEffectType
{
    None,           // <-- 기본값, 선택 안 된 상태
    DoubleScore,
    Gamble
}

public enum TrapEffectType
{
    None,           // <-- 기본값, 선택 안 된 상태
    SwapScoresIfLosing,
    BlockOpponentScore
}

using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class CrackCardHandler : SingleTon<CrackCardHandler>
{
    public void UseCrackCard(CrackCard card, PlayerManager user, PlayerManager opponent)
    {
        if (card == null) return;

        switch (card.cardType)
        {
            case CrackCardType.Spell:
                HandleSpell(card.spellEffect, user, opponent);
                break;

            case CrackCardType.Trap:
                HandleTrap(card.trapEffect, user, opponent);
                break;
        }
    }

    void HandleSpell(SpellEffectType effect, PlayerManager user, PlayerManager opponent)
    {
        switch (effect)
        {
            case SpellEffectType.DoubleScore:
            {
                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Score", out object scoreObj))
                {
                    int score = (int)scoreObj;
                    int newScore = score * 2;

                    Hashtable props = new Hashtable { { "Score", newScore } };
                    PhotonNetwork.LocalPlayer.SetCustomProperties(props);
                    LogManager.Instance.AddLog($"더블 스코어 적용! {score} → {newScore}");
                }
                break;
            }
            case SpellEffectType.Gamble:
            {
                if(PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Score", out object scoreObj))
                {
                    int score = (int)scoreObj;

                    int randomValue = Random.Range(0,2);
                    float value = randomValue == 0 ? 0.5f : 2f;

                    int newScore = (int)(score * value);

                    Hashtable props = new Hashtable { { "Score", newScore } };
                    PhotonNetwork.LocalPlayer.SetCustomProperties(props);
                    LogManager.Instance.AddLog($"도박 적용! {score} → {newScore}");
                }
                break;
            }
            case SpellEffectType.HeartOfTheBeast:
            {
                if(PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Score", out object scoreObj))
                {
                    int score = (int)scoreObj;

                    int randomValue = Random.Range(0,2);

                    int newScore = randomValue == 0 ? 0 : 169; //
                    Hashtable props = new Hashtable { { "Score", newScore } };
                    PhotonNetwork.LocalPlayer.SetCustomProperties(props);
                    LogManager.Instance.AddLog($"야수의 심장 적용! {score} → {newScore}");
                }
                break;
            }   
        }
    }

    void HandleTrap(TrapEffectType effect, PlayerManager user, PlayerManager opponent)
    {
        switch (effect)
        {
            case TrapEffectType.SwapScoresIfLosing:
                Player myPhotonPlayer = PhotonNetwork.LocalPlayer;
                Player oppPhotonPlayer = opponent.photonView.Owner;

                if (myPhotonPlayer.CustomProperties.TryGetValue("Score", out object myScoreObj) &&
                    oppPhotonPlayer.CustomProperties.TryGetValue("Score", out object oppScoreObj))
                {
                    int myScore = (int)myScoreObj;
                    int oppScore = (int)oppScoreObj;

                    if (myScore < oppScore)
                    {
                        Hashtable myProps = new Hashtable { { "Score", oppScore } };
                        Hashtable oppProps = new Hashtable { { "Score", myScore } };

                        myPhotonPlayer.SetCustomProperties(myProps);
                        oppPhotonPlayer.SetCustomProperties(oppProps);

                        Debug.Log($"[Trap 발동] 점수 교환! 내 점수: {myScore} ↔ 상대 점수: {oppScore}");
                    }
                    else
                    {
                        Debug.Log($"[Trap 무효] 점수가 낮지 않음 → 효과 발동 안 함");
                    }
                }
                break;
        }
    }
}

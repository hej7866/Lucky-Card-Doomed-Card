    using System;
    using UnityEngine;
    using Photon.Pun;
    using Photon.Realtime; 
    using ExitGames.Client.Photon;


    public class StrategyManager : MonoBehaviourPunCallbacks
    {
        public static StrategyManager Instance { get; private set; }

        [SerializeField] GameObject attackBtn;
        [SerializeField] GameObject defenceBtn;

        public bool isAttackSelected = false;
        public bool isDefenceSelected = false;

        public event Action<int> OnScoreChanged; // 점수 변경 이벤트

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            CardManager.Instance.OnCard01NumberChanged += _ => CalculateScore();
            CardManager.Instance.OnCard02NumberChanged += _ => CalculateScore();
        }

        public int Card01
        {
            get
            {
                return CardManager.Instance.card01Number;
            } 
        }
        
        public int Card02
        {
            get
            {
                return CardManager.Instance.card02Number;
            }
        }

        private int currentScore = 0; // 현재 점수 저장

        public void CalculateScore()
        {
            int newScore = Card01 * Card02;
            
            if (newScore != currentScore)
            {
                currentScore = newScore;
                OnScoreChanged?.Invoke(currentScore); // UI 업데이트 이벤트 호출

                // Photon에 점수 업데이트
                Hashtable props = new Hashtable { { "Score", currentScore } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            }
        }
        

        public void SelectScore()
        {
            if (TurnManager.Instance.CurrentPhase == TurnManager.TurnPhase.Battle)
            {
                LogManager.Instance.AddLog("전투페이즈에는 점수를 결정할 수 없습니다!");
                return;
            }

            if (TurnManager.Instance.isScoreSelected)
            {
                LogManager.Instance.AddLog("이미 점수를 결정하였습니다.");
                return;
            }

            TurnManager.Instance.isScoreSelected = true;
            LogManager.Instance.AddLog("점수를 결정하였습니다! 공 / 수를 선택해주세요.");
            attackBtn.SetActive(true);
            defenceBtn.SetActive(true);
        }


        private System.Collections.IEnumerator WaitForEnemyScoreAndSet()
        {
            Debug.Log("상대방 점수를 기다리는 중...");

            // 상대방이 점수를 선택할 때까지 기다림
            yield return new WaitUntil(() =>
                PhotonNetwork.PlayerListOthers.Length > 0 &&
                PhotonNetwork.PlayerListOthers[0].CustomProperties.ContainsKey("Score") &&
                PhotonNetwork.PlayerListOthers[0].CustomProperties["Score"] != null &&
                PhotonNetwork.PlayerListOthers[0].CustomProperties.ContainsKey("isAttackSelected") &&
                PhotonNetwork.PlayerListOthers[0].CustomProperties["isAttackSelected"] != null
            );

            // 상대방의 점수를 가져와서 `EnemyScore`로 저장
            foreach (Player player in PhotonNetwork.PlayerListOthers)
            {
                object enemyScoreObj, enemyAttackObj;
                if (player.CustomProperties.TryGetValue("Score", out enemyScoreObj) &&
                    player.CustomProperties.TryGetValue("isAttackSelected", out enemyAttackObj))
                {
                    Hashtable enemyProps = new Hashtable
                    {
                        { "EnemyScore", enemyScoreObj },
                        { "EnemyAttack", enemyAttackObj }
                    };
                    PhotonNetwork.CurrentRoom.SetCustomProperties(enemyProps);

                    Debug.Log($"상대방 점수 저장 완료! EnemyScore: {enemyScoreObj}, EnemyAttack: {enemyAttackObj}");
                }
            }
        }


        public void Attack()
        {
            isAttackSelected = true;
            isDefenceSelected = false;
            attackBtn.SetActive(false);
            defenceBtn.SetActive(false);
            
            SaveProps(); // 공격/수비 선택 후 네트워크에 저장
        }

        public void Defence()
        {
            isAttackSelected = false;
            isDefenceSelected = true;
            attackBtn.SetActive(false);
            defenceBtn.SetActive(false);

            SaveProps(); // 공격/수비 선택 후 네트워크에 저장
        }

        public void SaveProps()
        {
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Score", out object scoreObj))
            {
                Hashtable Props = new Hashtable 
                { 
                    { "Score", scoreObj }, 
                    { "isAttackSelected", isAttackSelected }    
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(Props);
            }

            // 마스터 클라이언트가 상대방 점수를 가져와서 `EnemyScore`를 설정
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(WaitForEnemyScoreAndSet());
            }
        }

        public void ResetStrategyState()
        {
            isAttackSelected = false;
            isDefenceSelected = false;
        }


    }

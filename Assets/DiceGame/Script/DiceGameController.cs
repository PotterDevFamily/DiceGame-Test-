using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DiceGame;
using TMPro;

public class DiceGameController : MonoBehaviour
{
    /// for showing dice rolling UI panel and Camera
    [SerializeField]
    private GameObject diceRollPanel;
    [SerializeField]
    private GameObject diceRollCam;
    [SerializeField]
    private TMP_Text diceTxt;

    /// for showing player moveable tiles
    [SerializeField]
    private Transform tileParent;
    [SerializeField]
    private List<List<GameObject>> tileLists = new List<List<GameObject>>();
    [SerializeField]
    private int squareCnt;

    //GamePanel
    [SerializeField]
    private GameObject gamePaenel;
    [SerializeField]
    private TMP_Text rolledTxt;

    //FinishPanel
    [SerializeField]
    private GameObject finishPanel;

    //Dice controlling values
    [SerializeField]
    private GameObject diceObj;
    public DiceController diceController;
    private bool diceRolledFlag;
    private Vector3 force;
    private Vector3 torque;


    //values for player
    private int stepsLeft;
    private Vector2 playerPos = new Vector2(0, 0);
    private int playerXPos;
    private int playerZPos;
    private int diceRolled;
    [SerializeField]
    private GameObject player;
    private UnityEngine.AI.NavMeshAgent playerAgent;

    [SerializeField]
    private TriggerAlert alert;


    private bool checkFlag = false;


    // Start is called before the first frame update
    void Start()
    {
        playerAgent = player.transform.parent.GetComponent<UnityEngine.AI.NavMeshAgent>();
        SetTileListsValue();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && DiceGameInfo.currentState == DiceGameInfo.GameState.SelectDirection){
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 10000)){
                if(hit.collider.tag == "tile"){
                    if(!hit.collider.GetComponent<MeshRenderer>().enabled)
                        return;
                    if(!hit.collider.GetComponent<MeshRenderer>().material.name.Contains("BlueMat")){
                        return;
                    }
                    DiceGameInfo.currentState = DiceGameInfo.GameState.PlayerMoving;
                    playerAgent.SetDestination(hit.collider.transform.position);
                    player.GetComponent<Animator>().Play("move");
                    playerPos = new Vector2(int.Parse(hit.collider.name.Split('.')[1]), int.Parse(hit.collider.name.Split('.')[0]));
                    StartCoroutine(CheckMove());
                }  
            }
        }

        if (diceRolledFlag == true && diceController.diceSleeping == true && DiceGameInfo.currentState == DiceGameInfo.GameState.DiceRolling)
        {
            DiceGameInfo.currentState = DiceGameInfo.GameState.SelectDirection;
            diceRolled = diceController.diceRoll;
            diceTxt.text =  "Dice Rolled : " + diceRolled;
            diceRolledFlag = false;
            stepsLeft = diceController.diceRoll;
            rolledTxt.gameObject.SetActive(true);
            rolledTxt.text = "Dice Rolled : " + diceRolled;
            ShowMoveableTile();
            StartCoroutine(ShowOrHideDice(false, 2.0f));
        }
        if(playerAgent.remainingDistance == 0 && DiceGameInfo.currentState == DiceGameInfo.GameState.PlayerMoving && checkFlag){
            InitGameState();
        }
    }
    IEnumerator CheckMove(){
        yield return new WaitForSeconds(0.5f);
        checkFlag = true;
    }
    //Reset
    void InitGameState(){
        if(alert.playerInFlag){
            UnityEngine.Debug.LogError("true");
            gamePaenel.SetActive(false);
            finishPanel.SetActive(true);
        }
        player.GetComponent<Animator>().Play("idle");
        DiceGameInfo.currentState = DiceGameInfo.GameState.Idle;
        foreach(Transform child in tileParent){
            child.GetComponent<MeshRenderer>().enabled = false;
        }
        checkFlag = false;
    }
    //showing player moveable tiles
    void ShowMoveableTile(){
        playerXPos = (int)playerPos.x;
        playerZPos = (int)playerPos.y;
        for(int i = 0; i < diceRolled + 1; i++){
            if(playerXPos + i < tileLists[playerZPos].Count)
                tileLists[playerZPos][playerXPos + i].GetComponent<MeshRenderer>().enabled = true;
            if(playerZPos + i < tileLists.Count)
                tileLists[playerZPos + i][playerXPos].GetComponent<MeshRenderer>().enabled = true;
        }
    }

    //setting tileLists value for showing player direction
    void SetTileListsValue(){
        int curInd = 0;
        List<GameObject> tempList = new List<GameObject>();
        foreach(Transform child in tileParent){
            if(curInd == 0){
                tempList = new List<GameObject>();
            }
            tempList.Add(child.gameObject);
            curInd ++;
            if(curInd == squareCnt){
                curInd = 0;
                tileLists.Add(tempList);
            }
        }
    }

    //click Dice Roll Button on RollDicePanel
    public void DiceRollBtnClick(){
        if(DiceGameInfo.currentState != DiceGameInfo.GameState.DiceRolling)
            return;
        force = new Vector3(Random.Range(-100,100) , Random.Range(100, 200), Random.Range(-100, 100));
        torque = new Vector3(Random.Range(100, 1000), Random.Range(100, 1000), Random.Range(100, 1000));
        diceObj.GetComponent<Rigidbody>().AddForce(torque * Random.Range(2 , 5));
        diceObj.GetComponent<Rigidbody>().AddTorque(torque * Random.Range(2, 5));
        diceObj.GetComponent<Rigidbody>().WakeUp();

        diceController.diceSleeping = false;

        diceRolledFlag = true;
    }

    //Click Move Button on GamePanel
    public void MoveBtnClick(){
        if(DiceGameInfo.currentState != DiceGameInfo.GameState.Idle)
            return;
        DiceGameInfo.currentState = DiceGameInfo.GameState.DiceRolling;
        gamePaenel.SetActive(false);
        StartCoroutine(ShowOrHideDice(true, 0));
    }
    IEnumerator ShowOrHideDice(bool actFlag, float delayTime){
        yield return new WaitForSeconds(delayTime);
        if(actFlag){
            gamePaenel.SetActive(false);
            diceRollPanel.SetActive(true);
            diceRollCam.SetActive(true);
            diceTxt.text =  "Dice Rolled : ";
        }else{
            gamePaenel.SetActive(true);
            diceRollPanel.SetActive(false);
            diceRollCam.SetActive(false);
        }
    }

    // Restart Level
    public void RestartBtnClick(){
        UnityEngine.SceneManagement.SceneManager.LoadScene("DiceGame");
    }
}

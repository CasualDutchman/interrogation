using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    bool player1Turn = true;
    bool player2Turn { get { return !player1Turn; } }

    int thisPlayer { get { return player1Turn ? 1 : 2; } }
    int otherPlayer { get { return player1Turn ? 2 : 1; } }

    bool movingButtons = false;

    [Header("Player 1")]
    public int player1Stress;
    public int player1Suspicion;
    public int player1Time;

    [Header("Player 2")]
    public int player2Stress;
    public int player2Suspicion;
    public int player2Time;

    [Header("Stress Thresholds")]
    public int beginStress;
    public int stressBecomesDifficult;
    public int stressLosesControl;
    public Vector2 HorizontalMaxButtonMovement, VerticalMaxButtonMovement;
    public float SpeedButtonMovement;

    [Header("Suspicion Thresholds")]
    public int beginSuspicion;
    public int suspicionNoTrust;

    [Header("CanvasObjects")]
    public Text player1StressText;
    public Text player1SuspicionText, player1TimeText, player2StressText, player2SuspicionText, player2TimeText;
    public Image player1TurnArrow, player2TurnArrow;
    public Button buttonLie, buttonTruth, buttonStall, buttonLawyer, buttonSilent, buttonBreak;
    public Button buttonChill, buttonOwn, buttonOther, buttonBoth, buttonRemoveAdd;
    public GameObject normalButtonHolder, breaktimeButtonHolder;

    Transform[] buttonTransforms = new Transform[6];
    Vector3[] buttonMovementDirections = new Vector3[6];

    void Start () {
        player1Stress = beginStress;
        player1Suspicion = beginSuspicion;

        player2Stress = beginStress;
        player2Suspicion = beginSuspicion;

        SetupButtons();

        if (breaktimeButtonHolder.activeSelf) {
            normalButtonHolder.SetActive(true);
            breaktimeButtonHolder.SetActive(false);
        }

        buttonTransforms[0] = buttonLie.transform;
        buttonTransforms[1] = buttonTruth.transform;
        buttonTransforms[2] = buttonStall.transform;
        buttonTransforms[3] = buttonLawyer.transform;
        buttonTransforms[4] = buttonSilent.transform;
        buttonTransforms[5] = buttonBreak.transform;

        for (int i = 0; i < 6; i++) {
            buttonMovementDirections[i] = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
        }
    }
	
    void SetupButtons() {
        buttonLie.onClick.AddListener(Lie);
        buttonTruth.onClick.AddListener(Truth);
        buttonStall.onClick.AddListener(Stall);
        buttonLawyer.onClick.AddListener(Lawyer);
        buttonSilent.onClick.AddListener(Silent);
        buttonBreak.onClick.AddListener(BreakTime);

        buttonChill.onClick.AddListener(ChillDude);
        buttonOwn.onClick.AddListener(TamperOwn);
        buttonOther.onClick.AddListener(TamperOther);
        buttonBoth.onClick.AddListener(TamperBoth);
        buttonRemoveAdd.onClick.AddListener(RemoveOwnAddOther);
    }

	void Update () {
        UpdateTexts();

        if ((player1Turn && player1Stress >= stressBecomesDifficult) || (player2Turn && player2Stress >= stressBecomesDifficult))
            MovingButtons();
    }

    void UpdateTexts() {
        player1StressText.text = player1Stress.ToString();
        player1SuspicionText.text = player1Suspicion.ToString();
        player1TimeText.text = player1Time.ToString();

        player2StressText.text = player2Stress.ToString();
        player2SuspicionText.text = player2Suspicion.ToString();
        player2TimeText.text = player2Time.ToString();
    }

    void MovingButtons() {
        if (normalButtonHolder.GetComponent<VerticalLayoutGroup>().enabled) {
            ButtonMovement(true);
            for (int i = 0; i < 6; i++) {
                buttonTransforms[i].localPosition = new Vector3(Random.Range(HorizontalMaxButtonMovement.x, HorizontalMaxButtonMovement.y), Random.Range(VerticalMaxButtonMovement.x, VerticalMaxButtonMovement.y));
            }
        }

        for (int i = 0; i < 6; i++) {
            if (buttonTransforms[i].localPosition.x < HorizontalMaxButtonMovement.x)
                buttonMovementDirections[i] = new Vector3(1, Random.Range(-1.0f, 1.0f));

            if (buttonTransforms[i].localPosition.x > HorizontalMaxButtonMovement.y)
                buttonMovementDirections[i] = new Vector3(-1, Random.Range(-1.0f, 1.0f));

            if (buttonTransforms[i].localPosition.y < VerticalMaxButtonMovement.x)
                buttonMovementDirections[i] = new Vector3(Random.Range(-1.0f, 1.0f), 1);

            if (buttonTransforms[i].localPosition.y > VerticalMaxButtonMovement.y)
                buttonMovementDirections[i] = new Vector3(Random.Range(-1.0f, 1.0f), -1);

            buttonTransforms[i].Translate(buttonMovementDirections[i].normalized * Time.deltaTime * SpeedButtonMovement);
        }
    }

    [ContextMenu("Test")]
    void TestFunction() {
        player1Stress = stressBecomesDifficult + 1;
    }

    void SwitchTurn() {
        player1Turn = !player1Turn;
        player1TurnArrow.enabled = player1Turn;
        player2TurnArrow.enabled = player2Turn;

        if (breaktimeButtonHolder.activeSelf) {
            normalButtonHolder.SetActive(true);
            breaktimeButtonHolder.SetActive(false);
        }

        if (player1Turn && player1Time >= 20 && player1Suspicion < suspicionNoTrust) {
            buttonBreak.gameObject.SetActive(true);
        }else if(player2Time >= 20 && player2Suspicion < suspicionNoTrust) {
            buttonBreak.gameObject.SetActive(true);
        }else {
            buttonBreak.gameObject.SetActive(false);
        }

        ButtonMovement(false);
    }

    void ButtonMovement(bool b) {
        normalButtonHolder.GetComponent<VerticalLayoutGroup>().enabled = !b;
        normalButtonHolder.transform.GetChild(0).GetComponent<HorizontalLayoutGroup>().enabled = !b;
        normalButtonHolder.transform.GetChild(1).GetComponent<HorizontalLayoutGroup>().enabled = !b;
    }

    void AskBreakTime() {
        normalButtonHolder.SetActive(false);
        breaktimeButtonHolder.SetActive(true);
    }

    #region setters
    void AddPlayerStress(int playerID, int amount) {
        if (playerID == 1) {
            player1Stress += amount;
        }else {
            player2Stress += amount;
        }
    }

    void AddPlayerSuspicion(int playerID, int amount) {
        if (playerID == 1) {
            player1Suspicion += amount;
        } else {
            player2Suspicion += amount;
        }
    }

    void AddPlayerTime(int playerID, int amount) {
        if (playerID == 1) {
            player1Time += amount;
        } else {
            player2Time += amount;
        }
    }
    #endregion

    #region buttonFunctions
    void Lie() {
        AddPlayerStress(thisPlayer, 10);
        AddPlayerTime(thisPlayer, 5);

        SwitchTurn();
    }

    void Truth() {
        AddPlayerSuspicion(thisPlayer, 10);
        AddPlayerTime(thisPlayer, 5);

        AddPlayerSuspicion(otherPlayer, 5);

        SwitchTurn();
    }

    void Silent() {
        AddPlayerStress(thisPlayer, -2);
        AddPlayerSuspicion(thisPlayer, 2);

        AddPlayerSuspicion(otherPlayer, 2);
        AddPlayerTime(otherPlayer, 2);

        SwitchTurn();
    }

    void Lawyer() {
        AddPlayerSuspicion(thisPlayer, -5);

        AddPlayerStress(otherPlayer, 10);
        AddPlayerSuspicion(otherPlayer, 5);
        AddPlayerTime(otherPlayer, 5);

        SwitchTurn();
    }

    void Stall() {
        AddPlayerStress(thisPlayer, 2);
        AddPlayerSuspicion(thisPlayer, 2);

        AddPlayerTime(otherPlayer, 5);

        SwitchTurn();
    }

    void BreakTime() {
        AskBreakTime();
    }

    void ChillDude() {
        AddPlayerStress(thisPlayer, -15);
        AddPlayerTime(thisPlayer, -10);

        SwitchTurn();
    }

    void TamperOwn() {
        AddPlayerSuspicion(thisPlayer, -15);
        AddPlayerTime(thisPlayer, -10);

        SwitchTurn();
    }

    void TamperOther() {
        AddPlayerTime(thisPlayer, -10);

        AddPlayerSuspicion(otherPlayer, -15);

        SwitchTurn();
    }

    void TamperBoth() {
        AddPlayerSuspicion(thisPlayer, -10);
        AddPlayerTime(thisPlayer, -15);

        AddPlayerSuspicion(otherPlayer, -10);

        SwitchTurn();
    }

    void RemoveOwnAddOther() {
        AddPlayerSuspicion(thisPlayer, -10);
        AddPlayerTime(thisPlayer, -5);

        AddPlayerSuspicion(otherPlayer, 5);

        SwitchTurn();
    }
    #endregion
}

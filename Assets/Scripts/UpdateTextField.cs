using System;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class UpdateTextField : MonoBehaviour
{

    public InputField inputField;
    public Text textField, gameText, attemptText, playerName, playerCount;
    string randomWord, correctWord;
    string[] fullStringArray = new string[10];
    string[] playedWordsArray = new string[10];
    public Canvas background;

    bool _restarting = true;

    float restartGame;
    int attempts, correctAttempts;

    System.Random rnd = new System.Random();
    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

    // Use this for initialization
    void Start()
    {
        attemptText.text = "Attempts: \n";
        inputField.textComponent.color = Color.green;
        textField.color = Color.white;

        foreach (var player in PhotonNetwork.playerList)
            playerCount.text += player.NickName + "–";

        //playerCount.text = PhotonNetwork.room.PlayerCount.ToString();
        playerName.text = PlayerPrefs.GetString("PlayerName", "Unnamed");

        SetupGame();
        inputField.onValueChanged.AddListener(delegate
        {

        });
    }

    // Update is called once per frame
    void Update()
    {
        if (restartGame > -1f)
        {
            if (_restarting)
            {
                textField.text = "Restarts in: " + restartGame.ToString();
                restartGame -= Time.deltaTime;
                if (restartGame <= 0)
                {
                    //Debug.Log(restartGame);
                    restartGame = -1f;
                    textField.text = "";
                    SetupGame();
                }
            }else{
				EndGame();
            }
        }
    }

    void GenerateTextfield()
    {
        string[] alphabet = new string[26] {
            "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z"
           // "!","\"","#","€","%","&","/","(","=","@","£","$","∞","§","[","]","}","{","±","™","•","Ω","ı","®","ç","√"
        };
        string firstAlpha, secondAlpha, thirdAlpha;

        for (var i = 0; i < fullStringArray.Length; i++)
        {
            string middleRandos = "!+<[%,=]&->|'.=~(/)*{?@a^";
            firstAlpha = alphabet[UnityEngine.Random.Range(0, 10)];
            secondAlpha = alphabet[UnityEngine.Random.Range(0, 20)];
            thirdAlpha = alphabet[UnityEngine.Random.Range(9, 25)];
            string alphaString = firstAlpha + secondAlpha + thirdAlpha;
            string beginning = "0x0" + Math.Round(i * 2.5).ToString() + alphaString + "  ";

            middleRandos = new string(Enumerable.Repeat(middleRandos, 50).Select(s => s[rnd.Next(s.Length)]).ToArray());
			middleRandos = RefactorStringWithWord(middleRandos, i);

            string fullGenerated = beginning + middleRandos;
            fullStringArray[i] = fullGenerated;
        }
		byte[] data = new byte[8];
		rng.GetBytes(data);
		ulong value = BitConverter.ToUInt64(data, 0);
        var endPos = (playedWordsArray.Length);
		var result = (int)(value % (float)endPos);
        correctWord = playedWordsArray[result];
        Debug.Log(correctWord);
    }

    private string RefactorStringWithWord(string wordToRefactor, int position)
    {
        string newString, playedWord;
        //System.Random rnd = new System.Random();
        GameWords gw = new GameWords();

        playedWord = gw.gameWords[rnd.Next(0, gw.gameWords.Length)].ToUpper();
        playedWordsArray[position] = playedWord;

        char[] refactorArray = wordToRefactor.ToCharArray();
        char[] newWordArray = playedWord.ToCharArray();

		byte[] data = new byte[8];
		rng.GetBytes(data);
		ulong value = BitConverter.ToUInt64(data, 0);
        var endPos = (refactorArray.Length - newWordArray.Length) + 1;
		var result = (int)(value % (float)endPos);

        var randomStartIndex = result;
        var randomPos = rnd.Next(5, 8);
        for (var i = 0; i < refactorArray.Length; i++){
            
            //Debug.Log("Refactor: " + refactorArray[randomPos + i] + " Playedword: " + newWordArray[i]);
            refactorArray[randomStartIndex + i] = newWordArray[i];

            if ((i + 1) >= newWordArray.Length)
				break;
        }
        newString = new string(refactorArray);
        //Debug.Log("NEwstring: " + newString);
        return newString;
	}

    void SetupGame()
    {
        var rows = 10;
        inputField.Select();
        inputField.ActivateInputField();
        gameText.color = Color.green;
        GenerateTextfield();

        for (var i = 0; i < rows; i++)
        {
            if (i == 0)
                gameText.text = fullStringArray[i] + "\n";
            else
                gameText.text += fullStringArray[i] + "\n";
        }
    }

    void EndGame()
    {
		if (restartGame > -1f)
		{
			textField.text = "Ends in: " + restartGame.ToString();
			restartGame -= Time.deltaTime;
			if (restartGame <= 0)
			{
				//Debug.Log(restartGame);
				restartGame = -1f;
				textField.text = "";

				GameManager.Instance.LeaveRoom();
			}
		}
    }

    void OnGUI()
    {
        //TODO: VARFÖR KÖRS DENNA TRE GÅNGER??
        GUI.backgroundColor = Color.black;
        if (inputField.isFocused && inputField.text != "" && Input.GetKey(KeyCode.Return))
        {
            attempts++;
            GameManager._attempts = attempts;
            if (inputField.text == correctWord)
            {
                correctAttempts++;
                
                if(correctAttempts >= 3)
                {
                    gameText.color = Color.green;
                    gameText.text = "CORRECT!";
                    _restarting = true;
                    restartGame = 10f;
                    attemptText.text += "\nCorrect: " + inputField.text + ". " + correctAttempts.ToString() + " / 3 correct.";
                    inputField.text = "";
					correctAttempts = 0;
                }
                else
                {
                    attemptText.text += "\nCorrect: " + inputField.text + ". " + correctAttempts.ToString() + " / 3 correct.";
                    inputField.text = "";
                    _restarting = true;
                    restartGame = 1f;
                }
            }
            else
            {
                if(attempts < 3)
                {
					gameText.color = Color.red;
					gameText.text = "WRONG! TRY AGAIN";
					attemptText.text += "\nWrong attempt: " + inputField.text;
					inputField.text = "";  
                    _restarting = true;
					restartGame = 2f;
                }
                else
                {
					gameText.color = Color.red;
					gameText.text = "Sorry! You failed.. Leaving game now!";
					attemptText.text += "\nWrong attempt: " + inputField.text;
					inputField.text = "";
                    _restarting = false;
                    restartGame = 3f;
                }

            }
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
//using Newtonsoft.Json;
using UnityEngine.UI;

using System.Collections;
using UnityEngine.SceneManagement;

public class PlayfabManager : MonoBehaviour { 

    [Header("UI")]
    public Text messageText;
    public InputField emailInput;
    public InputField passwordInput;

    //register/login/reset
    public void RegisterButton() {
        if (passwordInput.text.Length < 6) {
            messageText.text = "Password troppo corta!";
            return;
        }

        var request = new RegisterPlayFabUserRequest {
            Email = emailInput.text,
            Password = passwordInput.text,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
    }

     void OnRegisterSuccess(RegisterPlayFabUserResult result){
        messageText.text = "Registrazione eseguita!";
        StartCoroutine(ChangeSceneAfterDelay(2f));
    }
    
    IEnumerator ChangeSceneAfterDelay(float delayInSeconds)
    {
        yield return new WaitForSeconds(delayInSeconds);
        SceneManager.LoadScene(0);
    }

    public void LoginButton() {
        var request = new LoginWithEmailAddressRequest {
            Email = emailInput.text,
            Password = passwordInput.text
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);        
    }

    void OnLoginSuccess(LoginResult result) {
        messageText.text = "Login eseguito!";
        StartCoroutine(ChangeSceneAfterDelay(2f));
    }

    public void ResetPasswordButton() {
         var request = new SendAccountRecoveryEmailRequest {
            Email = emailInput.text,
            TitleId =  "21610"
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnPasswordReset, OnError);
    }

    void OnPasswordReset(SendAccountRecoveryEmailResult result) {
        messageText.text = "Inviata Email di recupero!";
    }

    void OnError(PlayFabError error) {
        messageText.text = error.ErrorMessage;
        Debug.Log(error.GenerateErrorReport());
    }

    //CLASSIFICA
    // public void SendLeaderboard(int score) {
    //     var request = new UpdatePlayerStatisticsRequest {
    //         Statistics = new List<StatisticUpdate> {
    //             new StatisticUpdate {
    //                 StatisticName = "PlatformScore",
    //                 Value = score
    //             }
    //         }
    //     };
    //     PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnError);
    // }
    // void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result) {
    //     Debug.Log("Successfull leaderboard sent");
    // }
}
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Tables;

public class GameOver : MonoBehaviour
{
    public Text scoreText;
    private Animation anim;
    public Text highscoreText;
    public LocalizedString highscoreLabel;
    public LocalizedString scoreLabel;
    public LocalizedString points;

    void Start()
    {
        anim = GetComponent<Animation>();
        gameObject.SetActive(false); // All'avvio dell'oggetto, lo rendiamo invisibile
    }

    public void SetUp(int score)
    {
        gameObject.SetActive(true); // Rendiamo l'oggetto visibile

        string localizedScore = scoreLabel.GetLocalizedString();
        string localizedPoints = points.GetLocalizedString();
        scoreText.text = $"{localizedScore}: {score.ToString()} {localizedPoints}";

        if (anim != null)
        {
            StartCoroutine(PlayAnimationsInSequence("scaleIn", "SkullAnim"));
        }
    }

    private IEnumerator PlayAnimationsInSequence(params string[] animNames)
    {
        foreach (string animName in animNames)
        {
            anim.Play(animName);
            yield return new WaitForSeconds(anim[animName].length); // Attendi la fine dell'animazione corrente
        }
    }

    public void SetHighscore(int highscore)
    {
        string localizedHighscoreLabel = highscoreLabel.GetLocalizedString();
        string localizedPoints = points.GetLocalizedString();
        highscoreText.text = $"{localizedHighscoreLabel}: {highscore.ToString()} {localizedPoints}";
    }
}

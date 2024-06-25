using System.Collections;
using System.Collections.Generic;
using TMPro.SpriteAssetUtilities;
using TMPro;
using UnityEngine;
using System.Linq;

public class FeedbackScreen : MonoBehaviour
{
    public static FeedbackScreen Instance;

    public GameObject feedback_Bad;
    public GameObject feedback_Good;
    public GameObject feedback_Free;
    public TextMeshProUGUI uiText_Likes;

    public MeshRenderer rend;

    private Photo submittedPhoto;

    private void Awake() {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        feedback_Bad.SetActive(false);
        feedback_Good.SetActive(false);
        feedback_Free.SetActive(false);

    }

    public void SetPhoto(Photo photo) {
        rend.material.mainTexture = photo.GetTexture();
        submittedPhoto = photo;
        feedback_Bad.SetActive(false);
        feedback_Good.SetActive(false);
        feedback_Free.SetActive(false);
    }

    public void SetFree() {
        feedback_Bad.SetActive(false);
        feedback_Good.SetActive(false);
        feedback_Free.SetActive(true);

        List<Item> items = new List<Item>();
        foreach (var item in submittedPhoto.items) {
            items.Add(item);
        }

        int score = 0;
        int safe = 0;
        string article = "";
        while ( items.Count > 0) {

            ++safe;
            if ( safe >= 10) {
                Debug.LogError("free pîcture loop error");
                break;
            }

            if ( items.Count == 1) {
                article += $"{items[0].phrase_setup} !\n";
                break;
            }

            int index = Random.Range(0, items.Count - 1);
            var item1 = items[index];
            var item2 = items[index+1];

            string phrase = Random.value > 0.5f ? item1.phrase_start : item1.phrase_end ;
            phrase = phrase.Replace("[ITEM2]", item2.phrase_setup);
            article += $"{phrase}\n";

            items.RemoveAt(index);
            items.RemoveAt(index);
        }

        uiText_Likes.text = Random.Range(0, 100).ToString();

        Boss.Instance.StartDialog(article);
    }

    public void SetGood() {
        feedback_Bad.SetActive(false);
        feedback_Good.SetActive(true);
        feedback_Free.SetActive(false);
    }

    public void SetBad() {
        feedback_Bad.SetActive(true);
        feedback_Good.SetActive(false);
        feedback_Free.SetActive(false);
    }

}

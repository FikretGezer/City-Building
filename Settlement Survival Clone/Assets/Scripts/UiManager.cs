using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiManager : MonoBehaviour
{
    [SerializeField] Image img;
    [SerializeField] TextMeshProUGUI speedText;
    [SerializeField] TMP_Text popUpNewTech;
    [SerializeField] TMP_Text popUpMonthText;

    public static bool popUpMonth;
    bool newTechArrived;
    public static Queue<string> popUp = new Queue<string>();
    public static float aMonthForAmountOfMinutes;

    float popTextShowTime = 50f;//10'un katlar�n� yapzmam�n sebebi 10x h�zda 5,10,.. sn s�rs�n diye.

    float timeMultiplier = 3f;
    float perSecondInGame = 1f;
    float fillAmount;
    

    // Start is called before the first frame update
    void Start()
    {
        img.fillAmount = 0;
        fillAmount = 1 / (3 * aMonthForAmountOfMinutes);
    }

    // Update is called once per frame
    void Update()
    {
        NewTechUpdate();
        if(popUpMonth)
        {
            StartCoroutine(CountDown(popUpMonthText));
            popUpMonth = false;
        }
        if(newTechArrived)
        {
            StartCoroutine(CountDown(popUpNewTech));
            newTechArrived = false;
        }
    }
    void NewTechUpdate()
    {
        perSecondInGame -= Time.deltaTime;
        if (perSecondInGame <= 0)
        {            
            img.fillAmount += fillAmount;
            if (img.fillAmount >= 1f)
            {
                img.fillAmount = 0f;
                popUp.Enqueue("New Tech");
                newTechArrived = true;
            }
                
            perSecondInGame = 1f;
        }
        //img.fillAmount += (0.00003f*timeMultiplier)*Time.deltaTime;//bir ayda 43200 dakika oldu�u i�in fillAmount alabilece�i maks. de�er olan 1'i 43200'e b�l�nce bu de�er ��k�yor.2 de yar�m saniye 1 dk ge�ti�i i�in
        //speedText.text = ((int)Time.timeScale).ToString() + "x";
    }
    void ShowPopUp(string text,TMP_Text tmp)
    {
        tmp.transform.parent.gameObject.SetActive(true);
        tmp.text = $"{text} arrived.";
    }
    IEnumerator CountDown(TMP_Text tmp)
    {
        ShowPopUp(popUp.Dequeue(),tmp);
        yield return new WaitForSeconds(popTextShowTime);
        tmp.transform.parent.gameObject.SetActive(false);
        StopCoroutine(CountDown(tmp));
    }
}

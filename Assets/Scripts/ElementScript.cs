using UnityEngine;
using TMPro;

public class ElementScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool state=false;
    public GameObject bifa;
    public TMP_Text textName;
    public GameManager gameManager;
    public GameObject countPart;
    //or multiple
    public TMP_Text countText;
    public bool hasFactor=false;
    public int nr=0;
    public string plantName;
    //public GameObject plusButton,minusButton;
    void Awake()
    {
        nr=0;
        if(state==false && bifa!=null)
            bifa.SetActive(false);
        //this for "add with factor" only
        if(state==false && countPart!=null)
            countPart.SetActive(false);
    }
    public void bifat(bool state)//is used when created
    {
        this.state=state;
        bifa.SetActive(state);
    }
    public void bifat(int nr,bool state)//is used when created
    {
        this.nr=nr;
        Debug.Log("bifat "+nr+" "+plantName);
        this.state=state;
        countText.text=nr.ToString();
        bifa.SetActive(state);
        if(countPart!=null)
            countPart.SetActive(state);
    }

    public void pressAndSendFactor()//this function also sends the factor field
    {
        if(state==false)
        {
            gameManager.selectPlant(textName.text,nr);
            bifa.SetActive(true);
        }
        else
        {
            gameManager.deselectPlant(textName.text,nr);
            bifa.SetActive(false);
        }
        state=!state;
        if(countPart!=null)
            countPart.SetActive(state);
    }
    public void press()
    {
        if(state==false)
        {
            gameManager.selectPlant(textName.text);
            bifa.SetActive(true);
        }
        else
        {
            gameManager.deselectPlant(textName.text);
            bifa.SetActive(false);
        }
        state=!state;
    }
    public void pressAndHaveOnlyThisPlant()
    {
        if(state==false)
        {
            gameManager.selectOnlyThisPlant(textName.text);
            bifa.SetActive(true);
        }
        else
        {
            gameManager.deselectThisPlant(textName.text);
            bifa.SetActive(false);
        }
        state=!state;
    }
    public void pressPlusAndSendFactor()
    {
        nr++;
        countText.text=nr.ToString();
        gameManager.selectPlant(textName.text,nr);
    }
    public void pressPlus()
    {
        nr++;
        countText.text=nr.ToString();
        gameManager.selectPlantMultiple(textName.text);
    }
    public void pressMinusAndSendFactor()
    {
        if(nr==0)
            return;
        nr--;
        countText.text=nr.ToString();
        gameManager.selectPlant(textName.text,nr);
    }
    public void pressMinus()
    {
        if(nr==0)
            return;
        nr--;
        countText.text=nr.ToString();
        gameManager.deselectPlantMultiple(textName.text);
    }
}

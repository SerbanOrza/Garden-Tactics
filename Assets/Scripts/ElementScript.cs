using UnityEngine;
using TMPro;

public class ElementScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool state=false;
    public GameObject bifa;
    public TMP_Text textName;
    public GameManager gameManager;
    //or multiple
    public TMP_Text countText;
    public int nr=0;
    //public GameObject plusButton,minusButton;
    void Start()
    {
        nr=0;
        if(state==false && bifa!=null)
            bifa.SetActive(false);
    }
    public void bifat(bool state)
    {
        this.state=state;
        bifa.SetActive(state);
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
    public void pressPlus()
    {
        nr++;
        countText.text=nr.ToString();
        gameManager.selectPlantMultiple(textName.text);
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

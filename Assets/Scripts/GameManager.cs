using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    public List<Plant> allPlants=new List<Plant>();
    public Dictionary<string,Plant> dict=new Dictionary<string,Plant>();
    public List<Plant> goodVList=new List<Plant>();
    public List<Plant> badVList=new List<Plant>();
    public List<Plant> selectedPlants=new List<Plant>();
    public int selectingState=0;
    public GameObject contentViewListMultiple,elementPrefab,elementMultiplePrefab;
    public GameObject contentViewListAdd,contentViewListRemove;
    public GameObject contentViewListSol,elementSolPrefab;
    public List<GameObject> panels=new List<GameObject>();
    public GameObject currentPanel;
    public PlantGraph plantGraph;
    //texts
    public GameObject addGoodText,addBadText;
    public GameObject addBackButton,addNextButton,addFinishButton,addOrderButton,tryAgainButton;
    public GameObject scrollMultiple,scrollSol;
    public TMP_InputField inputName;
    public TMP_Text plantsSelectedText,ansText,timeSearchedText;
    public Color greenColor,redColor;
    void Start()
    {
        Plant p1=new Plant("Ardei Capia");
        Plant p2=new Plant("Varza");
        Plant p3=new Plant("Rosii mici");
        Plant p4=new Plant("Rosii negre");
        Plant p5=new Plant("Vinete albe");
        Plant p6=new Plant("Morcovi");
        p3.veciniRai.Add(p4);
        p3.veciniRai.Add(p5);
        p3.veciniRai.Add(p6);

        p4.veciniRai.Add(p3);
        p5.veciniRai.Add(p3);
        p6.veciniRai.Add(p3);

        p1.veciniRai.Add(p2);
        p2.veciniRai.Add(p1);

        p4.veciniBuni.Add(p6);
        p6.veciniBuni.Add(p4);

        List<Plant> list=new List<Plant>();
        allPlants.Add(p1);
        allPlants.Add(p2);
        allPlants.Add(p3);
        allPlants.Add(p4);
        allPlants.Add(p5);
        allPlants.Add(p6);
        //allPlants=list;
       // allPlants.plants=list;
        
        //create the dictionary with all plants
        foreach(Plant p in allPlants)
            dict[p.name]=p;
        //scrollView.SetActive(false);
        foreach(GameObject g in panels)
            g.SetActive(false);
        moveToPanel(0);
    }
    public void showAddMenu()
    {
        moveToPanel(1);
        changeSelectingState(1);
        //scrollView.SetActive(true);
    }
    public void showCreateOrderMenu()
    {
        goodVList.Clear();
        badVList.Clear();
        moveToPanel(2);
        //selectingState=1;
        addOrderButton.SetActive(true);
        tryAgainButton.SetActive(false);
        scrollMultiple.SetActive(true);
        scrollSol.SetActive(false);
        searchingPlants=false;
        ansText.text="";
        timeSearchedText.text="";
        showListToChooseInMultiple();
        //selectingState=2;

        //changeSelectingState(2);
    }
    public List<string> ansForOrder;
    bool searchingPlants=false;
    public async void orderPlants()
    {
        //something with graph
        ansText.text="";
        addOrderButton.SetActive(false);
        searchingPlants=true;
        ansForOrder=await plantGraph.createOrder(goodVList);
        
        searchingPlants=false;
        tryAgainButton.SetActive(true);
        scrollMultiple.SetActive(false);
        //show answers
        showFinalOrderList();
        scrollSol.SetActive(true);
    }
    public void tryAgain()
    {
        addOrderButton.SetActive(true);
        tryAgainButton.SetActive(false);

        scrollMultiple.SetActive(true);
        scrollSol.SetActive(false);

    }
    public void changeSelectingState(int k)
    {
        addGoodText.SetActive(false);
        addBadText.SetActive(false);
        addNextButton.SetActive(false);
        addBackButton.SetActive(false);
        addFinishButton.SetActive(false);
        if(k==1)
        {
            selectingState=k;
            showListToChoose(1,contentViewListAdd);
            addGoodText.SetActive(true);

            addNextButton.SetActive(true);
        }
        else
        if(k==-1)
        {
            selectingState=k;
            showListToChoose(-1,contentViewListAdd);
            addBadText.SetActive(true);

            addBackButton.SetActive(true);
            addFinishButton.SetActive(true);
        }
    }
    public void finishAddingPlant()
    {
        Plant p=new Plant(inputName.text,new List<Plant>(goodVList),new List<Plant>(badVList));
        allPlants.Add(p);
        dict[p.name]=p;
        //for every plant in badVlist, mark this plant p as bad neighbor
        foreach(Plant v in goodVList)
        {
            if(!v.veciniBuni.Exists(x=>x.name==p.name))
                v.veciniBuni.Add(p);
        }
        foreach(Plant v in badVList)
        {
            if(!v.veciniRai.Exists(x=>x.name==p.name))
                v.veciniRai.Add(p);
        }
        goodVList.Clear();
        badVList.Clear();
        selectingState=0;
        //scrollView.SetActive(false);
        moveToPanel(0);
    }

    void Update()
    {
        
    }
    public void exitApp()
    {
        Application.Quit();
    }
    public void moveToPanel(int k)
    {
        if(k==0)
            selectingState=0;
        currentPanel.SetActive(false);
        currentPanel=panels[k];
        currentPanel.SetActive(true);
    }
    public void selectPlant(string name)
    {
        if(selectingState==1)//select for good list
        {
            if(!goodVList.Exists(x=>x.name==name))
                goodVList.Add(dict[name]);
        }
        else
        if(selectingState==-1)//select for bad list
        {
            if(!badVList.Exists(x=>x.name==name))
                badVList.Add(dict[name]);
        }
    }
    public void deselectPlant(string name)
    {
        if(selectingState==1)
        {
            if(goodVList.Exists(x=>x.name==name))
                goodVList.Remove(dict[name]);
        }
        else
        if(selectingState==-1)
        {
            if(badVList.Exists(x=>x.name==name))
                badVList.Remove(dict[name]);
        }
    }
    public void selectPlantMultiple(string name)
    {
        if(searchingPlants)
            return;
        goodVList.Add(dict[name]);
        plantsSelectedText.text="Plants selected: "+goodVList.Count.ToString();
    }
    public void deselectPlantMultiple(string name)
    {
        if(searchingPlants)
            return;
        if(goodVList.Exists(x=>x.name==name))
            goodVList.Remove(dict[name]);
        plantsSelectedText.text="Plants selected: "+goodVList.Count.ToString();
    }
    public void showDeleteMenu()
    {
        badVList.Clear();
        moveToPanel(3);
        selectingState=-1;//-1 means we will select into badList
        showListToChoose(-1,contentViewListRemove);

    }
    public void clearGoodBadLists()
    {
        goodVList.Clear();
        badVList.Clear();
    }
    public void pressDeletePlants()
    {
        foreach(Plant p in badVList)
            deletePlant(p.name);
        badVList.Clear();
        moveToPanel(0);
    }
    public void deletePlant(string name)
    {
        if(dict.ContainsKey(name)==false)
            return;
        Plant p=dict[name];
        foreach(Plant a in p.veciniBuni)
            if(a.veciniBuni.Exists(x=>x.name==p.name))
                a.veciniBuni.Remove(p);
        foreach(Plant a in p.veciniRai)
            if(a.veciniRai.Exists(x=>x.name==p.name))
                a.veciniRai.Remove(p);
        allPlants.Remove(p);
        dict.Remove(p.name);
        //save allPlants
    }
    public void showListToChoose(int k,GameObject contentViewList) //when selecting plants
    {
        foreach(Transform c in contentViewList.transform)
            Destroy(c.gameObject);
        foreach(Plant p in allPlants)
        {
            GameObject g=Instantiate(elementPrefab,contentViewList.transform);
            ElementScript el=g.GetComponent<ElementScript>();
            el.textName.text=p.name;
            el.gameManager=this;
            g.SetActive(true);
            if(k==1)//show good neighbors as selected
            {
                if(goodVList.Exists(x=>x.name==p.name))
                    el.bifat(true);
                //if it is in the other list
                if(badVList.Exists(x=>x.name==p.name))//mark the button as disabled
                    g.GetComponent<Button>().interactable=false;
            }
            else
            if(k==-1)//show bad neighbors as selected
            {
                if(badVList.Exists(x=>x.name==p.name))
                    el.bifat(true);
                //if it is in the other list
                if(goodVList.Exists(x=>x.name==p.name))//mark the button as disabled
                    g.GetComponent<Button>().interactable=false;
            }
        }
    }
    public void showListToChooseInMultiple() //when selecting plants for ordering
    {
        foreach(Transform c in contentViewListMultiple.transform)
            Destroy(c.gameObject);
        foreach(Plant p in allPlants)
        {
            GameObject g=Instantiate(elementMultiplePrefab,contentViewListMultiple.transform);
            ElementScript el=g.GetComponent<ElementScript>();
            el.textName.text=p.name;
            el.gameManager=this;
            g.SetActive(true);
        }
    }
    public void showFinalOrderList() //answers
    {
        foreach(Transform c in contentViewListSol.transform)
            Destroy(c.gameObject);
        int i=1;
        foreach(string p in ansForOrder)
        {
            Debug.Log(p);
            GameObject g=Instantiate(elementSolPrefab,contentViewListSol.transform);
            ElementScript el=g.GetComponent<ElementScript>();
            el.textName.text=p;
            el.countText.text=i.ToString()+")";
            //we are on plant i-1 in ansForOrder
            
            if(i-2>=0 && dict[p].veciniBuni.Exists(x=>x.name==ansForOrder[i-2])) 
                g.GetComponent<Image>().color=greenColor;
            else
            if(i<ansForOrder.Count && dict[p].veciniBuni.Exists(x=>x.name==ansForOrder[i]))
                g.GetComponent<Image>().color=greenColor;
            el.gameManager=this;
            g.SetActive(true);
            i++;
        }
    }
}

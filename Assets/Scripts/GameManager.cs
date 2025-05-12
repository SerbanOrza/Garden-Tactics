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
    public List<int> friendshipValues=new List<int>();
    public List<Plant> badVList=new List<Plant>();
    public List<Plant> selectedPlants=new List<Plant>();
    public int selectingState=0;
    public GameObject contentViewListMultiple,elementPrefab,elementAddWithFactorPrefab,elementMultiplePrefab;
    public GameObject contentViewListAdd,contentViewListRemove;
    public GameObject contentViewListSol,elementSolPrefab;
    public GameObject contentShowPlants;
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
        p3.veciniRai.Add(p4.name);
        p3.veciniRai.Add(p5.name);
        p3.veciniRai.Add(p6.name);

        p4.veciniRai.Add(p3.name);
        p5.veciniRai.Add(p3.name);
        p6.veciniRai.Add(p3.name);

        p1.veciniRai.Add(p2.name);
        p2.veciniRai.Add(p1.name);

        p4.veciniBuni.Add(p6.name);
        p4.friendshipValues.Add(1);
        
        p6.veciniBuni.Add(p4.name);
        p6.friendshipValues.Add(1);

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
    /***
        This function shows the add menu to the screen and prepares the menu to add a new plant.
    ***/
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
    }
    public void showAllPlantsMenu()
    {
        moveToPanel(4);
        showThesePlants(convertToNames(allPlants),elementSolPrefab,contentShowPlants);
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

    //this is used only in "add plant"
    public void changeSelectingState(int k)
    {
        addGoodText.SetActive(false);
        addBadText.SetActive(false);
        addNextButton.SetActive(false);
        addBackButton.SetActive(false);
        addFinishButton.SetActive(false);
        if(k==1)//select good plants
        {
            selectingState=k;
            //show the multiple way variant
            showListToChoose(1,elementAddWithFactorPrefab,contentViewListAdd);
            addGoodText.SetActive(true);

            addNextButton.SetActive(true);
        }
        else
        if(k==-1)//select bad plants
        {
            selectingState=k;
            showListToChoose(-1,elementPrefab,contentViewListAdd);
            addBadText.SetActive(true);

            addBackButton.SetActive(true);
            addFinishButton.SetActive(true);
        }
    }
    public void finishAddingPlant()
    {
        if(inputName.text=="")
        {
            //error, name must not be empty! Do not add the plant.
            clearGoodBadLists();
            selectingState=0;
            moveToPanel(0);
            Debug.Log("empty name not allowed!");
            return;
        }
        if(dict.ContainsKey(inputName.text))//already a plant with this name
        {
            clearGoodBadLists();
            selectingState=0;
            moveToPanel(0);
            Debug.Log("same name not allowed!");
            return;
        }
        Plant p=new Plant(inputName.text,new List<Plant>(goodVList),new List<int>(friendshipValues),new List<Plant>(badVList));
        allPlants.Add(p);
        dict[p.name]=p;
        //for every plant v in goodVList, mark our plant p to be good for v.
        foreach(Plant v in goodVList)
        {
            if(!v.veciniBuni.Exists(x=>x==p.name))//if not already there
            {
                v.veciniBuni.Add(p.name);
                int index=goodVList.FindIndex(x=>x.name==v.name);
                v.friendshipValues.Add(friendshipValues[index]);
            }
        }
        //for every plant in badVlist, mark this plant p as bad neighbor
        foreach(Plant v in badVList)
        {
            if(!v.veciniRai.Exists(x=>x==p.name))//if not already there
                v.veciniRai.Add(p.name);
        }
        clearGoodBadLists();
        selectingState=0;
        //scrollView.SetActive(false);
        moveToPanel(0);
    }

    void Update()
    {
        
    }
    public List<string> convertToNames(List<Plant>plants)
    {
        List<string> ans=new List<string>();
        foreach(Plant p in plants)
            ans.Add(p.name);
        return ans;
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
    public void selectPlant(string name,int factor)//used in add panel
    {
        if(selectingState==1)//select for good list
        {
            int i=goodVList.FindIndex(x=>x.name==name);
            if(i==-1)
            {
                goodVList.Add(dict[name]);
                friendshipValues.Add(factor);
            }
            else
            {
                //update
                friendshipValues[i]=factor;
            }
        }
        else //we would never reach this part
        if(selectingState==-1)//select for bad list
        {
            if(!badVList.Exists(x=>x.name==name))
                badVList.Add(dict[name]);
            Debug.Log("error in selectPlant with factor!");
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
    public void deselectPlant(string name,int factor)
    {
        if(selectingState==1)
        {
            if(goodVList.Exists(x=>x.name==name))
            {
                int i=goodVList.IndexOf(dict[name]);
                friendshipValues.RemoveAt(i);
                goodVList.Remove(dict[name]);
            }
        }
        else
        if(selectingState==-1)
        {
            if(badVList.Exists(x=>x.name==name))
                badVList.Remove(dict[name]);
            Debug.Log("error in deselectPlant with factor!");
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
        showListToChoose(-1,elementPrefab,contentViewListRemove);

    }
    public void clearGoodBadLists()
    {
        goodVList.Clear();
        badVList.Clear();
        friendshipValues.Clear();
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
        foreach(string a in p.veciniBuni)
        {
            Plant pa=dict[a];
            if(pa.veciniBuni.Exists(x=>x==p.name))
            {
                int i=pa.veciniBuni.IndexOf(p.name);
                pa.friendshipValues.RemoveAt(i);
                pa.veciniBuni.Remove(p.name);
            }
        }
        foreach(string a in p.veciniRai)
        {
            Plant pa=dict[a];
            if(pa.veciniRai.Exists(x=>x==p.name))
                pa.veciniRai.Remove(p.name);
        }
        allPlants.Remove(p);
        dict.Remove(p.name);
        //save allPlants
    }
    public void showListToChoose(int k,GameObject rowPrefab,GameObject contentViewList) //when selecting plants
    {
        foreach(Transform c in contentViewList.transform)
            Destroy(c.gameObject);
        foreach(Plant p in allPlants)
        {
            GameObject g=Instantiate(rowPrefab,contentViewList.transform);
            ElementScript el=g.GetComponent<ElementScript>();
            el.textName.text=p.name;
            el.gameManager=this;
            int factor=0;
            if(el.hasFactor)//only for good neighbors
            {
                int i=goodVList.IndexOf(dict[el.textName.text]);
                if(i!=-1)
                    factor=friendshipValues[i];
                //else it means it is not checked to the factor is 0
            }
            g.SetActive(true);
            if(k==1)//show good neighbors as selected
            {
                if(goodVList.Exists(x=>x.name==p.name))
                {
                    if(el.hasFactor)
                        el.bifat(factor,true);
                    else
                        el.bifat(true);
                }
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
    /***
        This methods puts the plant names in the chosen container.
    ***/
    public void showThesePlants(List<string> plantsNames,GameObject elementPrefab,GameObject whichContainer)
    {
        foreach(Transform c in whichContainer.transform)
            Destroy(c.gameObject);
        int i=1;
        foreach(string p in plantsNames)
        {
            GameObject g=Instantiate(elementPrefab,whichContainer.transform);
            ElementScript el=g.GetComponent<ElementScript>();
            el.textName.text=p;
            el.countText.text=i.ToString()+")";
            //we are on plant i-1 in ansForOrder
            el.gameManager=this;
            g.SetActive(true);
            i++;
        }
    }
    public void showFinalOrderList() //answers. A method that shows what is in
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
            
            if(i-2>=0 && dict[p].veciniBuni.Exists(x=>x==ansForOrder[i-2])) 
                g.GetComponent<Image>().color=greenColor;
            else
            if(i<ansForOrder.Count && dict[p].veciniBuni.Exists(x=>x==ansForOrder[i]))
                g.GetComponent<Image>().color=greenColor;
            el.gameManager=this;
            g.SetActive(true);
            i++;
        }
    }
}

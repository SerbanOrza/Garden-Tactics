using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class PlantGraph : MonoBehaviour
{
    private List<Node> nodes;
    public int n;
    private Stack<string> currentOrder;
    public int nrElements;
    public List<string> bestOrder;
    public int currentScore;
    public int maxScore,maxPossibleScore;
    public TMP_Text timeSearchedText,scoreText;
    private bool isRunning;
    private Dictionary<(int, int), int> memo;
    private Dictionary<(int, int), int> prevChoice;
    void Start()
    {
        
    }
    public void createNodes(List<Plant> plants)//plants can be repeated
    {
        nodes=new List<Node>();
        n=plants.Count;
        maxPossibleScore=0;
        int t=0;
        foreach(Plant p in plants)
        {
            nodes.Add(new Node(p,t));
            t++;
        }
        for(int i=0;i<n;i++)//for plant nodes[i].plant
        {
            for(int j=0;j<n;j++)//for plant nodes[j].plant
            {
                if(i==j)
                {
                    //nodes[i].neighbors.Add(nodes[j]);
                    nodes[i].costs.Add(0);

                }
                else
                if(nodes[i].plant.veciniBuni.Exists(x=>x==nodes[j].plant.name))//plant j is good for i
                {
                    int index=nodes[i].plant.veciniBuni.FindIndex(x=>x==nodes[j].plant.name);
                    nodes[i].neighbors.Add(nodes[j]);
                    nodes[i].costs.Add(nodes[i].plant.friendshipValues[index]);//good factor

                    //nodes[j].neighbors.Add(nodes[i]);
                    //nodes[j].costs.Add(2);

                    maxPossibleScore+=nodes[i].plant.friendshipValues[index];
                }
                else //neutral or if they have the same name
                if(!nodes[i].plant.veciniRai.Exists(x=>x==nodes[j].plant.name))//they are neither bad nor good
                {
                    //if the plants are the same, I considered them to be in this case
                    nodes[i].neighbors.Add(nodes[j]);
                    nodes[i].costs.Add(0);

                    //nodes[j].neighbors.Add(nodes[i]);
                    //nodes[j].costs.Add(0);
                }
                else //bad
                {

                    nodes[i].badNeighbors.Add(j);
                    //nodes[j].badNeighbors.Add(i);

                    nodes[i].costs.Add(-100000);
                    //nodes[j].costs.Add(-100000);
                }
            }
        }
    }
    bool abandon=true;
    private List<string> extractOrder()
    {
        List<string> order = new List<string>();
        int mask = 0;
        int lastUsed = -1;

        // Make sure to check that prevChoice[(mask, -1)] is properly initialized at the start
        while (mask != (1 << n) - 1)
        {
            int bestChoice = -1;

            // If we have already computed a previous choice
            if (prevChoice.ContainsKey((mask, lastUsed)))
            {
                bestChoice = prevChoice[(mask, lastUsed)];
            }
            else
            {
                // In case we can't find a valid choice, we should break to avoid infinite loop
                break;
            }

            order.Add(nodes[bestChoice].plant.name); // Add plant name to order
            mask |= (1 << bestChoice); // Mark the plant as placed
            lastUsed = bestChoice; // Update lastUsed
        }

        return order;
    }
    private int findBestOrderDP(int mask, int lastUsed)
    {
        // Base case: if all plants are placed, return 0 (no more score to add)
        if (mask == (1 << n) - 1) return 0;

        // If this state has been computed before, return the cached result
        if (memo.ContainsKey((mask, lastUsed)))
            return memo[(mask, lastUsed)];

        int bestScore = int.MinValue;
        int bestChoice = -1;

        // Loop through all plants
        for (int i=0;i<n;i++)
        {
            // If plant i is not used yet
            if ((mask & (1 << i)) == 0)
            {
                // Handle first plant placement (when lastUsed == -1)
                //if (lastUsed == -1 || !nodes[lastUsed].badNeighbors.Contains(i)) // Check constraint
                //{
                    int newMask = mask | (1 << i); // Mark this plant as used
                    int score = 0;

                    if (lastUsed == -1)
                        score = findBestOrderDP(newMask, i);
                    else
                    {
                        if(nodes[lastUsed].badNeighbors.Contains(i))
                            continue;
                        else
                            score = nodes[lastUsed].costs[i] + findBestOrderDP(newMask, i);
            
                    }

                    // Keep track of the best score and corresponding choice
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestChoice = i;
                    }
                //}
            }
        }

        // Store the best choice for backtracking
        if (bestChoice != -1)
            prevChoice[(mask, lastUsed)] = bestChoice;

        // Memoize and return the best score
        return memo[(mask, lastUsed)] = bestScore;
    }
    private void findBestOrder(Node node,int depth)
    {
        if(abandon==true)
            return;
        node.used=true;

        if(depth==n)
        {
            if(currentScore>maxScore)
            {
                maxScore=currentScore;
                bestOrder.Clear();
                bestOrder.AddRange(currentOrder);
            }
            node.used=false;
            return;
        }
        if(currentScore==maxPossibleScore)
        {
            abandon=true;
            node.used=false;
            return;
        }
        for(int i=0;i<node.neighbors.Count;i++)
        {
            Node neighbor=node.neighbors[i];
            if(!neighbor.used)
            {
                currentOrder.Push(neighbor.plant.name);
                currentScore+=node.costs[i];

                findBestOrder(neighbor,depth+1);

                currentOrder.Pop();
                currentScore-=node.costs[i];
            }
        }

        node.used=false;
    }

    public async Task<List<string>> createOrder(List<Plant> plants)
    {
        isRunning=true;
        StartCoroutine(calculateTime());
        memo = new Dictionary<(int, int), int>();
        prevChoice = new Dictionary<(int, int), int>();
        createNodes(plants);

        maxScore = -1;
        bestOrder = new List<string>();
        currentOrder = new Stack<string>();
        abandon=false;
        await Task.Run(() =>
        {
            maxScore=findBestOrderDP(0, -1);
            // foreach (Node startNode in nodes)
            // {
            //     currentScore = 0;
            //     currentOrder.Clear();
            //     currentOrder.Push(startNode.plant.name);
            //     findBestOrder(startNode, 1);
            // }
        });
        if(maxScore>=0)
            bestOrder=extractOrder();
        if(maxScore<0)
            scoreText.text="Imposibil, vor fi neaparat vecini rai";
        scoreText.text="Legaturi bune: "+maxScore.ToString();
        
        Debug.Log("Finished, Max Score: " + maxScore);
        isRunning=false;
        //System.GC.Collect();

        return bestOrder;
    }
    public IEnumerator calculateTime()
    {
        float t=0;
        while(isRunning)
        {
            t=t+Time.deltaTime;
            timeSearchedText.text="Searching...\n"+t.ToString()+" seconds";
            yield return null;
        }
        yield break;
    }
}

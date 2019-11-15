using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class ThiefController : MonoBehaviour {

    public NavMeshAgent agent;
    public NavMeshSurface surface;

    public Caravan caravan;
    public GameObject player;

    private Trader traders;

    private bool inWait = false;
    private bool inRange = false;
    private bool inWonderRange = true;
    private bool isActionFinished = false;

    private Vector3 wonderDestination;
    private int preIndex = 0;

    void Start() {
        traders = GameObject.FindGameObjectWithTag("traders").GetComponent<Trader>();
        surface.BuildNavMesh();
        wonderDestination = traders.traderPositions[0];
    }

    // Update is called once per frame
    void Update() {
        if (!inWait) {
           
            StartCoroutine("action");
            
        } else {
            Wondering();
        }
    }

    public IEnumerator action() {

        inRange = false;
        float stoleChance = Random.Range(0f, 1f);

        if (stoleChance <= .33f) {
            float carOrIn = Random.Range(0f, 1f);
            if (carOrIn <= 0.5f) {
                MoveAgent(player.transform.position);

            } else {
                MoveAgent(caravan.transform.position);
            }
        } else {
            Wondering();
        }
        print("Start: " + Time.time);
        inWait = true;
        yield return new WaitForSecondsRealtime(5f);
        inWait = false;
        print("End: " + Time.time);
    }
    public bool MoveAgent(Vector3 position) {
        print(Vector3.Distance(transform.position, wonderDestination));
        agent.SetDestination(position);
        if (Vector3.Distance(transform.position, position) < 5f) {
            inRange = true;
        }

        return inRange;
    }

    public void Wondering() {

        if (inWonderRange) {
            int nexIndex = GetNextIndex(preIndex);
            preIndex = nexIndex;
            agent.SetDestination(traders.traderPositions[nexIndex]);
            wonderDestination = traders.traderPositions[nexIndex];
            inWonderRange = false;
        }

       
        if (Vector3.Distance(transform.position, wonderDestination) < 5f) {
            inWonderRange = true;
        }

    }

    private int GetNextIndex(int preIndex) {
        int nextIndex = Random.Range(0, 8);
        while (preIndex == nextIndex) {
            nextIndex = Random.Range(0, 8);
        }
        return nextIndex;
    }
}

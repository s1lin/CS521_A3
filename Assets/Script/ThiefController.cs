using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class ThiefController : MonoBehaviour {

    public NavMeshAgent agent;
    public NavMeshSurface surface;

    public Caravan caravan;
    public Inventory inventory;
    public GameObject player;
    public Text StealCount;

    private Trader traders;

    private bool inWait = false;
    private bool isCaravan = false;
    private bool isWondering = false;
    private bool inWonderRange = true;
    private bool isActionFinished = true;

    private Vector3 actionDestination;
    private int preIndex = 0;
    private int stealCount = 0;

    void Start() {
        traders = GameObject.FindGameObjectWithTag("traders").GetComponent<Trader>();
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        surface.BuildNavMesh();
        actionDestination = traders.traderPositions[0];
    }

    // Update is called once per frame
    void Update() {
        if (!inWait && isActionFinished) {
            StartCoroutine("action");
        } else {
            if (isWondering || stealCount >= 2)
                Wondering();
            else {
                if (isCaravan) {
                    if (MoveToCaravan() && !isActionFinished) {
                        isActionFinished = StealRandomItemFromCar();
                    }
                } else {
                    if (MoveToPlayer() && !isActionFinished)
                        isActionFinished = StealRandomItemFromInv();
                }
            }
        }
    }
    private bool StealRandomItemFromCar() {

        List<KeyValuePair<SpiceName, int>> values = new List<KeyValuePair<SpiceName, int>>{
            new KeyValuePair<SpiceName, int>(SpiceName.Tu, caravan.GetItemValue(SpiceName.Tu)),
            new KeyValuePair<SpiceName, int>(SpiceName.Sa, caravan.GetItemValue(SpiceName.Sa)),
            new KeyValuePair<SpiceName, int>(SpiceName.Ca, caravan.GetItemValue(SpiceName.Ca)),
            new KeyValuePair<SpiceName, int>(SpiceName.Ci, caravan.GetItemValue(SpiceName.Ci)),
            new KeyValuePair<SpiceName, int>(SpiceName.Cl, caravan.GetItemValue(SpiceName.Cl)),
            new KeyValuePair<SpiceName, int>(SpiceName.Pe, caravan.GetItemValue(SpiceName.Pe)),
            new KeyValuePair<SpiceName, int>(SpiceName.Su, caravan.GetItemValue(SpiceName.Su))
        };

        values.RemoveAll(e => (int)e.Value <= 0);

        if (values.Count != 0) {
            int remove = Random.Range(0, values.Count);
            caravan.RemoveItemByOne(values[remove].Key);
            stealCount++;
            StealCount.text = stealCount.ToString();
            return true;
        }

        return true;

    }

    private bool StealRandomItemFromInv() {

        List<KeyValuePair<SpiceName, int>> values = new List<KeyValuePair<SpiceName, int>>{
            new KeyValuePair<SpiceName, int>(SpiceName.Tu, inventory.GetItemValue(SpiceName.Tu)),
            new KeyValuePair<SpiceName, int>(SpiceName.Sa, inventory.GetItemValue(SpiceName.Sa)),
            new KeyValuePair<SpiceName, int>(SpiceName.Ca, inventory.GetItemValue(SpiceName.Ca)),
            new KeyValuePair<SpiceName, int>(SpiceName.Ci, inventory.GetItemValue(SpiceName.Ci)),
            new KeyValuePair<SpiceName, int>(SpiceName.Cl, inventory.GetItemValue(SpiceName.Cl)),
            new KeyValuePair<SpiceName, int>(SpiceName.Pe, inventory.GetItemValue(SpiceName.Pe)),
            new KeyValuePair<SpiceName, int>(SpiceName.Su, inventory.GetItemValue(SpiceName.Su))
        };

        values.RemoveAll(e => (int)e.Value <= 0);

        if (values.Count != 0) {
            int remove = Random.Range(0, values.Count);
            inventory.RemoveItemByQuanlity(values[remove].Key, 1);
            stealCount++;
            StealCount.text = stealCount.ToString();
            return true;
        }

        return true;
    }

    public IEnumerator action() {

        float stoleChance = Random.Range(0f, 1f);

        if (stoleChance <= .33f && stealCount < 2) {
            isWondering = false;
            isActionFinished = false;
            float carOrIn = Random.Range(0f, 1f);
            //if (carOrIn <= 0.5f) {
            isCaravan = false;
            MoveToPlayer();
            //} else {
            //isCaravan = true;
            //MoveToCaravan();
            //}
        } else {
            isWondering = true;
            Wondering();
        }

        inWait = true;
        yield return new WaitForSecondsRealtime(5f);
        inWait = false;
    }
    public bool MoveToPlayer() {
        agent.SetDestination(player.transform.position);
        return Vector3.Distance(transform.position, player.transform.position) < 3f;
    }

    public bool MoveToCaravan() {
        agent.SetDestination(caravan.transform.position);
        return Vector3.Distance(transform.position, caravan.transform.position) < 3f;
    }

    public void Wondering() {

        if (inWonderRange) {
            int nexIndex = GetNextIndex(preIndex);
            preIndex = nexIndex;
            agent.SetDestination(traders.traderPositions[nexIndex]);
            actionDestination = traders.traderPositions[nexIndex];
            inWonderRange = false;
        }

        if (Vector3.Distance(transform.position, actionDestination) < 5f) {
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

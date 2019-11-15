using System.Collections.Generic;
using UnityEngine;

public class Trader : MonoBehaviour {

    public GameObject traderPrefab;
    private List<GameObject> traderInstance;

    public Vector3[] traderPositions = {
        new Vector3(8f, -16.5f, 0f),
        new Vector3(-7.5f, -16.5f, 0f),
        new Vector3(-18f, 6.4f, 0f),
        new Vector3(-18f, -5.5f, 0f),
        new Vector3(-7.5f, 18f, 0f),
        new Vector3(8f, 18f, 0f),
        new Vector3(20f, 7.3f, 0f),
        new Vector3(20f, -5.5f, 0f)
    };

    void Start() {
        traderInstance = new List<GameObject>();

        for (int t = 0; t < traderPositions.Length; t++) {
            var tmp = traderPositions[t];
            var r = Random.Range(t, traderPositions.Length);
            traderPositions[t] = traderPositions[r];
            traderPositions[r] = tmp;
        }

        for (int i = 0; i < 8; i++) {
            GameObject trader = Instantiate(traderPrefab, traderPositions[i], transform.rotation, transform);
            trader.transform.Find("Text").GetComponent<TextMesh>().text = "Trader " + (i + 1);
            trader.gameObject.tag = "trader" + (i + 1);
            traderInstance.Add(trader);
        }
    }
}

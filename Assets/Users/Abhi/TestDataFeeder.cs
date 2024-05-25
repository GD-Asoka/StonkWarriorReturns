using UnityEngine;

public class TestDataFeeder : MonoBehaviour
{
    public GraphPlotter graphPlotter;
    public float updateInterval = 1.0f; // Time interval between updates in seconds
    private float timer = 0.0f;

    void Start()
    {
        // Assuming you already have references to the graph plotter and have added stocks
        graphPlotter.AddStock("Stock1");
        graphPlotter.AddStock("Stock2");
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0.0f;
            AddNewDataPoints();
        }
    }

    void AddNewDataPoints()
    {
        // Here you would retrieve and add actual data points
        // For demonstration, we'll use random values

        float time = Time.time;
        float stock1Value = Random.Range(0f, 10f);
        float stock2Value = Random.Range(0f, 10f);

        graphPlotter.AddStockDataPoint("Stock1", new Vector2(time, stock1Value));
        graphPlotter.AddStockDataPoint("Stock2", new Vector2(time, stock2Value));
    }
}

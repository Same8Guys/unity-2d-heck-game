using UnityEngine;
using System.Collections;

public class Sensor : MonoBehaviour
{

    private int _ColCount = 0;

    private float _DisableTimer;

    private void OnEnable()
    {
        _ColCount = 0;
    }

    public bool State()
    {
        if (_DisableTimer > 0)
        {
            return false;
        }
        return _ColCount > 0;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        _ColCount++;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        _ColCount--;
    }

    void Update()
    {
        _DisableTimer -= Time.deltaTime;
    }

    public void Disable(float duration)
    {
        _DisableTimer = duration;
    }
}

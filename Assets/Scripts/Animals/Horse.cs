using UnityEngine;

public class Horse : Animal
{
    private void Start()
    {
        speed = Random.Range(5f, 6f);
    }
}

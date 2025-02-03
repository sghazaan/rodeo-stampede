using UnityEngine;

public class Horse : Animal
{
    private void Start()
    {
        speed = Random.Range(2f, 4.2f);
    }
}

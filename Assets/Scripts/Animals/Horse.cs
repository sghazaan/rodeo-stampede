using UnityEngine;

public class Horse : Animal
{
    private void Start()
    {
        speed = Random.Range(3f, 4.2f);
    }
}

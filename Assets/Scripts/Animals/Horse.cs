using UnityEngine;

public class Horse : Animal
{
    private void Start()
    {
        speed = Random.Range(1f, 2f);
    }
}

using UnityEngine;

public class Elephant : Animal
{
    private void Start()
    {
        speed = Random.Range(1.5f, 3.5f);
    }
}

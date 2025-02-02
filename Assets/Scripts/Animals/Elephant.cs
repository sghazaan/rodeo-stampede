using UnityEngine;

public class Elephant : Animal
{
    private void Start()
    {
        speed = Random.Range(1f, 1.5f);
    }
}

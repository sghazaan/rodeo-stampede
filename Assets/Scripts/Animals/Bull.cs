using UnityEngine;

public class Bull : Animal
{
    private void Start()
    {
        speed = Random.Range(1.5f, 2f);
    }
}

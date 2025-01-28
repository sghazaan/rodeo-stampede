using UnityEngine;

public class Bull : Animal
{
    private void Start()
    {
        speed = Random.Range(4f, 5f);
    }
}

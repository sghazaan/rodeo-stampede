using UnityEngine;

public class Bull : Animal
{
    private void Start()
    {
        speed = Random.Range(2f, 4f);
    }
}

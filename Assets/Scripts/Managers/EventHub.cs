using System;
using UnityEngine;

public static class EventHub
{
    // Event that sends a float parameter (Y position)
    public static event Action<float, Animal> OnAnimalRidden;

    // Method to invoke the event
    public static void InvokeAnimalRidden(float yPos, Animal animal)
    {
        OnAnimalRidden?.Invoke(yPos, animal);
    }
}

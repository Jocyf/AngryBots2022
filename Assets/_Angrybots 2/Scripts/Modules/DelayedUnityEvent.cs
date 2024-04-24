using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class DelayedEvent : object
{
    [Tooltip("GameObject receptor de la señal")]
    public UnityEvent eventToFire;
    [Tooltip("Tiempo de demora antes de enviar la señal")]
    public float delay = 0f;

    // Este método es una corrutina que se utiliza para enviar la señal al objeto receptor después de un tiempo de demora especificado
    public virtual IEnumerator SendWithDelay(MonoBehaviour sender)
    {
        yield return new WaitForSeconds(delay);
        eventToFire.Invoke();
    }
}

[System.Serializable]
public class DelayedUnityEvent : object
{
    [Tooltip("Determina si la señal debe enviarse solo una vez o múltiples veces")]
    public bool onlyOnce;
    [Tooltip("Array de Events que representan los objetos receptores de la señal")]
    public DelayedEvent[] delayedEvents;

    [SerializeField]
    [Disable]
    private bool hasFired = false;  // Controla si la señal ya ha sido enviada al menos una vez

    // Envia señales a todos los objetos receptores almacenados en el array receivers
    public virtual void SendSignals(MonoBehaviour sender)
    {
        if ((hasFired == false) || (onlyOnce == false))
        {
            int i = 0;
            while (i < delayedEvents.Length)
            {
                sender.StartCoroutine(delayedEvents[i].SendWithDelay(sender));   // Inicia la corrutina SendWithDelay para cada objeto receptor del array
                i++;
            }
            hasFired = true;    // Asigna a la varialble hasFired = true, indicando que la señal ya ha sido enviada al menos una vez
        }
    }
}
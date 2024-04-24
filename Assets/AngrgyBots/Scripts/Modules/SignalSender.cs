using UnityEngine;
using System.Collections;

[System.Serializable]
public class ReceiverItem : object
{
    [Tooltip("GameObject receptor de la se�al")]
    public GameObject receiver;
    [Tooltip("Nombre de la acci�n a ejecutar en el objeto receptor")]
    public string action = "OnSignal";
    [Tooltip("Tiempo de demora antes de enviar la se�al")]
    public float delay = 0f;

    // Este m�todo es una corrutina que se utiliza para enviar la se�al al objeto receptor despu�s de un tiempo de demora especificado
    public virtual IEnumerator SendWithDelay(MonoBehaviour sender)
    {
        yield return new WaitForSeconds(delay);
        if (receiver)
        {
            receiver.SendMessage(action);   // Si existe un objeto receptor, llama al m�todo especificado en la propiedad action
        }
        else
        {
            Debug.LogWarning(((((("No receiver of signal \"" + action) + "\" on object ") + sender.name) + " (") + sender.GetType().Name) + ")", sender);
        }
    }
}

[System.Serializable]
public class SignalSender : object
{
    [Tooltip("Determina si la se�al debe enviarse solo una vez o m�ltiples veces")]
    public bool onlyOnce;
    [Tooltip("Array de objetos ReceiverItem que representan los objetos receptores de la se�al")]
    public ReceiverItem[] receivers;

    [SerializeField]
    [Disable]
    private bool hasFired = false;  // Controla si la se�al ya ha sido enviada al menos una vez

    // Envia se�ales a todos los objetos receptores almacenados en el array receivers
    public virtual void SendSignals(MonoBehaviour sender)
    {
        if ((hasFired == false) || (onlyOnce == false))
        {
            int i = 0;
            while (i < receivers.Length)
            {
                sender.StartCoroutine(receivers[i].SendWithDelay(sender));   // Inicia la corrutina SendWithDelay para cada objeto receptor del array
                i++;
            }
            hasFired = true;    // Asigna a la varialble hasFired = true, indicando que la se�al ya ha sido enviada al menos una vez
        }
    }
}
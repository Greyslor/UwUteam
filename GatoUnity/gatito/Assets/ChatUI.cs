using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System;

public class ChatUI : MonoBehaviour
{
    [Header("Referencias UI")]
    public InputField messageInput;
    public Button sendButton;
    public Transform messagesContainer; // para el Content del Scrollview
    public GameObject messagePrefab;    

    [Header("Configuración")]
    public string baseUrl = "http://localhost/php/chat.php";
    public string sala = "anime";
    public string usuario = "frank";
    private string mensajesPrevios = "";


    void Start()
    {
        sendButton.onClick.AddListener(EnviarMensaje);
        StartCoroutine(GetMessagesRoutine()); // actualiza
    }

    void EnviarMensaje()
    {
        string mensaje = messageInput.text;
        if (mensaje != "")
        {
            StartCoroutine(SendMessageRequest(mensaje));
            messageInput.text = "";
        }
    }

    IEnumerator SendMessageRequest(string message)
    {
        string url = $"{baseUrl}?action=3&room={sala}&username={usuario}&message={UnityWebRequest.EscapeURL(message)}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al enviar: " + request.error);
        }
        else
        {
            StartCoroutine(GetMessagesRequest()); 
        }
    }

    IEnumerator GetMessagesRoutine()
    {
        while (true)
        {
            yield return StartCoroutine(GetMessagesRequest());
            yield return new WaitForSeconds(3f); // tambien actualiza cada 3 segs
        }
    }

    IEnumerator GetMessagesRequest()
    {
        string url = $"{baseUrl}?action=2&room={sala}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al recibir: " + request.error);
        }
        else
        {
            string[] mensajes = request.downloadHandler.text.Split('\n');
            MostrarMensajes(mensajes);
        }
    }


    void MostrarMensajes(string[] mensajes)
    {
        string todoJunto = string.Join("\n", mensajes);

        if (todoJunto == mensajesPrevios) return;

        mensajesPrevios = todoJunto;

        foreach (Transform child in messagesContainer)
        {
            Destroy(child.gameObject); 
        }

        foreach (string m in mensajes)
        {
            if (string.IsNullOrWhiteSpace(m)) continue;
            GameObject nuevo = Instantiate(messagePrefab, messagesContainer);
            nuevo.GetComponent<Text>().text = m;
        }

        // Para forzar el scroll
        Canvas.ForceUpdateCanvases();
        ScrollRect scroll = messagesContainer.GetComponentInParent<ScrollRect>();
        scroll.verticalNormalizedPosition = 0f;
    }

    public void LimpiarChatServidor() //no funciona pq lo limpia y lo recarga pipipipi
    {
        StartCoroutine(ClearChatRequest());
    }

    IEnumerator ClearChatRequest()
    {
        string url = $"{baseUrl}?action=4";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al limpiar: " + request.error);
        }
        else
        {
            Debug.Log("Chat limpiado");
            
            foreach (Transform child in messagesContainer)
            {
                Destroy(child.gameObject);
            }
        }
    }


}

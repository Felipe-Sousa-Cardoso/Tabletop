using TMPro;
using UnityEngine;

public class CanvasFlutuante : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] Token token;
    [SerializeField] TextMeshProUGUI nomeDoToken;
    private Transform cameraTransform;
    void Start()
    {
        canvas.worldCamera = Camera.main;
        cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        nomeDoToken.text = token.nome.Value.ToString();
    }


    void LateUpdate()
    {
        if (cameraTransform != null)
        {
            // O LookAt faz o objeto olhar para a câmera.
            // O (transform.position + cameraTransform.rotation * Vector3.forward)
            // é uma maneira mais robusta de garantir que o objeto não "vira" de cabeça para baixo.
            transform.LookAt(transform.position + cameraTransform.rotation * Vector3.forward,
                             cameraTransform.rotation * Vector3.up);
        }
    }
}

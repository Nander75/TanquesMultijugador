using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Singleton Instance
    public static UIManager Instance { get; private set; }

    // Propiedad con Get público y Set privado, visible en el Inspector
    [field: SerializeField] public TMP_Text AmmoText { get; private set; }

    private void Awake()
    {
        // Lógica de Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

}

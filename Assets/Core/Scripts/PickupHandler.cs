using UnityEngine;
using TMPro;

//Handles picking of collectible orbs and calling game manager win condition
public class PickupHandler : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private int totalRequired = 5;
    [SerializeField] private LayerMask collectibleMask;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI progressLabel;

    private int collected;

    private void Start() => UpdateDisplay();

    private void OnCollisionEnter(Collision other)
    {
        if (IsCollectible(other.gameObject))
            Gather(other.gameObject);
    }

    private bool IsCollectible(GameObject obj) =>
        (collectibleMask & (1 << obj.layer)) != 0;

    private void Gather(GameObject item)
    {
        collected++;
        Destroy(item);
        UpdateDisplay();

        if (collected >= totalRequired)
            GameManager.Instance.GameWon();
    }

    private void UpdateDisplay()
    {
        if (progressLabel != null)
            progressLabel.text = $"Orbs: {collected} / {totalRequired}";
    }
}

using UnityEngine;
using System.Collections;

public enum GravityDirection
{
    Down,
    Up,
    Right,
    Left
}

//Handles gravity switching and player rotation
public class GravityController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject previewHologram;
    [SerializeField] private MonoBehaviour locomotionScript;

    [Header("Settings")]
    [SerializeField] private float flipDuration = 0.5f;

    [SerializeField] private GravityDirection selectedDirection = GravityDirection.Down;

    private bool PreviewActive => previewHologram != null && previewHologram.activeSelf;

    private static readonly KeyCode[] DirectionKeys =
    {
        KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.RightArrow, KeyCode.LeftArrow
    };

    private static readonly GravityDirection[] DirectionMap =
    {
        GravityDirection.Up, GravityDirection.Down, GravityDirection.Right, GravityDirection.Left
    };

    private void Start()
    {
        if (previewHologram != null)
            previewHologram.SetActive(false);
    }

    private void Update()
    {
        HandleDirectionInput();

        if (Input.GetKeyDown(KeyCode.Return) && PreviewActive)
            ExecuteGravityFlip();
    }

    private void HandleDirectionInput()
    {
        for (int i = 0; i < DirectionKeys.Length; i++)
        {
            if (Input.GetKeyDown(DirectionKeys[i]))
            {
                DisplayPreview(DirectionMap[i]);
                return;
            }
        }
    }

    private void DisplayPreview(GravityDirection direction)
    {
        if (previewHologram == null) return;

        selectedDirection = direction;
        previewHologram.transform.localRotation = Quaternion.Euler(DirectionToEuler(direction));
        previewHologram.SetActive(true);
    }

    private void ExecuteGravityFlip()
    {
        SetLocomotionActive(false);
        Invoke(nameof(BeginRotation), 0.1f);
        Invoke(nameof(ResumeLocomotion), 0.2f);
    }

    private void BeginRotation() =>
        StartCoroutine(AnimateRotation(previewHologram.transform.rotation));

    private void ResumeLocomotion() => SetLocomotionActive(true);

    private IEnumerator AnimateRotation(Quaternion target)
    {
        var origin = transform.rotation;
        float progress = 0f;

        while (progress < flipDuration)
        {
            transform.rotation = Quaternion.Lerp(origin, target, progress / flipDuration);
            progress += Time.deltaTime;
            yield return null;
        }

        transform.rotation = target;
        previewHologram.SetActive(false);
    }

    private void SetLocomotionActive(bool state)
    {
        if (locomotionScript != null)
            locomotionScript.enabled = state;
    }

    //Hardcoded rotation values for Hologram visuals
    private static Vector3 DirectionToEuler(GravityDirection dir) => dir switch
    {
        GravityDirection.Down  => new Vector3(90, 0, 0),
        GravityDirection.Up    => new Vector3(90, 0, -180),
        GravityDirection.Right => new Vector3(90, 0, 90),
        GravityDirection.Left  => new Vector3(90, 0, -90), 
        _ => Vector3.zero
    };
}

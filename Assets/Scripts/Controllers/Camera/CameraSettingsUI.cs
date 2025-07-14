using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class CameraSettingsUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CameraController cameraController;

    [Header("UI Controls")]
    public TMP_Dropdown cameraTypeDropdown;
    public TMP_Dropdown projectionDropdown;

    public TMP_InputField distanceInput;
    public TMP_InputField heightInput;
    public TMP_InputField pitchInput;
    public TMP_InputField yawInput;
    public TMP_InputField fovInput;

    public TMP_InputField offsetXInput;
    public TMP_InputField offsetYInput;
    public TMP_InputField offsetZInput;

    public TMP_InputField orthoSizeInput;
    public GameObject cards;

    public Toggle cardsVisivbeToggle;
    public Button copyButton;

    [Header("Debug Output")]
    public TMP_Text debugText;

    private void Start()
    {
        cameraTypeDropdown.ClearOptions();
        cameraTypeDropdown.AddOptions(Enum.GetNames(typeof(CameraType)).ToList());
        cameraTypeDropdown.onValueChanged.AddListener(index =>
        {
            cameraController.cameraType = (CameraType)index;
        });

        projectionDropdown.ClearOptions();
        projectionDropdown.AddOptions(Enum.GetNames(typeof(ProjectionMode)).ToList());
        projectionDropdown.onValueChanged.AddListener(index =>
        {
            cameraController.projectionMode = (ProjectionMode)index;
        });

        copyButton.onClick.AddListener(() => GUIUtility.systemCopyBuffer = debugText.text);

        distanceInput.onEndEdit.AddListener(value => TryParseFloat(value, v => cameraController.distance = v));
        heightInput.onEndEdit.AddListener(value => TryParseFloat(value, v => cameraController.height = v));
        pitchInput.onEndEdit.AddListener(value => TryParseFloat(value, v => cameraController.pitch = v));
        yawInput.onEndEdit.AddListener(value => TryParseFloat(value, v => cameraController.yaw = v));
        fovInput.onEndEdit.AddListener(value => TryParseFloat(value, v => cameraController.fov = v));
        orthoSizeInput.onEndEdit.AddListener(value => TryParseFloat(value, v => cameraController.orthographicSize = v));

        cardsVisivbeToggle.onValueChanged.AddListener(value => cards.SetActive(value));

        offsetXInput.onEndEdit.AddListener(_ => UpdateOffset());
        offsetYInput.onEndEdit.AddListener(_ => UpdateOffset());
        offsetZInput.onEndEdit.AddListener(_ => UpdateOffset());

        SyncUIWithCamera();
    }

    private void TryParseFloat(string input, Action<float> onSuccess)
    {
        if (float.TryParse(input, out float result))
            onSuccess(result);
    }

    private void UpdateOffset()
    {
        float.TryParse(offsetXInput.text, out float x);
        float.TryParse(offsetYInput.text, out float y);
        float.TryParse(offsetZInput.text, out float z);

        cameraController.offset = new Vector3(x, y, z);
    }

    private void Update()
    {
        if (debugText == null || cameraController == null)
            return;

        Vector3 pos = cameraController.transform.position;
        Vector3 rot = cameraController.transform.eulerAngles;

        string debugInfo = $"<b>Camera Debug Info</b>\n" +
                           $"Type: {cameraController.cameraType}\n" +
                           $"Projection: {cameraController.projectionMode}\n" +
                           $"Position: {pos:F2}\n" +
                           $"Rotation: {rot:F2}\n" +
                           $"Distance: {cameraController.distance:F2}\n" +
                           $"Height: {cameraController.height:F2}\n" +
                           $"Pitch: {cameraController.pitch:F1}\n" +
                           $"Yaw: {cameraController.yaw:F1}\n" +
                           $"FOV: {cameraController.fov:F1}\n";

        if (cameraController.projectionMode == ProjectionMode.Orthographic)
            debugInfo += $"Ortho Size: {cameraController.orthographicSize:F2}\n";

        debugText.text = debugInfo;
    }

    private void SyncUIWithCamera()
    {
        cameraTypeDropdown.value = (int)cameraController.cameraType;
        projectionDropdown.value = (int)cameraController.projectionMode;

        distanceInput.text = cameraController.distance.ToString("F2");
        heightInput.text = cameraController.height.ToString("F2");
        pitchInput.text = cameraController.pitch.ToString("F1");
        yawInput.text = cameraController.yaw.ToString("F1");
        fovInput.text = cameraController.fov.ToString("F1");

        offsetXInput.text = cameraController.offset.x.ToString("F2");
        offsetYInput.text = cameraController.offset.y.ToString("F2");
        offsetZInput.text = cameraController.offset.z.ToString("F2");

        orthoSizeInput.text = cameraController.orthographicSize.ToString("F2");
    }
}

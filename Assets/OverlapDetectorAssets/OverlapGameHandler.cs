using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OverlapGameHandler : MonoBehaviour 
{
    [Header("Game Objects")]
    [SerializeField] private GameObject sphereObject;
    [SerializeField] private GameObject cubeObject;

    [Header("Buttons")]
    [SerializeField] private Button hideInputPanelButton;
    [SerializeField] private Button showInputPanelButton;

    [Header("Panels")]
    [SerializeField] private GameObject inputPanel;

    [Header("Position Input Fields")]
    [SerializeField] private TMP_InputField spherePosXInput;
    [SerializeField] private TMP_InputField spherePosYInput;
    [SerializeField] private TMP_InputField spherePosZInput;

    [SerializeField] private TMP_InputField cubePosXInput;
    [SerializeField] private TMP_InputField cubePosYInput;
    [SerializeField] private TMP_InputField cubePosZInput;

    [Header("Scale Input Fields")]
    [SerializeField] private TMP_InputField sphereScaleInput;
    [SerializeField] private TMP_InputField cubeScaleInput;

    [Header("Visualization")]
    [SerializeField] private OverlapDetector overlapDetector;

    [Header("Colors")]
    [SerializeField] private Color overlapColor = Color.red;
    [SerializeField] private Color noOverlapColor = Color.green;
    [Header("Result Text")]
    [SerializeField] private TMP_Text resultText;

    private void Start()
    {
        inputPanel.SetActive(false);
        // Initial setup of input field listeners
        SetupInputListeners();
        
        // Initial update of visualization
        UpdateOverlapVisualization();
    }

    private void SetupInputListeners()
    {
        hideInputPanelButton.onClick.AddListener(HideInputPanel);
        showInputPanelButton.onClick.AddListener(ShowInputPanel);
        // Sphere Position Inputs
        spherePosXInput.onValueChanged.AddListener((value) => UpdateSpherePosition());
        spherePosYInput.onValueChanged.AddListener((value) => UpdateSpherePosition());
        spherePosZInput.onValueChanged.AddListener((value) => UpdateSpherePosition());

        // Cube Position Inputs
        cubePosXInput.onValueChanged.AddListener((value) => UpdateCubePosition());
        cubePosYInput.onValueChanged.AddListener((value) => UpdateCubePosition());
        cubePosZInput.onValueChanged.AddListener((value) => UpdateCubePosition());

        // Scale Inputs
        sphereScaleInput.onValueChanged.AddListener((value) => UpdateSphereScale());
        cubeScaleInput.onValueChanged.AddListener((value) => UpdateCubeScale());
    }

    private void UpdateSpherePosition()
    {
        // Parse input values, default to 0 if invalid
        float x = ParseFloatInput(spherePosXInput, 0f);
        float y = ParseFloatInput(spherePosYInput, 0f);
        float z = ParseFloatInput(spherePosZInput, 0f);

        // Update sphere position
        sphereObject.transform.position = new Vector3(x, y, z);

        // Update overlap visualization
        UpdateOverlapVisualization();
    }

    private void UpdateCubePosition()
    {
        // Parse input values, default to 0 if invalid
        float x = ParseFloatInput(cubePosXInput, 0f);
        float y = ParseFloatInput(cubePosYInput, 0f);
        float z = ParseFloatInput(cubePosZInput, 0f);

        // Update cube position
        cubeObject.transform.position = new Vector3(x, y, z);

        // Update overlap visualization
        UpdateOverlapVisualization();
    }

    private void UpdateSphereScale()
    {
        // Parse scale input, default to 1 if invalid
        float scale = ParseFloatInput(sphereScaleInput, 1f);

        // Apply uniform scale to sphere
        sphereObject.transform.localScale = Vector3.one * scale;

        // Update overlap visualization
        UpdateOverlapVisualization();
    }

    private void UpdateCubeScale()
    {
        // Parse scale input, default to 1 if invalid
        float scale = ParseFloatInput(cubeScaleInput, 1f);

        // Apply uniform scale to cube
        cubeObject.transform.localScale = Vector3.one * scale;

        // Update overlap visualization
        UpdateOverlapVisualization();
    }

    private void UpdateOverlapVisualization()
    {
        // Check for overlap using the intersection detector
        bool isOverlapping = overlapDetector.CheckBoxSphereIntersection(sphereObject, cubeObject);

        // Update indicator color based on overlap status
        sphereObject.GetComponent<Renderer>().material.color = isOverlapping ? overlapColor : noOverlapColor;
        cubeObject.GetComponent<Renderer>().material.color = isOverlapping ? overlapColor : noOverlapColor;
        resultText.text = isOverlapping ? "Overlap Detected" : "No Overlap Detected";
    }

    private float ParseFloatInput(TMP_InputField input, float defaultValue)
    {
        // Try to parse input, return default if parsing fails
        if (float.TryParse(input.text, out float result))
        {
            return result;
        }
        return defaultValue;
    }

    // Optional: Method to reset all values to default
    public void ResetToDefaults()
    {
        // Reset positions
        sphereObject.transform.position = Vector3.zero;
        cubeObject.transform.position = Vector3.zero;

        // Reset scales
        sphereObject.transform.localScale = Vector3.one;
        cubeObject.transform.localScale = Vector3.one;

        // Update input fields
        spherePosXInput.text = "0";
        spherePosYInput.text = "0";
        spherePosZInput.text = "0";
        cubePosXInput.text = "0";
        cubePosYInput.text = "0";
        cubePosZInput.text = "0";
        sphereScaleInput.text = "1";
        cubeScaleInput.text = "1";

        // Update visualization
        UpdateOverlapVisualization();
    }

    #region Button Listeners
    private void HideInputPanel()
    {
        inputPanel.SetActive(false);
    }

    private void ShowInputPanel()
    {
        inputPanel.SetActive(true);
    }
    #endregion
}
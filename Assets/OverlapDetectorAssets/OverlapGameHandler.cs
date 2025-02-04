using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OverlapGameHandler : MonoBehaviour 
{
    [Header("Game Objects")]
    public GameObject sphereObject;
    public GameObject cubeObject;

    [Header("Position Input Fields")]
    public TMP_InputField spherePosXInput;
    public TMP_InputField spherePosYInput;
    public TMP_InputField spherePosZInput;

    public TMP_InputField cubePosXInput;
    public TMP_InputField cubePosYInput;
    public TMP_InputField cubePosZInput;

    [Header("Scale Input Fields")]
    public TMP_InputField sphereScaleInput;
    public TMP_InputField cubeScaleInput;

    [Header("Visualization")]
    public Image overlapIndicator;
    public BoxSphereIntersection intersectionDetector;

    [Header("Colors")]
    public Color overlapColor = Color.red;
    public Color noOverlapColor = Color.green;

    private void Start()
    {
        // Initial setup of input field listeners
        SetupInputListeners();
        
        // Initial update of visualization
        UpdateOverlapVisualization();
    }

    private void SetupInputListeners()
    {
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
        bool isOverlapping = intersectionDetector.CheckBoxSphereIntersection(sphereObject, cubeObject);

        // Update indicator color based on overlap status
        overlapIndicator.color = isOverlapping ? overlapColor : noOverlapColor;
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
}
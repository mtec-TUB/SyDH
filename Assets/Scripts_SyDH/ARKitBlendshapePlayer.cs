using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ARKitBlendshapePlayer : MonoBehaviour
{
    [Header("Assign your CC head mesh (SkinnedMeshRenderer)")]
    public SkinnedMeshRenderer characterHead;

    [Header("Drop your exported JSON here")]
    public TextAsset blendshapeDataJson;

    [Header("Optional: sync with audio")]
    public AudioSource audioSource;

    [Header("Settings")]
    public float frameDuration = 1f / 60f; // 60 FPS playback
    public float startDelay = 1f; // wait seconds before starting
    public float expressivenessFactor = 1f; // Multiply Blendshapes values

    [Header("Smoothness")]
    [Range(0f, 1f)]
    public float smoothness = 0.5f; // 0 = no smoothing, 1 = very smooth

    [Header("Symmetry Settings")]
    [Range(0, 1)]
    public float symmetryAmount = 0f;

    // Define symmetry in ARKit space, not CC4
    private List<(string left, string right)> ARKitSymmetryPairs = new List<(string, string)>
    {
        ("jawLeft", "jawRight"),
        ("mouthSmileLeft", "mouthSmileRight"),
        ("mouthFrownLeft", "mouthFrownRight"),
        ("mouthDimpleLeft", "mouthDimpleRight"),
        ("mouthPressLeft", "mouthPressRight"),
        ("mouthUpperUpLeft", "mouthUpperUpRight"),
        ("mouthLowerDownLeft", "mouthLowerDownRight"),
        ("mouthLeft", "mouthRight"),
        ("mouthStretchLeft", "mouthStretchRight"),
        ("cheekSquintLeft", "cheekSquintRight"),
        ("noseSneerLeft", "noseSneerRight"),
    };

    // Mapping ARKit names -> CC4 blendshape names
    private Dictionary<string, List<string>> NAME_MAP = new Dictionary<string, List<string>>()
    {
// Brows
        { "browInnerUp", new List<string>{ "Brow_Raise_Inner_L", "Brow_Raise_Inner_R" } },
        { "browOuterUpLeft", new List<string>{ "Brow_Raise_Outer_L" } },
        { "browOuterUpRight", new List<string>{ "Brow_Raise_Outer_R" } },
        { "browDownLeft", new List<string>{ "Brow_Drop_L" } },
        { "browDownRight", new List<string>{ "Brow_Drop_R" } },

        // Eyes
        { "eyeBlinkLeft", new List<string>{ "Eye_Blink_L" } },
        { "eyeBlinkRight", new List<string>{ "Eye_Blink_R" } },
        { "eyeSquintLeft", new List<string>{ "Eye_Squint_L" } },
        { "eyeSquintRight", new List<string>{ "Eye_Squint_R" } },
        { "eyeWideLeft", new List<string>{ "Eye_Wide_L" } },
        { "eyeWideRight", new List<string>{ "Eye_Wide_R" } },
        { "eyeLookDownLeft", new List<string>{ "Eye_L_Look_Down" } },
        { "eyeLookDownRight", new List<string>{ "Eye_R_Look_Down" } },
        { "eyeLookInLeft", new List<string>{ "Eye_L_Look_R" } },
        { "eyeLookInRight", new List<string>{ "Eye_R_Look_L" } },
        { "eyeLookOutLeft", new List<string>{ "Eye_L_Look_L" } },
        { "eyeLookOutRight", new List<string>{ "Eye_R_Look_R" } },
        { "eyeLookUpLeft", new List<string>{ "Eye_L_Look_Up" } },
        { "eyeLookUpRight", new List<string>{ "Eye_R_Look_Up" } },

        // Jaw
        { "jawOpen", new List<string>{ "Merged_Open_Mouth" } },
        { "jawForward", new List<string>{ "Jaw_Forward" } },
        { "jawLeft", new List<string>{ "Jaw_L" } },
        { "jawRight", new List<string>{ "Jaw_R" } },

        // Mouth
        { "mouthSmileLeft", new List<string>{ "Mouth_Smile_L" } },
        { "mouthSmileRight", new List<string>{ "Mouth_Smile_R" } },
        { "mouthFrownLeft", new List<string>{ "Mouth_Frown_L" } },
        { "mouthFrownRight", new List<string>{ "Mouth_Frown_R" } },
        { "mouthDimpleLeft", new List<string>{ "Mouth_Dimple_L" } },
        { "mouthDimpleRight", new List<string>{ "Mouth_Dimple_R" } },
        { "mouthPressLeft", new List<string>{ "Mouth_Press_L" } }, // Press should be checked with arkit docs again
        { "mouthPressRight", new List<string>{ "Mouth_Press_R" } },
        { "mouthPucker", new List<string>{ "Mouth_Pucker_Up_L", "Mouth_Pucker_Up_R", "Mouth_Pucker_Down_L", "Mouth_Pucker_Down_R" } },
        { "mouthFunnel", new List<string>{ "Mouth_Funnel_Up_L", "Mouth_Funnel_Up_R", "Mouth_Funnel_Down_L", "Mouth_Funnel_Down_R" } },
        { "mouthShrugUpper", new List<string>{ "Mouth_Shrug_Upper" } },
        { "mouthShrugLower", new List<string>{ "Mouth_Shrug_Lower" } },
        { "mouthClose", new List<string>{ "Mouth_Close" } },
        { "mouthUpperUpLeft", new List<string>{ "Mouth_Up_Upper_L" } },
        { "mouthUpperUpRight", new List<string>{ "Mouth_Up_Upper_R" } },
        { "mouthLowerDownLeft", new List<string>{ "Mouth_Down_Lower_L" } },
        { "mouthLowerDownRight", new List<string>{ "Mouth_Down_Lower_R" } },
        { "mouthLeft", new List<string>{ "Mouth_L" } },
        { "mouthRight", new List<string>{ "Mouth_R" } },
        { "mouthRollLower", new List<string>{ "Mouth_Roll_In_Lower_L", "Mouth_Roll_In_Lower_R" } },
        { "mouthRollUpper", new List<string>{ "Mouth_Roll_In_Upper_L", "Mouth_Roll_In_Upper_R" } },
        { "mouthStretchLeft", new List<string>{ "Mouth_Stretch_L" } },
        { "mouthStretchRight", new List<string>{ "Mouth_Stretch_R" } },

        // Cheeks
        { "cheekPuff", new List<string>{ "Cheek_Puff_L", "Cheek_Puff_R" } },
        { "cheekSquintLeft", new List<string>{ "Cheek_Raise_L" } },
        { "cheekSquintRight", new List<string>{ "Cheek_Raise_R" } },

        // Nose
        { "noseSneerLeft", new List<string>{ "Nose_Sneer_L" } },
        { "noseSneerRight", new List<string>{ "Nose_Sneer_R" } },

        // Tongue
        { "tongueOut", new List<string>{ "Tongue_Out" } },
    };

    private List<Dictionary<string, float>> frames;
    private float startTime;
    private bool started = false;

    private Dictionary<string, int> cc4NameToIndex = new Dictionary<string, int>();

    private float[] previousWeights;

    private bool IsExcludedFromExpressiveness(string ccName)
    {
        string lower = ccName.ToLower();
        return lower.Contains("eye") || lower.Contains("brow");
    }

    void Start()
    {
        if (characterHead == null || blendshapeDataJson == null)
        {
            Debug.LogError("Assign Character Head + JSON file in Inspector.");
            return;
        }

        Mesh mesh = characterHead.sharedMesh;

        previousWeights = new float[mesh.blendShapeCount];

        // Build name->index lookup
        for (int i = 0; i < mesh.blendShapeCount; i++)
        {
            string clean = CleanName(mesh.GetBlendShapeName(i));
            cc4NameToIndex[clean] = i;
        }

        // Parse JSON
        var dict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, float>>>(blendshapeDataJson.text);
        var sortedKeys = new List<string>(dict.Keys);
        sortedKeys.Sort((a, b) => float.Parse(a).CompareTo(float.Parse(b)));

        frames = new List<Dictionary<string, float>>();
        foreach (var key in sortedKeys)
            frames.Add(dict[key]);

        startTime = Time.time;
    }

    void Update()
    {
        if (frames == null || frames.Count == 0) return;

        float elapsed = Time.time - startTime;

        if (!started && elapsed >= startDelay)
        {
            started = true;
            if (audioSource != null) audioSource.Play();
            startTime = Time.time;
        }

        if (!started) return;

        elapsed = Time.time - startTime;
        int frameIndex = Mathf.Min(Mathf.FloorToInt(elapsed / frameDuration), frames.Count - 1);
        var frame = frames[frameIndex];

        float[] weights = new float[characterHead.sharedMesh.blendShapeCount];

        // --- Step 1: Apply ARKit values mapped to CC4 indices ---
        foreach (var kv in frame)
        {
            if (NAME_MAP.TryGetValue(kv.Key, out var ccNames))
            {
                foreach (var ccName in ccNames)
                {
                    int idx = FindIndex(ccName);
                    float factor = IsExcludedFromExpressiveness(ccName) ? 1f : expressivenessFactor;
                    if (idx != -1)
                    {
                    weights[idx] = kv.Value * 100f * factor;
                    }
                }
            }
        }

        // --- Step 2: Symmetry based on ARKit pairs using mapping ---
        if (symmetryAmount > 0.001f)
        {
            foreach (var (arkitLeft, arkitRight) in ARKitSymmetryPairs)
            {
                if (!NAME_MAP.ContainsKey(arkitLeft) || !NAME_MAP.ContainsKey(arkitRight))
                    continue;

                var leftList = NAME_MAP[arkitLeft];
                var rightList = NAME_MAP[arkitRight];

                for (int i = 0; i < Mathf.Min(leftList.Count, rightList.Count); i++)
                {
                    int leftIdx = FindIndex(leftList[i]);
                    int rightIdx = FindIndex(rightList[i]);
                    if (leftIdx == -1 || rightIdx == -1)
                        continue;

                    float leftVal = weights[leftIdx];
                    float rightVal = weights[rightIdx];
                    float avg = (leftVal + rightVal) / 2f;

                    weights[leftIdx] = Mathf.Lerp(leftVal, avg, symmetryAmount);
                    weights[rightIdx] = Mathf.Lerp(rightVal, avg, symmetryAmount);
                }
            }
        }

        // --- Step 3: Smooth and apply all weights ---
        for (int i = 0; i < weights.Length; i++)
        {
            // Smooth transition using exponential moving average
            float smoothed = Mathf.Lerp(previousWeights[i], weights[i], 1f - smoothness);
            characterHead.SetBlendShapeWeight(i, smoothed);
            previousWeights[i] = smoothed;
        }
    }

    private int FindIndex(string name)
    {
        string clean = CleanName(name);
        return cc4NameToIndex.TryGetValue(clean, out int idx) ? idx : -1;
    }

    private string CleanName(string name)
    {
        return name.ToLower().Replace("_", "").Replace(" ", "");
    }

    public void LoadJson(TextAsset json)
{
    blendshapeDataJson = json;
    Start(); // reinitialize animation data
}

public float GetAnimationDuration()
{
    if (frames == null) return 0;
    return frames.Count * frameDuration;
}
}
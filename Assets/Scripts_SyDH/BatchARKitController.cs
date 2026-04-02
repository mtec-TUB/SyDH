using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
#if UNITY_EDITOR
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
#endif

public class BatchARKitController : MonoBehaviour
{
    [Header("Character Animator")]
    public ARKitBlendshapePlayer animator;
    public AudioSource audioSource;

    [Header("Folders")]
    public string jsonFolder = "Assets/JSON_SyDH-main/s9";
    public string audioFolder = "Assets/Audio_SyDH-main/s9";
    public string outputFolder = "Recordings";

    [Header("Parameters")]
    public float expressiveness = 1f;
    public float symmetry = 0f;
    public float smoothness = 0.0f;

    [Header("Batch Settings")]
    public float preRollTime = 0.2f;
    public float postRollTime = 0.0f;
    public bool autoStart = true;

#if UNITY_EDITOR
    private RecorderController recorderController;
#endif

    void Start()
    {
        if (autoStart)
            StartCoroutine(RunBatch());
    }

    IEnumerator RunBatch()
    {
        Directory.CreateDirectory(outputFolder);
        string[] jsonFiles = Directory.GetFiles(jsonFolder, "*.json");

        foreach (string jsonPath in jsonFiles)
        {
            string baseName = Path.GetFileNameWithoutExtension(jsonPath);
            string audioPath = Path.Combine(audioFolder, baseName + ".wav");
            
            if (!File.Exists(audioPath))
            {
                Debug.LogWarning($"❌ Missing audio for {baseName}");
                continue;
            }

            Debug.Log($"▶️ Running {baseName}");

            // Load audio
            string fullPath = Path.GetFullPath(audioPath).Replace("\\", "/");
            if (!fullPath.StartsWith("file://"))
                fullPath = "file://" + fullPath;
                
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(fullPath, AudioType.WAV))
            {
                yield return www.SendWebRequest();
                
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Failed to load audio: {audioPath}\n{www.error}");
                    continue;
                }

                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = clip;

                // Load JSON
                string jsonText = File.ReadAllText(jsonPath);
                var jsonAsset = new TextAsset(jsonText);
                animator.LoadJson(jsonAsset);

                // Apply parameters
                animator.expressivenessFactor = expressiveness;
                animator.symmetryAmount = symmetry;
                animator.smoothness = smoothness;
                animator.audioSource = audioSource;

                // Calculate total recording time
                float audioDuration = clip.length;
                float totalRecordingTime = preRollTime + audioDuration + postRollTime;

                // Start recording BEFORE anything else
#if UNITY_EDITOR
                string outputFile = Path.Combine(outputFolder, baseName);
                StartRecording(outputFile);
#endif

                // Wait pre-roll time
                yield return new WaitForSeconds(preRollTime);

                // Start audio and animation simultaneously
                audioSource.Play();

                // Wait for audio duration
                yield return new WaitForSeconds(audioDuration);

                // Wait post-roll time
                yield return new WaitForSeconds(postRollTime);

#if UNITY_EDITOR
                StopRecording();
#endif

                // Clean up
                DestroyImmediate(clip);
                Resources.UnloadUnusedAssets();

                Debug.Log($"✅ Finished {baseName}");
                
                // Small delay between recordings
                yield return new WaitForSeconds(0.2f);
            }
        }

        Debug.Log("🏁 All samples complete!");
    }

#if UNITY_EDITOR
    void StartRecording(string path)
    {
        var controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
        recorderController = new RecorderController(controllerSettings);

        var movieRecorder = ScriptableObject.CreateInstance<MovieRecorderSettings>();
        movieRecorder.name = "AutoRecorder";
        movieRecorder.Enabled = true;
        movieRecorder.OutputFormat = MovieRecorderSettings.VideoRecorderOutputFormat.MP4;
        movieRecorder.ImageInputSettings = new GameViewInputSettings
        {
            OutputWidth = 1920,
            OutputHeight = 1080
        };
        movieRecorder.AudioInputSettings.PreserveAudio = true;
        movieRecorder.OutputFile = path;

        controllerSettings.AddRecorderSettings(movieRecorder);
        controllerSettings.SetRecordModeToManual();
        controllerSettings.FrameRate = 60.0f;
        controllerSettings.CapFrameRate = true;

        recorderController.PrepareRecording();
        recorderController.StartRecording();

        Debug.Log($"🎥 Recording started: {path}");
    }

    void StopRecording()
    {
        if (recorderController != null && recorderController.IsRecording())
        {
            recorderController.StopRecording();
            Debug.Log("🎥 Recording stopped.");
        }
    }
#endif
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraController : MonoBehaviour
{
    [SerializeField] private int rendererCount = 1;

    private UniversalAdditionalCameraData cameraData;
    private UniversalRenderPipelineAsset universalRenderPipelineAsset;
    private int currentRendereIndex;

    // Start is called before the first frame update
    void Start()
    {
        cameraData = GetComponent<UniversalAdditionalCameraData>();
        cameraData.SetRenderer(currentRendereIndex);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchCameraRenderer();
        }
    }

    private void SwitchCameraRenderer()
    {
        currentRendereIndex = (currentRendereIndex + 1)%rendererCount;
        cameraData.SetRenderer(currentRendereIndex);

    }
}

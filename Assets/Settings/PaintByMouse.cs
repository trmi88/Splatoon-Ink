using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PaintByMouse : ScriptableRendererFeature
{
    class PainByMousePass : ScriptableRenderPass
    {
        public Settings settings;
        string m_ProfilerTag;

        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            bool click;
            click = settings.mouseSingleClick ? Input.GetMouseButtonDown(0) : Input.GetMouseButton(0);

            if (click)
            {
                Vector3 position = Input.mousePosition;
                Ray ray = settings.cam.ScreenPointToRay(position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100.0f))
                {
                    Debug.DrawRay(ray.origin, hit.point - ray.origin, Color.red);
                    //settings.mousePainter.transform.position = hit.point;
                    Paintable p = hit.collider.GetComponent<Paintable>();
                    if (p != null)
                    {
                        PaintManager.instance.paint(p, hit.point, settings.radius, settings.hardness, settings.strength, settings.paintColor);
                    }
                }
            }
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }

        public PainByMousePass(string tag)
        {
            m_ProfilerTag = tag;
        }

    }

    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;

        [Header("Mouse Settings")]
        //public GameObject mousePainter = GameObject.Find("MousePainter");
        public Camera cam;
        public bool mouseSingleClick;
        public Color paintColor;
        public float radius = 1;
        public float strength = 1;
        public float hardness = 1;
    }

    private void OnEnable()
    {
        settings.cam = Camera.main;
    }

    public Settings settings = new Settings();
    PainByMousePass m_ScriptablePass;

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new PainByMousePass(name);

    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        m_ScriptablePass.renderPassEvent = settings.renderPassEvent;
        m_ScriptablePass.settings = settings;
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


